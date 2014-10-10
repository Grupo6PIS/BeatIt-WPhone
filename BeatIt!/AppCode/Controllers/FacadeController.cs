using System;
using System.Collections.Generic;
using System.Globalization;
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
        //public delegate void CallbackMethod();

        private static FacadeController _instance;

        private readonly SQLiteConnection _db;
        private readonly bool _isForTesting;
        private Challenge _currentChallenge;
        private Round _currentRound;
        private User _currentUser;
        private List<DTRanking> _ranking;
        private bool _hayCambiosParaEnviar;

        private FacadeController()
        {
            _hayCambiosParaEnviar = false;
            _db = new SQLiteConnection("BeatItDB.db");
            _db.CreateTable<DTStatePersistible>();
            _isForTesting = false;
        }

        private FacadeController(bool isForTesting)
        {
            _isForTesting = isForTesting;
        }

        public bool isLoggedUser()
        {
            return _currentUser != null;
        }

        public User getCurrentUser()
        {
            return _currentUser;
        }

        public List<DTRanking> getRanking()
        {
            return _ranking;
        }

        public void loginUser(User user, JObject jsonResponse)
        {
            _currentUser = user;

            // GENERO DESAFIOS DE RONDA
            JObject round = (JObject) jsonResponse["round"],
                jObjectTemp;
            var challengList = (JArray) round["challengeList"];

            var roundObj = new Round
            {
                RoundId = (int) round["weekNumber"],
                StartDate = new DateTime((long) round["start_date"], DateTimeKind.Utc),
                EndDate = new DateTime((long) round["end_date"], DateTimeKind.Utc)
            };
            _currentRound = roundObj;

            var challenges = new Dictionary<int, Challenge>();

            foreach (var t in challengList)
            {
                jObjectTemp = (JObject) t;
                Challenge c;
                switch ((int) jObjectTemp["id"])
                {
                    case 1: // Usain Bolt
                        c = new ChallengeDetail1((int) jObjectTemp["id"],
                            (string) jObjectTemp["challengeName"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"]) {Round = roundObj};
                        challenges.Add(1, c);

                        break;
                    case 2: // Wake Me Up!
                        c = new ChallengeDetail2((int) jObjectTemp["id"],
                            (string) jObjectTemp["challengeName"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"]) {Round = roundObj};
                        challenges.Add(2, c);

                        break;
                    case 3: // Can you play?
                        c = new ChallengeDetail3((int) jObjectTemp["id"],
                            (string) jObjectTemp["challengeName"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"]) {Round = roundObj};
                        challenges.Add(3, c);

                        break;
                    case 4: // Calla al perro!
                        c = new ChallengeDetail4((int) jObjectTemp["id"],
                            (string) jObjectTemp["challengeName"],
                            (string) jObjectTemp["colorHex"],
                            (int) jObjectTemp["challengeLevel"],
                            (int) jObjectTemp["maxAttemps"]) {Round = roundObj};
                        challenges.Add(4, c);

                        break;
                }
            }

            roundObj.Challenges = challenges;

            // GENERO ESTADOS DE DESAFIOS

            var states = _db.Query<DTStatePersistible>("select * from DTStatePersistible");

            var addNewStates = false;

            if (states.Count > 0) // Si hay estados guardados.
            {
                IEnumerator<DTStatePersistible> enumerator = states.GetEnumerator();
                enumerator.MoveNext();
                var dt = enumerator.Current;
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
                    // Si no se corresponden con la ronda actual, los borramos ya que no los necesitamos //????????????? ES ASI?
                {
                    _db.Query<DTStatePersistible>("delete from DTStatePersistible");
                    addNewStates = true;
                }
            }
            else
                addNewStates = true;

            if (addNewStates)
            {
                foreach (var aux in roundObj.Challenges)
                {
                    var s = new State {Challenge = aux.Value};
                    aux.Value.State = s;
                    var dts = s.GetDtStatePersistible();
                    _db.Insert(dts);
                }
            }

            // GENERO LISTA DE RANKINGS

            _ranking = new List<DTRanking>();
            var rankingJson = (JArray) round["ranking"];

            for (var i = 0; i < rankingJson.Count; i++)
            {
                jObjectTemp = (JObject) rankingJson[i];
                _ranking.Add(new DTRanking(i, i + 1, (int) jObjectTemp["score"], (string) jObjectTemp["userName"],
                    (string) jObjectTemp["imageURL"]));
            }
        }

        public void logoutUser()
        {
            _currentUser = null;
            _currentRound = null;
            _db.Query<DTStatePersistible>("delete from DTStatePersistible");

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
            return _currentRound.Challenges[challengeId];
        }

        public Dictionary<int, Challenge> getChallenges()
        {
            return _currentRound.Challenges;
        }

        public void setCurrentChallenge(Challenge challenge)
        {
            _currentChallenge = challenge;
        }

        public Challenge getCurrentChallenge()
        {
            return _currentChallenge;
        }


        public void updateRanking(JObject jsonResponse)
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
                    _ranking.Add(new DTRanking(i, i + 1, (int) jObjectTemp["score"], (string) jObjectTemp["userName"],
                        (string) jObjectTemp["imageURL"]));
                }
            }
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
            var dts = state.GetDtStatePersistible();
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
                {ch10.ChallengeId, ch10}
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
                _instance._currentUser.FirstName + " " + _instance._currentUser.LastName,
                _instance._currentUser.ImageUrl);
            var r2 = new DTRanking(2, 2, 127, "Martín Berguer",
                "http://graph.facebook.com/100002316914037/picture?type=square");
            var r3 = new DTRanking(3, 3, 106, "Cristian Bauza", "http://graph.facebook.com/cristian.bauza/picture");
            var r4 = new DTRanking(4, 4, 94, "Pablo Olivera", "http://graph.facebook.com/pablo.olivera/picture");
            var r5 = new DTRanking(5, 5, 73, "Alejandro Brusco",
                "http://graph.facebook.com/alejandro.brusco/picture?type=square");
            var r6 = new DTRanking(6, 6, 22, "Felipe Garcia", "http://graph.facebook.com/felipe92/picture?type=square");
            var r7 = new DTRanking(7, 7, 15, "Martín Steglich",
                "http://graph.facebook.com/tinchoste/picture?type=square");

            _instance._ranking = new List<DTRanking> {r1, r2, r3, r4, r5, r6, r7};

            return _instance;
        }

        public void SetHayCambiosParaEnviar()
        {
            _hayCambiosParaEnviar = true;
        }

        public void UploadPuntaje(WebServicesController.CallbackWebService callback)
        {
            // Primero Si hay cambios para enviar vemos si hay cambios para enviar, en caso positivo
            // calculamos el puntaje total y lo enviamos.
            if (_hayCambiosParaEnviar)
            {
                int totalPuntos = _currentRound.Challenges.Where(variable => variable.Value.IsEnabled).Sum(variable => variable.Value.State.BestScore);
                var wsController = new WebServicesController();
                wsController.SendScore(_currentUser.UserId.ToString(CultureInfo.InvariantCulture), totalPuntos, callback);
            }
            else
            {
                const string errorStr = "{'error':true}";
                callback(JObject.Parse(errorStr));
            }
        }
    }
}