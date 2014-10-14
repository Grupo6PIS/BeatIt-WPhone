using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail5 : Challenge
    {
        public ChallengeDetail5() 
        {
            ChallengeId = 5;
            Name = "Bouncing Game!";
            ColorHex = "#FFe51400";
            Description = "Description 5";
            IsEnabled = false;
            Level = 1;
            MaxAttempt = 3;
        }

        public ChallengeDetail5(int challengeId, string name, string colorHex, int level, int maxAttempts)
        {
            this.ChallengeId = challengeId;
            this.Name = name;
            this.ColorHex = colorHex;
            this.Level = level;
            this.IsEnabled = true;
            this.MaxAttempt = maxAttempts;
        }

        public void ChanllengeComplete( int  collisionCount ){

            var puntaje = collisionCount * 2;

            State.LastScore = puntaje;
            State.BestScore = puntaje > State.BestScore ? puntaje : State.BestScore;
            State.CurrentAttempt++;

            FacadeController.GetInstance().SaveState(State);

        }
    }
}
