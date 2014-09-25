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
    public class ChallengeDetail4 : Challenge
    {
        public ChallengeDetail4() 
        {
            this.ChallengeId = 4;
            this.Name = "Challenge 4";
            this.ColorHex = "#FFE3C900";
            this.Description = "Description 4";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
