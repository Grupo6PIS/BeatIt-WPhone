using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BeatIt_.Pages
{
    public partial class ChallengeDetail : PhoneApplicationPage
    {
        Challenge challenge;

        public ChallengeDetail()
        {
            InitializeComponent();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn };
            navigateInTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn };

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut };
            navigateOutTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            IFacadeController ifc = FacadeController.GetInstance();
            challenge = ifc.getCurrentChallenge();

            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 0.7;
            ApplicationBar.IsVisible = challenge.State.CurrentAttempt < challenge.MaxAttempt;
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton retryBtn = new ApplicationBarIconButton();
            retryBtn.IconUri = new Uri("/Images/appbar_retry.png", UriKind.Relative);
            retryBtn.Text = "Reintentar";
            ApplicationBar.Buttons.Add(retryBtn);
            retryBtn.Click += new EventHandler(retryBtn_Click);

            this.PageTitle.Text = challenge.Name;
            this.imageRec.Fill = GetColorFromHexa(challenge.ColorHex);
            
            this.lastScoreRec.Fill = GetColorFromHexa(challenge.ColorHex);
            this.lastScoreTxtBlock.Text = challenge.State.LastScore.ToString();

            this.bestScoreRec.Fill = GetColorFromHexa(challenge.ColorHex);
            this.bestScoreTxtBlock.Text = challenge.State.BestScore.ToString();

            Uri uri = new Uri("/BeatIt!;component/Images/icon_challenge_" + challenge.ChallengeId + ".png", UriKind.Relative);
            this.iconImage.Source = new BitmapImage(uri);
            this.startDateTxtBlock.Text = challenge.getDTChallenge().StartTime.ToString();
            this.attemptsTxtBlock.Text = challenge.State.CurrentAttempt + "/" + challenge.MaxAttempt;
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
            Uri uri = new Uri("/BeatIt!;component/AppCode/Pages/Challenge" + challenge.ChallengeId + ".xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}