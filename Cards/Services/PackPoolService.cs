using Cards.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Services
{
    public class PackPoolService
    {
        private List<Pack> PackPool { get; set; }
        private SuitRangePoolService suitRangePoolService;

        public PackPoolService(IServiceProvider serviceProvider)
        {
            PackPool = new List<Pack>();
            suitRangePoolService = serviceProvider.GetRequiredService<SuitRangePoolService>();
        }

        public enum Sort { Range = 1, Random = 2, ManualSimul = 3}

        private readonly Dictionary<Sort, string> SortView = new Dictionary<Sort, string>()
        {
            { Sort.ManualSimul, "Симуляция ручного перемешивания" },
            { Sort.Random, "Случайное"},
            { Sort.Range, "Упорядоченный"}
        };

        public async Task<Pack> AddToPool(Guid suitRangeId, string name)
        {
            return await Task.Run(() =>
            {
                var pack = new Pack
                {
                    Id = Guid.NewGuid(),
                    SuitRangeId = suitRangeId,
                    Name = name,
                    Cards = new List<Card>(),
                    SortedCards = new Dictionary<int, Guid>()
                };
                InitPack(pack);               
                lock (PackPool)
                {
                    PackPool.Add(pack);
                }
                return pack;
            });
        }

        public Pack GetPack(Guid guid)
        {
            var result = PackPool.Where(s=>s.Id == guid);
            if (result.Count() > 0)
            {
                return result.First();
            }
            return null;
        }

        public void DeletePack(Guid id)
        {
            lock (PackPool)
            {
                var res = PackPool.Find(s => s.Id == id);
                if(res!=null)
                    PackPool.Remove(res);
            }
        }

        public void DeletePack(string name)
        {
            lock (PackPool)
            {
                var res = PackPool.Find(s => s.Name == name);
                if (res != null)
                    PackPool.Remove(res);
            }
        }

        public Dictionary<Sort, string> GetSorts()
        {
            return SortView;
        }

        public List<Pack> GetPacks()
        {
            return PackPool;
        }

        public Pack GetPack(string name)
        {
            var result = PackPool.Where(s => s.Name == name);
            if (result.Count() > 0)
            {
                return result.First();
            }
            return null;
        }

        public async Task<Pack> SortPack(string name, Sort sort, int count = 1)
        {
            return await SortPack(GetPack(name), sort, count);
        }

        public async Task<Pack> SortPack(Guid guid, Sort sort, int count = 1)
        {
            return await SortPack(GetPack(guid), sort, count);           
        }

        private async Task<Pack> SortPack(Pack pack, Sort sort, int count = 1)
        {
            return await Task.Run(() =>
            {
                if (pack != null)
                {
                    switch (sort)
                    {
                        case Sort.ManualSimul:
                            ManualSimulSort(pack, count);
                            break;
                        case Sort.Random:
                            RandomSort(pack);
                            break;
                        case Sort.Range:
                            RangeSort(pack);
                            break;
                    }
                }
                return pack;
            });
        }

        private void ManualSimulSort(Pack pack, int count)
        {
            for (int a = 0; a < count; a++)
            {
                List<Guid> newList = new List<Guid>();
                List<Guid> oldList = pack.SortedCards.Select(s => s.Value).ToList();
                MoveBack(newList, oldList);
                Dictionary<int, Guid> tempPack = new Dictionary<int, Guid>();
                for (int s = 0; s < newList.Count; s++)
                {
                    tempPack.Add(s, newList[s]);
                }
                pack.SortedCards = tempPack;
            }
        }

        private void RandomSort(Pack pack)
        {
            int y = 0;
            Random random = new Random();
            List<Guid> guids = pack.Cards.Select(s => s.Id).ToList();
            while (guids.Count > 0)
            {
                var next = guids[random.Next(1, guids.Count) - 1];
                pack.SortedCards[y++] = next;
                guids.Remove(next);
            }

        }

        private void InitPack(Pack pack)
        {
            var suitRange = suitRangePoolService.GetSuitRange(pack.SuitRangeId);
            var suitSet = suitRangePoolService.GetSuitSet(suitRange.SuitSetId);
            var rangeSet = suitRangePoolService.GetRangeSet(suitRange.RangeSetId);
            int i = 0;
            foreach (var suit in suitSet.Suits.OrderBy(s => s.Value).Select(s => s.Key))
            {
                foreach (var range in rangeSet.Ranges.OrderBy(s => s.Value).Select(s => s.Key))
                {
                    var id = Guid.NewGuid();
                    pack.Cards.Add(new Card() { Id = id, RangeId = range, SuitId = suit });
                    pack.SortedCards.Add(i++, id);
                }
            }
        }

        private void RangeSort(Pack pack)
        {
            var suitRange = suitRangePoolService.GetSuitRange(pack.SuitRangeId);
            var suitSet = suitRangePoolService.GetSuitSet(suitRange.SuitSetId);
            var rangeSet = suitRangePoolService.GetRangeSet(suitRange.RangeSetId);
            int i = 0;
            pack.SortedCards = new Dictionary<int, Guid>();
            foreach (var suit in suitSet.Suits.OrderBy(s => s.Value).Select(s => s.Key))
            {
                foreach (var range in rangeSet.Ranges.OrderBy(s => s.Value).Select(s => s.Key))
                {
                    var id = pack.Cards.Find(s=>s.RangeId == range && s.SuitId == suit).Id;                    
                    pack.SortedCards.Add(i++, id);
                }
            }
        }

        private void MoveForward(List<Guid> newList, List<Guid> oldList)
        {
            Random random = new Random();
            if (oldList.Count > 2)
            {
                int offset = Math.Max((oldList.Count / 4) + random.Next(oldList.Count / 2), 1);
                newList.InsertRange(0, oldList.GetRange(0, offset));
                MoveBack(newList, oldList.GetRange(offset, oldList.Count - offset));
            }
            else
            {
                newList.InsertRange(0, oldList);
            }
        }

        private void MoveBack(List<Guid> newList, List<Guid> oldList)
        {
            Random random = new Random();
            if (oldList.Count > 2)
            {
                int offset = Math.Max((oldList.Count / 4) + random.Next(oldList.Count / 2), 1);
                newList.AddRange(oldList.GetRange(0, offset));
                MoveForward(newList, oldList.GetRange(offset, oldList.Count - offset));
            }
            else
            {
                newList.AddRange(oldList);
            }
        }
    }
}
