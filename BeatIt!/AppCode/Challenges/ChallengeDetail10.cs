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
    public class ChallengeDetail10 : Challenge
    {
        public ChallengeDetail10() 
        {
            this.ChallengeId = 10;
            this.Name = "Challenge 10";
            this.ColorHex = "#FFE3C900";
            this.Description = "Description 10";
            this.IsEnabled = false;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
