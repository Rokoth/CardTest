using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Models
{
    public class RangeSet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Dictionary<Guid, int> Ranges { get; set; }
    }
}
