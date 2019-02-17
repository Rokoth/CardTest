using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Models
{
    public class SuitRange
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SuitSetId { get; set; }
        public Guid RangeSetId { get; set; }
    }
}
