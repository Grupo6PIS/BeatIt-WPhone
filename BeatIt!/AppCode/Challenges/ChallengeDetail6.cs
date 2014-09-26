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
    public class ChallengeDetail6 : Challenge
    {
        public ChallengeDetail6() 
        {
            this.ChallengeId = 6;
            this.Name = "Challenge 6";
            this.ColorHex = "#FFE61300";
            this.Description = "Description 6";
            this.IsEnabled = false;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
