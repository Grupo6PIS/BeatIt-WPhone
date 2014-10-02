using BeatIt_.Resources;

namespace BeatIt_
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static AppResources localizedResources = new AppResources();

        public AppResources AppResources 
        {
            get { return localizedResources; }
        }
    }
}
