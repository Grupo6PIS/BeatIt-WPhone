using System;

namespace BeatIt_.AppCode.Enums
{
    public class ChallengeType
    {
        public enum CHALLENGE_TYPE { USAIN_BOLT };

        // No es lo mas prolijo, lo hice para probar.
        public static string ToString(CHALLENGE_TYPE challenge)
        {
            String toReturn = "";

            switch (challenge)
            {
                case CHALLENGE_TYPE.USAIN_BOLT:
                    toReturn = "USAIN_BOLT";
                    break;
            }

            return toReturn;
        }

        public static CHALLENGE_TYPE ToChallengeType(String challenge)
        {
            CHALLENGE_TYPE toReturn = CHALLENGE_TYPE.USAIN_BOLT;

            switch (challenge)
            {
                case "USAIN_BOLT":
                    toReturn = CHALLENGE_TYPE.USAIN_BOLT;
                    break;
            }

            return toReturn;
        }
    }
}
