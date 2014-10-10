using System;
using System.Collections.Generic;

namespace BeatIt_.AppCode.Classes
{
    public class Round
    {
        public int RoundId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<int, Challenge> Challenges { get; set; }
    }
}
