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
    public class ChallengeDetail8 : Challenge
    {
        public ChallengeDetail8() 
        {
            this.ChallengeId = 8;
            this.Name = "Challenge 8";
            this.ColorHex = "#FF008A02";
            this.Description = "Description 8";
            this.IsEnabled = false;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
