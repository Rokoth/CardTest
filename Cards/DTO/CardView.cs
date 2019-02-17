using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.DTO
{
    public class CardView
    {
        public Guid Id { get; set; }
        public Suit Suit { get; set; }
        public Range Range { get; set; }        
    }
}
