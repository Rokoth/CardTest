using Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.DTO
{
    public class SuitSetView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Dictionary<Suit, int> Suits { get; set; }
    }
}
