using BeatIt_.AppCode.Classes;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail3 : Challenge
    {
        public ChallengeDetail3() 
        {
            this.ChallengeId = 3;
            this.Name = "Invite Friends";
            this.ColorHex = "#FF3B5998";
            this.Description = "You have to invite friends using SMS of Facebook";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }
    }
}
