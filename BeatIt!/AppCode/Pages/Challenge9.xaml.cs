using System.Globalization;
using System.Windows;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge9
    {
        private ChallengeDetail9 _currentChallenge;
        private IFacadeController _ifc;

        public Challenge9()
        {
            InitializeComponent();

            var navigateInTransition = new NavigationInTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn }
            };

            var navigateOutTransition = new NavigationOutTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut }
            };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            InitChallenge();
        }

        private void InitChallenge()
        {
            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail9)_ifc.getChallenge(9);

            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge()
                .StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}