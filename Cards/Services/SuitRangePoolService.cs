using Cards.DTO;
using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Services
{
    public class SuitRangePoolService
    {
        private List<SuitRange> SuitRangePool;
        private List<SuitSet> SuitSetPool = new List<SuitSet>();
        private List<RangeSet> RangeSetPool = new List<RangeSet>();
        private List<Suit> Suits = new List<Suit>();
        private List<Range> Ranges = new List<Range>();

        public SuitRangePoolService(IServiceProvider serviceProvider)
        {
            SuitRangePool = new List<SuitRange>();
            Dictionary<Guid, int> suitSet = new Dictionary<Guid, int>();
            Dictionary<Guid, int> range36Set = new Dictionary<Guid, int>();
            Dictionary<Guid, int> range52Set = new Dictionary<Guid, int>();

            foreach (var name in new string[] { "Hearts", "Diamonds", "Clubs", "Spades" })
            {
                suitSet.Add(CreateSuit(name), 1);
            }
            int i = 0, y = 0;
            foreach (var name in new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" })
            {
                range52Set.Add(CreateRange(name), i++);
                if(i >= 5) range36Set.Add(CreateRange(name), y++);                
            }
            var suitSetId = CreateSuitSet("DefaultSuitSet", suitSet);
            var range36SetId = CreateRangeSet("DefaultRange36Set", range36Set);
            var range52SetId = CreateRangeSet("DefaultRange52Set", range52Set);
            AddToSuitRangePool("DefaultSuitRange36Set", suitSetId, range36SetId);
            AddToSuitRangePool("DefaultSuitRange52Set", suitSetId, range52SetId);
        }

        public async Task<Guid> CreateSuitAsync(string name)
        {
            return await Task.Run(() =>
            {
                return CreateSuit(name);
            });
        }
        
        public async Task<Guid> CreateRangeAsync(string name)
        {
            return await Task.Run(() =>
            {
                return CreateRange(name);
            });
        }
        
        public async Task<Guid> CreateSuitSetAsync(string name, Dictionary<Guid, int> suitRange)
        {
            return await Task.Run(() =>
            {
                return CreateSuitSet(name, suitRange);
            });
        }
        
        public async Task<Guid> CreateRangeSetAsync(string name, Dictionary<Guid, int> rangeRange)
        {
            return await Task.Run(() =>
            {
                return CreateRangeSet(name, rangeRange);
            });
        }
        
        public async Task<Guid> AddToSuitRangePoolAsync(string name, Guid suitSetId, Guid rangeSetId)
        {
            return await Task.Run(() =>
            {
                return AddToSuitRangePool(name, suitSetId, rangeSetId);
            });
        }

        public SuitRange GetSuitRange(Guid id)
        {
            return SuitRangePool.Find(s=>s.Id == id);
        }

        public SuitSet GetSuitSet(Guid id)
        {
            return SuitSetPool.Find(s => s.Id == id);
        }

        public RangeSet GetRangeSet(Guid id)
        {
            return RangeSetPool.Find(s => s.Id == id);
        }

        public Suit GetSuit(Guid id)
        {
            return Suits.Find(s => s.Id == id);
        }

        public Range GetRange(Guid id)
        {
            return Ranges.Find(s => s.Id == id);
        }

        public List<Suit> GetSuits()
        {
            return Suits;
        }

        public List<SuitRange> GetSuitRanges()
        {
            return SuitRangePool;
        }

        public List<Range> GetRanges()
        {
            return Ranges;
        }

        public List<SuitSet> GetSuitSets()
        {
            return SuitSetPool;
        }
        
        public List<RangeSet> GetRangeSets()
        {
            return RangeSetPool;
        }

        public CardView ToDto(Card card)
        {
            return new CardView()
            {
                Id = card.Id,
                Range = Ranges.Find(s=>s.Id == card.RangeId),
                Suit = Suits.Find(s => s.Id == card.SuitId)
            };
        }

        public RangeSetView ToDto(RangeSet rangeSet)
        {
            return new RangeSetView()
            {
                Id = rangeSet.Id,
                Name = rangeSet.Name,
                Ranges = rangeSet.Ranges.ToDictionary(s=>Ranges.Find(c=>c.Id==s.Key), s=>s.Value)
            };
        }

        public SuitSetView ToDto(SuitSet suitSet)
        {
            return new SuitSetView()
            {
                Id = suitSet.Id,
                Name = suitSet.Name,
                Suits = suitSet.Suits.ToDictionary(s => Suits.Find(c => c.Id == s.Key), s => s.Value)
            };
        }

        public SuitRangeView ToDto(SuitRange suitRange)
        {
           var ret= new SuitRangeView()
            {
                Id = suitRange.Id,
                Name = suitRange.Name,
                RangeSet = ToDto(RangeSetPool.Find(s=>s.Id == suitRange.RangeSetId)),
                SuitSet = ToDto(SuitSetPool.Find(s => s.Id == suitRange.SuitSetId))
            };
            return ret;
        }

        public PackView ToDto(Pack pack)
        {            
            var result =  new PackView()
            {
                Id = pack.Id,
                Name = pack.Name,
                Cards = pack.Cards.Select(s=>ToDto(s)).ToList(),
                SuitRange = ToDto(SuitRangePool.Find(s=>s.Id == pack.SuitRangeId))
            };
            result.SortedCards = pack.SortedCards.ToDictionary(s => s.Key, s => result.Cards.Find(c => c.Id == s.Value));
            return result;
        }

        private Guid AddToSuitRangePool(string name, Guid suitSetId, Guid rangeSetId)
        {
            lock (SuitRangePool)
            {
                var id = Guid.NewGuid();
                SuitRangePool.Add(new SuitRange()
                {
                    Id = id,
                    Name = name,
                    RangeSetId = rangeSetId,
                    SuitSetId = suitSetId
                });
                return id;
            }
        }

        private Guid CreateSuit(string name)
        {
            lock (Suits)
            {
                var id = Suits.Find(s => s.Name.Equals(name, StringComparison.CurrentCulture))?.Id;
                if (!id.HasValue)
                {
                    id = Guid.NewGuid();
                    Suits.Add(new Suit() { Id = id.Value, Name = name });
                }
                return id.Value;
            }
        }

        private Guid CreateRange(string name)
        {
            lock (Ranges)
            {
                var id = Ranges.Find(s => s.Name.Equals(name, StringComparison.CurrentCulture))?.Id;
                if (!id.HasValue)
                {
                    id = Guid.NewGuid();
                    Ranges.Add(new Range() { Id = id.Value, Name = name });
                }
                return id.Value;
            }
        }

        private Guid CreateSuitSet(string name, Dictionary<Guid, int> suitRange)
        {
            lock (SuitSetPool)
            {
                var id = Guid.NewGuid();
                SuitSetPool.Add(new SuitSet() { Id = id, Name = name, Suits = suitRange });
                return id;
            }
        }

        private Guid CreateRangeSet(string name, Dictionary<Guid, int> rangeRange)
        {
            lock (RangeSetPool)
            {
                var id = Guid.NewGuid();
                RangeSetPool.Add(new RangeSet() { Id = id, Name = name, Ranges = rangeRange });
                return id;
            }
        }
    }
}
