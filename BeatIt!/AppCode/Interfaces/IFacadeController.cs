using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Datatypes;
using Newtonsoft.Json.Linq;

namespace BeatIt_.AppCode.Interfaces
{
    interface IFacadeController
    {
        bool IsLoggedUser();
        User GetCurrentUser();
        List<DTRanking> GetRanking();
        void LoginUser(User user, JObject jsonResponse, JObject serverDataUser);
        void LogoutUser();
        void UpdateRanking(JObject jsonResponse);
        Challenge GetChallenge(int challengeId);
        Dictionary<int, Challenge> GetChallenges();
        void SetCurrentChallenge(Challenge challenge);
        Challenge GetCurrentChallenge();
        int GetRoundScore();
        bool ShouldSendScore();
    }
}
