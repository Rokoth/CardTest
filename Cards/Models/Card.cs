using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Models
{
    public class Card
    {
        public Guid Id { get; set; }
        public Guid SuitId { get; set; }
        public Guid RangeId { get; set; }        
    }
}
