using Microsoft.VisualStudio.TestTools.UnitTesting;

//ojo

namespace BeatIt.Tests
{
    [TestClass]
    public class TestingUb
    {
        [TestMethod]
        public void TestingFunctioncalculatePuntaje()
        {
            Assert.AreEqual(5,2+2);
            
            //IChallengeController ich = ChallengeController.getInstance();
           // UsainBolt ubBolt = (UsainBolt)ich.getChallenge(BeatIt_.AppCode.Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);

            // ACÁ se tiene que probar la función de puntaje, 
            // el tema es que no está creada la función de almacenar el puntaje
            //ubBolt.getPuntajeObtenido()
        }
    };
};


