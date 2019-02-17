using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.DTO
{
    public class PackView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<CardView> Cards { get; set; }
        public Dictionary<int, CardView> SortedCards { get; set; }
        public SuitRangeView SuitRange { get; set; }
    }
}
