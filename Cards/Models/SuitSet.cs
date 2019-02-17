using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Models
{
    public class SuitSet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Dictionary<Guid, int> Suits { get; set; }
    }
}
