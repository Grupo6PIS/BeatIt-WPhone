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
using System.Collections;
using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Controllers
{
    public class FacadeController : IFacadeController
    {
        private static FacadeController instance = null;

        private User currentUser;
        private Round currentRound;
        private List<DTRanking> ranking;

        private FacadeController()
        {

        }

        public static FacadeController getInstance()
        {
            if (FacadeController.instance == null)
                FacadeController.instance = new FacadeController();

            return FacadeController.instance;
        }

        public bool isLoggedUser()
        {
            return this.currentUser != null;
        }

        public User getCurrentUser()
        {
            return this.currentUser;
        }

        public List<DTRanking> getRanking() 
        {
            return this.ranking;
        }

        public void loginUser(User user) 
        {
            this.currentUser = user;

            ChallengeDetail1 ch1 = new ChallengeDetail1();
            ChallengeDetail2 ch2 = new ChallengeDetail2();
            ChallengeDetail3 ch3 = new ChallengeDetail3();
            ChallengeDetail4 ch4 = new ChallengeDetail4();
            ChallengeDetail5 ch5 = new ChallengeDetail5();
            ChallengeDetail6 ch6 = new ChallengeDetail6();
            ChallengeDetail7 ch7 = new ChallengeDetail7();
            ChallengeDetail8 ch8 = new ChallengeDetail8();
            ChallengeDetail9 ch9 = new ChallengeDetail9();
            ChallengeDetail10 ch10 = new ChallengeDetail10();

            Round round = new Round();
            round.RoundId = 1;
            round.StartDate = new DateTime(2014, 09, 22, 0, 0, 0);
            round.EndDate = new DateTime(2014, 09, 29, 0, 0, 0);

            Dictionary<int, Challenge> challenges = new Dictionary<int, Challenge>();
            challenges.Add(ch1.ChallengeId, ch1);
            challenges.Add(ch2.ChallengeId, ch2);
            challenges.Add(ch3.ChallengeId, ch3);
            challenges.Add(ch4.ChallengeId, ch4);
            challenges.Add(ch5.ChallengeId, ch5);
            challenges.Add(ch6.ChallengeId, ch6);
            challenges.Add(ch7.ChallengeId, ch7);
            challenges.Add(ch8.ChallengeId, ch8);
            challenges.Add(ch9.ChallengeId, ch9);
            challenges.Add(ch10.ChallengeId, ch10);

            round.Challenges = challenges;

            ch1.Round = round;
            ch2.Round = round;
            ch3.Round = round;
            ch4.Round = round;
            ch5.Round = round;
            ch6.Round = round;
            ch7.Round = round;
            ch8.Round = round;
            ch9.Round = round;
            ch10.Round = round;

            State ch1State = new State();
            State ch2State = new State();
            State ch3State = new State();
            State ch4State = new State();
            State ch5State = new State();
            State ch6State = new State();
            State ch7State = new State();
            State ch8State = new State();
            State ch9State = new State();
            State ch10State = new State();

            ch1.State = ch1State;
            ch1State.setChallenge(ch1);
            ch2.State = ch2State;
            ch2State.setChallenge(ch2);
            ch3.State = ch3State;
            ch3State.setChallenge(ch3);
            ch4.State = ch4State;
            ch4State.setChallenge(ch4);
            ch5.State = ch5State;
            ch5State.setChallenge(ch5);
            ch6.State = ch6State;
            ch6State.setChallenge(ch6);
            ch7.State = ch7State;
            ch7State.setChallenge(ch7);
            ch8.State = ch8State;
            ch8State.setChallenge(ch8);
            ch9.State = ch9State;
            ch9State.setChallenge(ch9);
            ch10.State = ch10State;
            ch10State.setChallenge(ch10);

            this.currentRound = round;

            DTRanking r1 = new DTRanking(this.currentUser.UserId, 1, 280, this.currentUser.FirstName + " " + this.currentUser.LastName, this.currentUser.ImageUrl);
            DTRanking r2 = new DTRanking(2, 2, 127, "Martín Berguer", "http://graph.facebook.com/100002316914037/picture?type=square");
            DTRanking r3 = new DTRanking(3, 3, 106, "Cristian Bauza", "http://graph.facebook.com/cristian.bauza/picture");
            DTRanking r4 = new DTRanking(4, 4, 94, "Pablo Olivera", "http://graph.facebook.com/pablo.olivera/picture");
            DTRanking r5 = new DTRanking(5, 5, 73, "Alejandro Brusco", "http://graph.facebook.com/alejandro.brusco/picture?type=square");
            DTRanking r6 = new DTRanking(6, 6, 22, "Felipe Garcia", "http://graph.facebook.com/felipe92/picture?type=square");
            DTRanking r7 = new DTRanking(7, 7, 15, "Martín Steglich", "http://graph.facebook.com/tinchoste/picture?type=square");

            this.ranking = new List<DTRanking>();
            this.ranking.Add(r1);
            this.ranking.Add(r2);
            this.ranking.Add(r3);
            this.ranking.Add(r4);
            this.ranking.Add(r5);
            this.ranking.Add(r6);
            this.ranking.Add(r7);
        }

        public void logoutUser()
        {
            this.currentUser = null;
            this.currentRound = null;
        }

        public Challenge getChallenge(int challengeId)
        {
            return (Challenge)this.currentRound.Challenges[challengeId];  
        }

        public Dictionary<int, Challenge> getChallenges() 
        {
            return this.currentRound.Challenges;
        }
    }
}
