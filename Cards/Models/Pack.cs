using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Models
{
    public class Pack
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
        public Dictionary<int, Guid> SortedCards { get; set; }
        public Guid SuitRangeId { get; set; }
    }
}
