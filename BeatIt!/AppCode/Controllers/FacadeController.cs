using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Datatypes;
using BeatIt_.AppCode.Interfaces;
using Newtonsoft.Json.Linq;
using SQLite;
using System.IO.IsolatedStorage;

namespace BeatIt_.AppCode.Controllers
{
    public class FacadeController : IFacadeController
    {

        public delegate void CallbackMethod();
        private static FacadeController instance = null;

        private SQLiteConnection db;
        private CallbackMethod saverCallback;
        private User currentUser;
        private Round currentRound;
        private List<DTRanking> ranking;
        private Challenge currentChallenge;
        private WebServicesController ws;

        private bool isForTesting;

        private FacadeController()
        {
            try
            {
                this.db = new SQLiteConnection("BeatItDB.db");
                this.db.CreateTable<DTStatePersistible>();
                this.isForTesting = false;
                ws = new WebServicesController();
            }
            catch (Exception ex)
            {
            }
        }

        public static FacadeController getInstance()
        {
            if (FacadeController.instance == null)
            {
                FacadeController.instance = new FacadeController();                
            }
            
            return FacadeController.instance;
        }

        private FacadeController(bool isForTesting)
        {
            this.isForTesting = isForTesting;
        }

        public bool GetIsForTesting()
        {
            return this.isForTesting;
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

        public void updateRanking(CallbackMethod toCall)
        {
            saverCallback = toCall;
            ws.GetRanking(buildRankingList);
        }

        public void loginUser(User user, JObject jsonResponse) 
        {
            this.currentUser = user;

            // GENERO DESAFIOS DE RONDA
            JObject round = (JObject)jsonResponse["round"],
                    jObjectTemp;
            JArray challengList = (JArray)round["challengeList"];

            Round roundObj = new Round();
            roundObj.RoundId = (int)round["weekNumber"];
            roundObj.StartDate = new DateTime((long)round["start_date"], DateTimeKind.Utc);
            roundObj.EndDate = new DateTime((long)round["end_date"], DateTimeKind.Utc);
            this.currentRound = roundObj;

            Dictionary<int, Challenge> challenges = new Dictionary<int, Challenge>();
            Challenge c;

            for (int i = 0; i < challengList.Count; i++)
            {
                jObjectTemp = (JObject)challengList[i];
                switch ((int)jObjectTemp["id"])
                {
                    case 1: // Usain Bolt
                        c = new ChallengeDetail1((int)jObjectTemp["id"],
                                                 (string)jObjectTemp["challengeName"],
                                                 (int)jObjectTemp["challengeLevel"]);
                        c.Round = roundObj;
                        challenges.Add(1, c);

                        break;
                    case 2: // Wake Me Up!
                        c = new ChallengeDetail2((int)jObjectTemp["id"],
                                                 (string)jObjectTemp["challengeName"],
                                                 (int)jObjectTemp["challengeLevel"]);
                        c.Round = roundObj;
                        challenges.Add(2, c);

                        break;
                    case 3: // Can you play?
                        c = new ChallengeDetail3((int)jObjectTemp["id"],
                                                 (string)jObjectTemp["challengeName"],
                                                 (int)jObjectTemp["challengeLevel"]);
                        c.Round = roundObj;
                        challenges.Add(3, c);

                        break;
                    case 4: // Calla al perro!
                        c = new ChallengeDetail4((int)jObjectTemp["id"],
                                                 (string)jObjectTemp["challengeName"],
                                                 (int)jObjectTemp["challengeLevel"]);
                        c.Round = roundObj;
                        challenges.Add(4, c);

                        break;
                }
            }

            roundObj.Challenges = challenges;

            // GENERO ESTADOS DE DESAFIOS

            List<DTStatePersistible> states = null;
            states = this.db.Query<DTStatePersistible>("select * from DTStatePersistible");

            bool addNewStates = false;

            if (states.Count > 0) // Si hay estados guardados.
            {
                IEnumerator<DTStatePersistible> enumerator = states.GetEnumerator();
                enumerator.MoveNext();
                DTStatePersistible dt = enumerator.Current;
                if (this.currentRound.RoundId == dt.RoundId) // Si los estados guardados corresponden a la ronda actual.
                {
                    foreach (DTStatePersistible aux in states)
                    {
                        State s = new State();
                        s.Challenge = (this.currentRound.Challenges[aux.ChallengeId]);
                        s.CurrentAttempt = aux.CurrentAttempt;
                        s.Finished = aux.Finished;
                        s.LastScore = aux.LastScore;
                        s.BestScore = aux.BestScore;
                        s.StartDate = aux.StartDate;

                        this.currentRound.Challenges[aux.ChallengeId].State = s;
                    }
                }
                else // Si no se corresponden con la ronda actual, los borramos ya que no los necesitamos //????????????? ES ASI?
                {
                    this.db.Query<DTStatePersistible>("delete from DTStatePersistible");
                    addNewStates = true;
                }
            }
            else
                addNewStates = true;

            if (addNewStates)
            {
                foreach (KeyValuePair<int, Challenge> aux in roundObj.Challenges)
                {
                    State s = new State();
                    s.Challenge = aux.Value;
                    aux.Value.State = s;
                    DTStatePersistible dts = s.getDTStatePersistible();
                    this.db.Insert(dts);
                }
            }

            // GENERO LISTA DE RANKINGS

            this.ranking = new List<DTRanking>();
            JArray rankingJson = (JArray)round["ranking"];

            for (int i = 0; i < rankingJson.Count; i++)
            {
                jObjectTemp = (JObject)rankingJson[i];
                this.ranking.Add(new DTRanking(i, i, (int)jObjectTemp["score"], (string)jObjectTemp["name"], (string)jObjectTemp["imageURL"]));
            }
        }

        public void logoutUser()
        {
            this.currentUser = null;
            this.currentRound = null;
            this.db.Query<DTStatePersistible>("delete from DTStatePersistible");

            IsolatedStorageSettings.ApplicationSettings.Remove("IsLoggedUser");
            IsolatedStorageSettings.ApplicationSettings.Remove("Id");
            IsolatedStorageSettings.ApplicationSettings.Remove("FbId");
            IsolatedStorageSettings.ApplicationSettings.Remove("FbAccessToken");
            IsolatedStorageSettings.ApplicationSettings.Remove("FirstName");
            IsolatedStorageSettings.ApplicationSettings.Remove("LastName");
            IsolatedStorageSettings.ApplicationSettings.Remove("Country");
            IsolatedStorageSettings.ApplicationSettings.Remove("BirthDate");
            IsolatedStorageSettings.ApplicationSettings.Remove("ImageUrl");
            IsolatedStorageSettings.ApplicationSettings.Remove("Email");
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public Challenge getChallenge(int challengeId)
        {
            return (Challenge)this.currentRound.Challenges[challengeId];  
        }

        public Dictionary<int, Challenge> getChallenges() 
        {
            return this.currentRound.Challenges;
        }

        public void setCurrentChallenge(Challenge challenge)
        {
            this.currentChallenge = challenge;
        }

        public Challenge getCurrentChallenge()
        {
            return this.currentChallenge;
        }

        public bool saveState(State state)
        {
            try
            {
                DTStatePersistible dts = state.getDTStatePersistible();
                this.db.Update(dts);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void buildRankingList(JObject json)
        {
            if (!(bool) json["error"])
            {
                JArray lastRankings = (JArray)json["data"];
                if (lastRankings.Count > 0)
                {
                    JObject lastRound0 = (JObject) lastRankings[0],
                            jObjectTemp;
                    JArray rankingLastRound0 = (JArray) lastRound0["ranking"];

                    this.ranking.Clear();

                    for (int i = 0; i < rankingLastRound0.Count; i++)
                    {
                        jObjectTemp = (JObject)rankingLastRound0[i];
                        this.ranking.Add(new DTRanking(i, i, (int)jObjectTemp["score"], (string)jObjectTemp["name"], (string)jObjectTemp["imageURL"]));
                    }

                    saverCallback();
                }
            }
        }

        public static FacadeController getInstanceForTesting(User user, DateTime fechaDesdeRonda, DateTime fechaHastaRonda)
        {
            FacadeController.instance = new FacadeController(true);

            FacadeController.instance.currentUser = user;

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
            round.StartDate = fechaDesdeRonda;
            round.EndDate = fechaHastaRonda;
            FacadeController.instance.currentRound = round;

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
            ch1State.Challenge = ch1;
            ch2.State = ch2State;
            ch2State.Challenge = ch2;
            ch3.State = ch3State;
            ch3State.Challenge = ch3;
            ch4.State = ch4State;
            ch4State.Challenge = ch4;
            ch5.State = ch5State;
            ch5State.Challenge = ch5;
            ch6.State = ch6State;
            ch6State.Challenge = ch6;
            ch7.State = ch7State;
            ch7State.Challenge = ch7;
            ch8.State = ch8State;
            ch8State.Challenge = ch8;
            ch9.State = ch9State;
            ch9State.Challenge = ch9;
            ch10.State = ch10State;
            ch10State.Challenge = ch10;

            DTRanking r1 = new DTRanking(FacadeController.instance.currentUser.UserId, 1, 280, FacadeController.instance.currentUser.FirstName + " " + FacadeController.instance.currentUser.LastName, FacadeController.instance.currentUser.ImageUrl);
            DTRanking r2 = new DTRanking(2, 2, 127, "Martín Berguer", "http://graph.facebook.com/100002316914037/picture?type=square");
            DTRanking r3 = new DTRanking(3, 3, 106, "Cristian Bauza", "http://graph.facebook.com/cristian.bauza/picture");
            DTRanking r4 = new DTRanking(4, 4, 94, "Pablo Olivera", "http://graph.facebook.com/pablo.olivera/picture");
            DTRanking r5 = new DTRanking(5, 5, 73, "Alejandro Brusco", "http://graph.facebook.com/alejandro.brusco/picture?type=square");
            DTRanking r6 = new DTRanking(6, 6, 22, "Felipe Garcia", "http://graph.facebook.com/felipe92/picture?type=square");
            DTRanking r7 = new DTRanking(7, 7, 15, "Martín Steglich", "http://graph.facebook.com/tinchoste/picture?type=square");

            FacadeController.instance.ranking = new List<DTRanking>();
            FacadeController.instance.ranking.Add(r1);
            FacadeController.instance.ranking.Add(r2);
            FacadeController.instance.ranking.Add(r3);
            FacadeController.instance.ranking.Add(r4);
            FacadeController.instance.ranking.Add(r5);
            FacadeController.instance.ranking.Add(r6);
            FacadeController.instance.ranking.Add(r7);



            return FacadeController.instance;
        }
    }
}
