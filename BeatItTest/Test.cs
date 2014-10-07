using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Datatypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeatIt.Tests
{
    [TestClass] // Logica Usain Bolt
    public class TestingChallengeDetail1
    {
        [DeploymentItem("BeatItDB.db")]
        [TestMethod]
        public void TestingFunctioncalculatePuntaje()
        {
            Assert.AreEqual(4, 2 + 2);

            // Por algun motivo no puede el sqlite no puede crear la base 
            // o mas bien crear un archivo, me parece que tiene algo que ver
            // con el contexto en el que se ejecuta el test.
            // Por eso gener un metodo a parte para generar la instancia del controlador, 
            // en este metodos esta todo hardcodeado el tema de la carga de los estados que era lo
            // que estabamos cargando de SQLite.
            var ifc = FacadeController.getInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));

            var currentChallenge = (ChallengeDetail1)ifc.getChallenge(1);
            currentChallenge.completeChallenge(false, 45, 42);

            //Si esta bien debería ser 174 el puntaje de en el desafío (42+45)*2
            Assert.AreEqual(174, currentChallenge.State.LastScore);
        }

    };

    [TestClass] // Logica Wake Me Up!
    public class TestingChallengeDetail2
    {
        [TestMethod]
        public void TestingFunction_GetSecondsToWakeMeUp()
        {
            ChallengeDetail2 ch = new ChallengeDetail2();
            ch.Level = 1;
            int[] secondsToWakeMeUp = ch.getSecondsToWakeMeUp();
            Assert.AreEqual(secondsToWakeMeUp[0], 3);

            ch.Level = 2;
            secondsToWakeMeUp = ch.getSecondsToWakeMeUp();
            Assert.AreEqual(secondsToWakeMeUp[3], 9);

            ch = new ChallengeDetail2();
            ch.State = new State();

        }

        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
            FacadeController cont = FacadeController.getInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));

            ChallengeDetail2 challenge = (ChallengeDetail2)cont.getChallenge(2);
            challenge.Level = 1;

            challenge.CompleteChallenge(1);
            // El ultimo puntaje y el mejor deberian ser 20.
            Assert.AreEqual(challenge.State.BestScore, 20);
            Assert.AreEqual(challenge.State.LastScore, 20);

            challenge.CompleteChallenge(2);
            // El ultimo puntaje y el mejor deberian ser 40
            Assert.AreEqual(challenge.State.BestScore, 40);
            Assert.AreEqual(challenge.State.LastScore, 40);

            challenge.CompleteChallenge(0);
            // El ultimo puntaje deberia ser 0 y el mejor 40.
            Assert.AreEqual(challenge.State.BestScore, 40);
            Assert.AreEqual(challenge.State.LastScore, 0);
        }
    }


    [TestClass] // Logica Challenge
    public class TestingChallenge
    {
        [TestMethod]
        public void TestingFunction_GetDTChallenge()
        {
            FacadeController cont = FacadeController.getInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));

            ChallengeDetail2 challenge = (ChallengeDetail2)cont.getChallenge(2);
            challenge.Level = 1;

            for(int i = 1; i <= challenge.MaxAttempt; i++){
                System.Random r = new System.Random();
                challenge.CompleteChallenge(r.Next(1, challenge.getSecondsToWakeMeUp().Length + 1));
            }

            DTChallenge dtc = challenge.getDTChallenge();

            Assert.AreEqual(dtc.Attempts, challenge.MaxAttempt);
            Assert.AreEqual(dtc.Finished, true);
        }


        [TestMethod]
        public void TestingFunction_GetDurationString()
        {
            FacadeController cont = FacadeController.getInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6).AddMinutes(-1));
            Assert.AreEqual(cont.getChallenge(2).getDurationString(), "5 dias");

            cont = FacadeController.getInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddHours(6).AddMinutes(-1));
            Assert.AreEqual(cont.getChallenge(2).getDurationString(), "5 horas");

            cont = FacadeController.getInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddMinutes(6).AddSeconds(-1));
            Assert.AreEqual(cont.getChallenge(2).getDurationString(), "5 minutos");

            cont = FacadeController.getInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddMinutes(1).AddSeconds(-1));
            Assert.AreEqual(cont.getChallenge(2).getDurationString(), "Menos de un minuto!!");
        }
    }
};


