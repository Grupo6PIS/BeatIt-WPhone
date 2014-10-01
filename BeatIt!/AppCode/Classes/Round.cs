using System;
using System.Collections.Generic;

namespace BeatIt_.AppCode.Classes
{
    public class Round
    {
        private int roundId;
        private DateTime startDate;
        private DateTime endDate;
        private Dictionary<int, Challenge> challenges;

        public Round()
        {
           
        }

        public int RoundId
        {
            get { return roundId; }
            set { roundId = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public Dictionary<int, Challenge> Challenges
        {
            get { return challenges; }
            set { challenges = value; }
        }

    }
}
