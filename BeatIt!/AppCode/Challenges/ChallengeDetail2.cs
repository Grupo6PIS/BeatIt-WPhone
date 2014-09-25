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
using BeatIt_.AppCode.Classes;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail2 : Challenge
    {
        public ChallengeDetail2() 
        {
            this.ChallengeId = 2;
            this.Name = "Challenge 2";
            this.ColorHex = "#FF1CA1E3";
            this.Description = "Description 2";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
