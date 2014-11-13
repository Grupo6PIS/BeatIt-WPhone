using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Datatypes;
using BeatIt_.AppCode.Interfaces;
using Newtonsoft.Json.Linq;
using SQLite;

namespace BeatIt_.AppCode.Controllers
{
    public class FacadeController : IFacadeController
    {
        private static FacadeController _instance;

        private readonly SQLiteConnection _db;
        private readonly bool _isForTesting;
        private Challenge _currentChallenge;
        private Round _currentRound;
        private User _currentUser;
        private List<DTRanking> _ranking;

        private FacadeController()
        {
            _db = new SQLiteConnection("BeatItDB.db");
            _db.CreateTable<DTStatePersistible>();
            _isForTesting = false;
        }

        private FacadeController(bool isForTesting)
        {
            _isForTesting = isForTesting;
        }

        public bool IsLoggedUser()
        {
            return _currentUser != null;
        }

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        public List<DTRanking> GetRanking()
        {
            return _ranking;
        }

        public void LoginUser(User user, JObject roundJsonResponse, JObject userJsonResponse)
        {
            _currentUser = user;

            // GENERO DESAFIOS DE RONDA
            JObject round = (JObject) roundJsonResponse["round"],
                jObjectTemp;
            var challengList = (JArray) round["challengeList"];

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            var roundObj = new Round
            {
                RoundId = (int) round["weekNumber"],
                StartDate = unixEpoch.AddMilliseconds((long) round["start_date"]),
                EndDate = unixEpoch.AddMilliseconds((long) round["end_date"])
            };

            _currentRound = roundObj;

            var challenges = new Dictionary<int, Challenge>();

            foreach (JToken t in challengList)
            {
                jObjectTemp = (JObject) t;
                Challenge c;
                switch ((int) jObjectTemp["_id"])
                {
                    case 1: // Usain Bolt
                        c = new ChallengeDetail1((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(1, c);

                        break;
                    case 2: // Wake Me Up!
                        c = new ChallengeDetail2((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(2, c);

                        break;
                    case 3: // Can you play?
                        c = new ChallengeDetail3((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(3, c);

                        break;
                    case 4: // Shut the Dog!
                        c = new ChallengeDetail4((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(4, c);

                        break;
                    case 5: // Bouncing Game
                        c = new ChallengeDetail5((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(5, c);
                        break;

                    case 6: // Throw the Phone
                        c = new ChallengeDetail6((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(6, c);
                        break;

                    case 7: // Catch Me!
                        c = new ChallengeDetail7((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(7, c);
                        break;

                    case 8: // Color & Text
                        c = new ChallengeDetail8((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(8, c);
                        break;

                    case 9: // Song Complete
                        c = new ChallengeDetail9((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(9, c);
                        break;
                    case 10: // Selfie Group
                        c = new ChallengeDetail10((int) jObjectTemp["_id"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"],
                            (bool) jObjectTemp["active"]) {Round = roundObj};
                        challenges.Add(10, c);
                        break;
                }
            }

            roundObj.Challenges = challenges;

            // GENERO ESTADOS DE DESAFIOS
            bool addNewStates = false;

            if (userJsonResponse["roundStates"] != null)
            {
                JToken roundState = userJsonResponse["roundStates"];
                if (roundState["challenges"] != null)
                {
                    var challengesServer = (JArray)roundState["challenges"];
                    if (challengesServer.Count > 0) // Si hay estados guardados.
                    {
                        if (_currentRound.RoundId == (int)((JObject)userJsonResponse["roundStates"])["id"])
                        {
                            foreach (JToken jToken in challengesServer)
                            {
                                var jObject = (JObject)jToken;
                                var s = new State
                                {
                                    Challenge = (_currentRound.Challenges[(int)jObject["id"]]),
                                    CurrentAttempt = (int)jObject["attemps"],
                                    Finished = (bool)jObject["finished"],
                                    LastScore = (int)jObject["lastScore"],
                                    BestScore = (int)jObject["bestScore"],
                                    StartDate = Convert.ToDateTime((string)jObject["start_date"])
                                };

                                _currentRound.Challenges[(int)jObject["id"]].State = s;
                                if (!SaveState(s))
                                {
                                    _db.Insert(s.GetDtStatePersistible());
                                }
                            }
                        }
                        else
                        {
                            addNewStates = true;
                        }
                    }
                    else
                        addNewStates = true;
                }
                else
                    addNewStates = true;
            }
            else
            {
                List<DTStatePersistible> states = _db.Query<DTStatePersistible>("select * from DTStatePersistible");

                if (states.Count > 0) // Si hay estados guardados.
                {
                    IEnumerator<DTStatePersistible> enumerator = states.GetEnumerator();
                    enumerator.MoveNext();
                    DTStatePersistible dt = enumerator.Current;
                    if (_currentRound.RoundId == dt.RoundId) // Si los estados guardados corresponden a la ronda actual.
                    {
                        foreach (DTStatePersistible aux in states)
                        {
                            var s = new State
                            {
                                Challenge = (_currentRound.Challenges[aux.ChallengeId]),
                                CurrentAttempt = aux.CurrentAttempt,
                                Finished = aux.Finished,
                                LastScore = aux.LastScore,
                                BestScore = aux.BestScore,
                                StartDate = aux.StartDate
                            };

                            _currentRound.Challenges[aux.ChallengeId].State = s;
                        }
                    }
                    else
                        // Si no se corresponden con la ronda actual, los borramos ya que no los necesitamos
                    {
                        _db.Query<DTStatePersistible>("delete from DTStatePersistible");
                        addNewStates = true;
                    }
                }
                else
                    addNewStates = true;
            }

            if (addNewStates)
            {
                foreach (var aux in roundObj.Challenges)
                {
                    var s = new State {Challenge = aux.Value};
                    aux.Value.State = s;
                    DTStatePersistible dts = s.GetDtStatePersistible();
                    _db.Insert(dts);
                }
            }

            // GENERO LISTA DE RANKINGS

            _ranking = new List<DTRanking>();
            var rankingJson = (JArray) round["ranking"];

            for (int i = 0; i < rankingJson.Count; i++)
            {
                jObjectTemp = (JObject) rankingJson[i];
                _ranking.Add(new DTRanking((string) jObjectTemp["id"], i + 1, (int) jObjectTemp["score"],
                    (string) jObjectTemp["userName"],
                    (string) jObjectTemp["imageURL"]));
            }
        }

        public void LogoutUser()
        {
            //Save state instances on server
            if (_currentRound != null && _currentUser != null)
            {
                int index = 0;
                string jsonString = "{\"roundId\":" + _currentRound.RoundId +
                                    ", \"userId\":\"" + _currentUser.UserId + "\"" +
                                    ", \"challenges\": [";

                var ws = new WebServicesController();

                foreach (var c in _currentRound.Challenges)
                {
                    Challenge challengeTemp = c.Value;
                    State stateTemp = challengeTemp.State;

                    jsonString += "{\"challengeId\":" + challengeTemp.ChallengeId +
                                  ", \"attemps\":" + stateTemp.CurrentAttempt +
                                  ", \"finished\": " + (stateTemp.Finished ? "true" : "false") +
                                  ", \"start_date\": \"" + stateTemp.StartDate + "\"" +
                                  ", \"bestScore\" : " + stateTemp.BestScore +
                                  ", \"lastScore\": " + stateTemp.LastScore +
                                  (index < _currentRound.Challenges.Count - 1 ? "}," : "}");

                    index++;
                }

                jsonString += "]}";

                ws.SendAllStates(jsonString);
            }

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
            IsolatedStorageSettings.ApplicationSettings.Remove("LastScoreSent");
            IsolatedStorageSettings.ApplicationSettings.Save();

            _currentUser = null;
            _currentRound = null;

            _db.Query<DTStatePersistible>("delete from DTStatePersistible");
        }

        public Challenge GetChallenge(int challengeId)
        {
            return _currentRound.Challenges[challengeId];
        }

        public Dictionary<int, Challenge> GetChallenges()
        {
            return _currentRound.Challenges;
        }

        public void SetCurrentChallenge(Challenge challenge)
        {
            _currentChallenge = challenge;
        }

        public Challenge GetCurrentChallenge()
        {
            return _currentChallenge;
        }


        public void UpdateRanking(JObject jsonResponse)
        {
            var lastRankings = (JArray) jsonResponse["data"];
            if (lastRankings.Count > 0)
            {
                var lastRound0 = (JObject) lastRankings[0];
                var rankingLastRound0 = (JArray) lastRound0["ranking"];

                _ranking.Clear();

                for (int i = 0; i < rankingLastRound0.Count; i++)
                {
                    var jObjectTemp = (JObject) rankingLastRound0[i];
                    _ranking.Add(new DTRanking((string) jObjectTemp["id"], i + 1, (int) jObjectTemp["score"],
                        (string) jObjectTemp["userName"],
                        (string) jObjectTemp["imageURL"]));
                }
            }
        }

        public int GetRoundScore()
        {
            return
                _currentRound.Challenges.Where(variable => variable.Value.IsEnabled)
                    .Sum(variable => variable.Value.State.BestScore);
        }

        public bool ShouldSendScore()
        {
            var lastScoreSent = (int) IsolatedStorageSettings.ApplicationSettings["LastScoreSent"];
            return
                _currentRound.Challenges.Values.All(x => (x.IsEnabled && x.State.CurrentAttempt > 0) || !x.IsEnabled) &&
                GetRoundScore() > lastScoreSent;
        }

        public static FacadeController GetInstance()
        {
            return _instance ?? (_instance = new FacadeController());
        }

        public bool GetIsForTesting()
        {
            return _isForTesting;
        }

        public bool SaveState(State state)
        {
            DTStatePersistible dts = state.GetDtStatePersistible();
            int rowsAffected = _db.Update(dts);
            return rowsAffected > 0;
        }

        public static FacadeController GetInstanceForTesting(User user, DateTime fechaDesdeRonda,
            DateTime fechaHastaRonda)
        {
            _instance = new FacadeController(true) {_currentUser = user};

            var ch1 = new ChallengeDetail1();
            var ch2 = new ChallengeDetail2();
            var ch3 = new ChallengeDetail3();
            var ch4 = new ChallengeDetail4();
            var ch5 = new ChallengeDetail5();
            var ch6 = new ChallengeDetail6();
            var ch7 = new ChallengeDetail7();
            var ch8 = new ChallengeDetail8();
            var ch9 = new ChallengeDetail9();
            var ch10 = new ChallengeDetail10();

            var round = new Round {RoundId = 1, StartDate = fechaDesdeRonda, EndDate = fechaHastaRonda};
            _instance._currentRound = round;

            var challenges = new Dictionary<int, Challenge>
            {
                {ch1.ChallengeId, ch1},
                {ch2.ChallengeId, ch2},
                {ch3.ChallengeId, ch3},
                {ch4.ChallengeId, ch4},
                {ch5.ChallengeId, ch5},
                {ch6.ChallengeId, ch6},
                {ch7.ChallengeId, ch7},
                {ch8.ChallengeId, ch8},
                {ch9.ChallengeId, ch9},
                {ch10.ChallengeId, ch10},
            };

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

            var ch1State = new State();
            var ch2State = new State();
            var ch3State = new State();
            var ch4State = new State();
            var ch5State = new State();
            var ch6State = new State();
            var ch7State = new State();
            var ch8State = new State();
            var ch9State = new State();
            var ch10State = new State();


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

            var r1 = new DTRanking(_instance._currentUser.UserId, 1, 280,
                _instance._currentUser.Name,
                _instance._currentUser.ImageUrl);
            var r2 = new DTRanking("2", 2, 127, "Martín Berguer",
                "http://graph.facebook.com/100002316914037/picture?type=square");
            var r3 = new DTRanking("3", 3, 106, "Cristian Bauza", "http://graph.facebook.com/cristian.bauza/picture");
            var r4 = new DTRanking("4", 4, 94, "Pablo Olivera", "http://graph.facebook.com/pablo.olivera/picture");
            var r5 = new DTRanking("5", 5, 73, "Alejandro Brusco",
                "http://graph.facebook.com/alejandro.brusco/picture?type=square");
            var r6 = new DTRanking("6", 6, 22, "Felipe Garcia", "http://graph.facebook.com/felipe92/picture?type=square");
            var r7 = new DTRanking("7", 7, 15, "Martín Steglich",
                "http://graph.facebook.com/tinchoste/picture?type=square");

            _instance._ranking = new List<DTRanking> {r1, r2, r3, r4, r5, r6, r7};

            return _instance;
        }
    }
}