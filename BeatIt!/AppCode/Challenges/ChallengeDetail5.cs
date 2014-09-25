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
    public class ChallengeDetail5 : Challenge
    {
        public ChallengeDetail5() 
        {
            this.ChallengeId = 5;
            this.Name = "Challenge 5";
            this.ColorHex = "#FFAA00FF";
            this.Description = "Description 5";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
