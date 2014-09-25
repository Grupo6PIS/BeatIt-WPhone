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
    public class ChallengeDetail3 : Challenge
    {
        public ChallengeDetail3() 
        {
            this.ChallengeId = 3;
            this.Name = "Challenge 3";
            this.ColorHex = "#FFFA6800";
            this.Description = "Description 3";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
