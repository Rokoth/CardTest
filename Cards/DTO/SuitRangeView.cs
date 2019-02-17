using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.DTO
{
    public class SuitRangeView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SuitSetView SuitSet { get; set; }
        public RangeSetView RangeSet { get; set; }
    }
}
