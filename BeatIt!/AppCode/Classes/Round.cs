using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
