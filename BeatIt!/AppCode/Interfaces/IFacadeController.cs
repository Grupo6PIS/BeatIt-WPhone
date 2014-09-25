using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
