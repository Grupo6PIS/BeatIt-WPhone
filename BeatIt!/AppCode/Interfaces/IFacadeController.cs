using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Datatypes;
using Newtonsoft.Json.Linq;

namespace BeatIt_.AppCode.Interfaces
{
    interface IFacadeController
    {
        bool isLoggedUser();
        User getCurrentUser();
        List<DTRanking> getRanking();
        void loginUser(User user, JObject jsonResponse);
        void logoutUser();
        void updateRanking(JObject jsonResponse);
        Challenge getChallenge(int challengeId);
        Dictionary<int, Challenge> getChallenges();
        void setCurrentChallenge(Challenge challenge);
        Challenge getCurrentChallenge();
        int GetRoundScore();
        bool ShouldSendScore();
    }
}
