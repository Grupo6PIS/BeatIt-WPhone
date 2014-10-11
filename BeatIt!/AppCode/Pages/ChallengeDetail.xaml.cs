using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BeatIt_.AppCode.Pages
{
    public partial class ChallengeDetail
    {
        readonly Challenge _challenge;

        public ChallengeDetail()
        {
            InitializeComponent();

            var navigateInTransition = new NavigationInTransition
            {
                Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeIn},
                Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeIn}
            };

            var navigateOutTransition = new NavigationOutTransition
            {
                Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeOut},
                Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeOut}
            };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            IFacadeController ifc = FacadeController.GetInstance();
            _challenge = ifc.getCurrentChallenge();

            ApplicationBar = new ApplicationBar
            {
                Mode = ApplicationBarMode.Minimized,
                Opacity = 0.7,
                IsVisible = _challenge.State.CurrentAttempt < _challenge.MaxAttempt,
                IsMenuEnabled = false
            };

            var retryBtn = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/appbar_retry.png", UriKind.Relative),
                Text = "Reintentar"
            };
            ApplicationBar.Buttons.Add(retryBtn);
            retryBtn.Click += retryBtn_Click;

            PageTitle.Text = _challenge.Name;
            imageRec.Fill = GetColorFromHexa(_challenge.ColorHex);
            
            lastScoreRec.Fill = GetColorFromHexa(_challenge.ColorHex);
            lastScoreTxtBlock.Text = _challenge.State.LastScore.ToString(CultureInfo.InvariantCulture);

            bestScoreRec.Fill = GetColorFromHexa(_challenge.ColorHex);
            bestScoreTxtBlock.Text = _challenge.State.BestScore.ToString(CultureInfo.InvariantCulture);

            var uri = new Uri("/BeatIt!;component/Images/icon_challenge_" + _challenge.ChallengeId + ".png", UriKind.Relative);
            iconImage.Source = new BitmapImage(uri);
            startDateTxtBlock.Text = _challenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
            attemptsTxtBlock.Text = _challenge.State.CurrentAttempt + "/" + _challenge.MaxAttempt;
        }

        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }

        private void retryBtn_Click(object sender, EventArgs e)
        {
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/Challenge" + _challenge.ChallengeId + ".xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}