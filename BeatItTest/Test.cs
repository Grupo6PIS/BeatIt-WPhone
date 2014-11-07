using BeatIt_.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt.Tests
{
    [TestClass] //Usain Bolt
    public class TestingChallengeDetail1
    {
        [TestMethod]
        public void TestingFunctioncalculatePuntaje()
        {
            
            var ifc = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));

            var currentChallenge = (ChallengeDetail1)ifc.GetChallenge(1);
            currentChallenge.CompleteChallenge(false, 45, 42);

            ////348 = (42+45)*4
            Assert.AreEqual(348, currentChallenge.State.LastScore);
        }
    };

    [TestClass] //Wake Me Up!
    public class TestingChallengeDetail2
    {
        [TestMethod]
        public void TestingFunction_GetSecondsToWakeMeUp()
        {
            var ch = new ChallengeDetail2 {Level = 1};
            int[] secondsToWakeMeUp = ch.GetSecondsToWakeMeUp();
            Assert.AreEqual(secondsToWakeMeUp[0], 3);

            ch.Level = 2;
            secondsToWakeMeUp = ch.GetSecondsToWakeMeUp();
            Assert.AreEqual(secondsToWakeMeUp[3], 9);
        }

        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
            FacadeController cont = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1),
                System.DateTime.Now.AddDays(6));

            var challenge = (ChallengeDetail2) cont.GetChallenge(2);
            challenge.Level = 1;

            challenge.CompleteChallenge(1);
            
            Assert.AreEqual(challenge.State.BestScore, 80);
            Assert.AreEqual(challenge.State.LastScore, 80);

            challenge.CompleteChallenge(2);
            
            Assert.AreEqual(challenge.State.BestScore, 160);
            Assert.AreEqual(challenge.State.LastScore, 160);

            challenge.CompleteChallenge(0);
            
            Assert.AreEqual(challenge.State.BestScore, 160);
            Assert.AreEqual(challenge.State.LastScore, 0);
        }
    };

    [TestClass] //Challenge
    public class TestingChallenge
    {
        [TestMethod]
        public void TestingFunction_GetDTChallenge()
        {
            var cont = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1),
                System.DateTime.Now.AddDays(6));

            var challenge = (ChallengeDetail2) cont.GetChallenge(2);
            challenge.Level = 1;

            for (var i = 1; i <= challenge.MaxAttempt; i++)
            {
                var r = new System.Random();
                challenge.CompleteChallenge(r.Next(1, challenge.GetSecondsToWakeMeUp().Length + 1));
            }

            DTChallenge dtc = challenge.GetDtChallenge();

            Assert.AreEqual(dtc.Attempts, challenge.MaxAttempt);
            Assert.AreEqual(dtc.Finished, true);
        }

        [TestMethod]
        public void TestingFunction_GetDurationString()
        {
            var cont = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1),
                System.DateTime.Now.AddDays(6).AddMinutes(-1));
            Assert.AreEqual(cont.GetChallenge(2).GetDurationString(), "5 " + AppResources.Challenge_Days);

            cont = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1),
                System.DateTime.Now.AddHours(6).AddMinutes(-1));
            Assert.AreEqual(cont.GetChallenge(2).GetDurationString(), "5 " + AppResources.Challenge_Hours);

            cont = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1),
                System.DateTime.Now.AddMinutes(6).AddSeconds(-1));
            Assert.AreEqual(cont.GetChallenge(2).GetDurationString(), "5 " + AppResources.Challenge_Minutes);

            cont = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1),
                System.DateTime.Now.AddMinutes(1).AddSeconds(-1));
            Assert.AreEqual(cont.GetChallenge(2).GetDurationString(), AppResources.Challenge_LessMinutes);
        }
    };

    [TestClass] // Can you play?
    public class TestingChallengeDetail3
    {
        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
            var ifc = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));
            var ch = (ChallengeDetail3)ifc.GetChallenge(3);

            var result = ch.CompleteChallenge(true);
            Assert.AreEqual(false, result.Key);
            Assert.AreEqual(0, result.Value);

            ch.AddFacebook();
            ch.AddSms();
            ch.AddSms();
            ch.AddSms();
            ch.AddSms();
            result = ch.CompleteChallenge(false);
            Assert.AreEqual(true, result.Key);
            Assert.AreEqual(130, result.Value);

            ch.AddFacebook();
            ch.AddSms();
            ch.AddSms();
            result = ch.CompleteChallenge(false);
            Assert.AreEqual(false, result.Key);
            Assert.AreEqual(130, result.Value);
        }

        [TestMethod]
        public void TestingFunction_AddFacebook()
        {
            var ch = new ChallengeDetail3();

            Assert.AreEqual(0, ch.GetCountFacebook());
            ch.AddFacebook();
            Assert.AreEqual(1, ch.GetCountFacebook());
            ch.AddFacebook();
            Assert.AreEqual(2, ch.GetCountFacebook());
            ch.AddFacebook();
            Assert.AreEqual(3, ch.GetCountFacebook());
            ch.AddFacebook();
            Assert.AreEqual(4, ch.GetCountFacebook());
            ch.AddFacebook();
            Assert.AreEqual(5, ch.GetCountFacebook());
        }

        [TestMethod]
        public void TestingFunction_AddSms()
        {
            var ch = new ChallengeDetail3();

            Assert.AreEqual(0, ch.GetCountSms());
            ch.AddSms();
            Assert.AreEqual(1, ch.GetCountSms());
            ch.AddSms();
            Assert.AreEqual(2, ch.GetCountSms());
            ch.AddSms();
            Assert.AreEqual(3, ch.GetCountSms());
            ch.AddSms();
            Assert.AreEqual(4, ch.GetCountSms());
            ch.AddSms();
            Assert.AreEqual(5, ch.GetCountSms());
        }
    };

    [TestClass] //Callar al Perro
    public class TestingChallengeDetail4
    {
        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
            var ifc = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));
            var ch = (ChallengeDetail4)ifc.GetChallenge(4);

            Assert.IsNotNull(ch);

            Assert.AreEqual(ch.State.BestScore, 0);
            Assert.AreEqual(ch.State.LastScore, 0);
            Assert.AreEqual(ch.State.CurrentAttempt, 0);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(new [] {100, 100, 100});
            Assert.AreEqual(ch.State.LastScore, 30);
            Assert.AreEqual(ch.State.BestScore, 30);
            Assert.AreEqual(ch.State.CurrentAttempt, 1);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(new []{0, 0, 0});
            Assert.AreEqual(ch.State.LastScore, 0);
            Assert.AreEqual(ch.State.BestScore, 30);
            Assert.AreEqual(ch.State.CurrentAttempt, 2);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(new[] { 20, 20, 20 });
            Assert.AreEqual(ch.State.LastScore, 150);
            Assert.AreEqual(ch.State.BestScore, 150);
            Assert.AreEqual(ch.State.CurrentAttempt, 3);
            Assert.IsTrue(ch.State.Finished);
        }
    };

    [TestClass] //Catch Me
    public class TestingChallengeDetail7
    {
        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
            var ifc = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));
            var ch = (ChallengeDetail7)ifc.GetChallenge(7);

            

            ch.CompleteChallenge(0);
            Assert.AreEqual(0, ch.State.BestScore);

            ch.CompleteChallenge(5);
            Assert.AreEqual(5, ch.State.BestScore);


            ch.CompleteChallenge(29);
            Assert.AreEqual(170, ch.State.BestScore);
            
        }
    };

    [TestClass] //Color & Texto
    public class TestingChallengeDetail8
    {
        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
            var ifc = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));
            var ch = (ChallengeDetail8)ifc.GetChallenge(8);

            Assert.IsNotNull(ch);

            Assert.AreEqual(ch.State.BestScore, 0);
            Assert.AreEqual(ch.State.LastScore, 0);
            Assert.AreEqual(ch.State.CurrentAttempt, 0);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(10);
            Assert.AreEqual(ch.State.LastScore, 30);
            Assert.AreEqual(ch.State.BestScore, 30);
            Assert.AreEqual(ch.State.CurrentAttempt, 1);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(5);
            Assert.AreEqual(ch.State.LastScore, 5);
            Assert.AreEqual(ch.State.BestScore, 30);
            Assert.AreEqual(ch.State.CurrentAttempt, 2);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(30);
            Assert.AreEqual(ch.State.LastScore, 230);
            Assert.AreEqual(ch.State.BestScore, 230);
            Assert.AreEqual(ch.State.CurrentAttempt, 3);
            Assert.IsTrue(ch.State.Finished);
        }
    };

    [TestClass] //Song Complete
    public class TestingChallengeDetail9
    {
        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
            var ifc = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));
            var ch = (ChallengeDetail9)ifc.GetChallenge(9);

            Assert.IsNotNull(ch);

            Assert.AreEqual(ch.State.BestScore, 0);
            Assert.AreEqual(ch.State.LastScore, 0);
            Assert.AreEqual(ch.State.CurrentAttempt, 0);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(new[] { 40, 40, 40, 40, 40 });
            Assert.AreEqual(ch.State.LastScore, 200);
            Assert.AreEqual(ch.State.BestScore, 200);
            Assert.AreEqual(ch.State.CurrentAttempt, 1);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(new[] { 0, 0, 0, 0, 0 });
            Assert.AreEqual(ch.State.LastScore, 0);
            Assert.AreEqual(ch.State.BestScore, 200);
            Assert.AreEqual(ch.State.CurrentAttempt, 2);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(new[] { 40, 0, 40, 0, 40 });
            Assert.AreEqual(ch.State.LastScore, 120);
            Assert.AreEqual(ch.State.BestScore, 200);
            Assert.AreEqual(ch.State.CurrentAttempt, 3);
            Assert.IsTrue(ch.State.Finished);
        }
    };


    [TestClass] //Selfie Groupie
    public class TestingChallengeDetail10
    {
        [TestMethod]
        public void TestingFunction_CompleteChallenge()
        {
         /*   var ifc = FacadeController.GetInstanceForTesting(new User(), System.DateTime.Now.AddDays(-1), System.DateTime.Now.AddDays(6));
            var ch = (ChallengeDetail10)ifc.GetChallenge(10);

            Assert.IsNotNull(ch);

            Assert.AreEqual(ch.State.BestScore, 0);
            Assert.AreEqual(ch.State.LastScore, 0);
            Assert.AreEqual(ch.State.CurrentAttempt, 0);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(1);
            Assert.AreEqual(ch.State.LastScore, 0);
            Assert.AreEqual(ch.State.BestScore, 0);
            Assert.AreEqual(ch.State.CurrentAttempt, 1);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(3);
            Assert.AreEqual(ch.State.LastScore, 90);
            Assert.AreEqual(ch.State.BestScore, 90);
            Assert.AreEqual(ch.State.CurrentAttempt, 2);
            Assert.IsFalse(ch.State.Finished);

            ch.CompleteChallenge(2);
            Assert.AreEqual(ch.State.LastScore, 60);
            Assert.AreEqual(ch.State.BestScore, 90);
            Assert.AreEqual(ch.State.CurrentAttempt, 3);
            Assert.IsTrue(ch.State.Finished);*/
        }
    };

}


