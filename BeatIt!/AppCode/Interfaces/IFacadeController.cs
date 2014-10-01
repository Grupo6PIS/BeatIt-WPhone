using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Interfaces
{
    interface IFacadeController
    {
        bool isLoggedUser();
        User getCurrentUser();
        List<DTRanking> getRanking();
        void loginUser(User user);
        void logoutUser();
        Challenge getChallenge(int challengeId);
        Dictionary<int, Challenge> getChallenges();
        void setCurrentChallenge(Challenge challenge);
        Challenge getCurrentChallenge();
    }
}
