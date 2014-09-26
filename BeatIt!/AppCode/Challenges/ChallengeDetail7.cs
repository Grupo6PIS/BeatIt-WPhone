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
    public class ChallengeDetail7 : Challenge
    {
        public ChallengeDetail7() 
        {
            this.ChallengeId = 7;
            this.Name = "Challenge 7";
            this.ColorHex = "#FFFA6800";
            this.Description = "Description 7";
            this.IsEnabled = false;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
