using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Datatypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//ojo

namespace BeatIt.Tests
{
    [TestClass]
    public class TestingUb
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

    [TestClass]
    public class TestingPhoneFlying
    {
        [TestMethod]
        public void TestingFunctionGetSeconsToWakeMeUp()
        {
            ChallengeDetail2 ch = new ChallengeDetail2();
            ch.Level = 1;
            int[] secondsToWakeMeUp = ch.getSeconsToWakeMeUp();
            Assert.AreEqual(secondsToWakeMeUp[0], 3);

            ch.Level = 2;
            secondsToWakeMeUp = ch.getSeconsToWakeMeUp();
            Assert.AreEqual(secondsToWakeMeUp[3], 9);

            ch = new ChallengeDetail2();
            ch.State = new State();

        }
    }
};


