using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
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
            Assert.AreEqual(4 ,2+2);
            
            //IChallengeController ich = ChallengeController.getInstance();
            //UsainBolt ubBolt = (UsainBolt)ich.getChallenge(BeatIt_.AppCode.Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);

            var ifc = FacadeController.getInstance();
            if (!ifc.isLoggedUser())
            {
                ifc.loginUser(new User());
            }
            var currentChallenge = (ChallengeDetail1)ifc.getChallenge(1);
            currentChallenge.completeChallenge(false, 45, 42);
            //Si esta bien debería ser 174 el puntaje de en el desafío (42+45)*2
            Assert.AreEqual(174, currentChallenge.State.LastScore);

        }
    };
};


