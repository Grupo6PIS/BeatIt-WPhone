using System;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;

namespace BeatIt_.AppCode.Pages
{
    public partial class ChallengeDetail
    {
        readonly Challenge _challenge;
        private readonly IFacadeController _facade;

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
                Text = AppResources.HomePage_RetryBtnTitle
            };
            ApplicationBar.Buttons.Add(retryBtn);
            retryBtn.Click += retryBtn_Click;

            PageTitle.Text = _challenge.Name;
            ImageRectangle.Fill = GetColorFromHexa(_challenge.ColorHex);
            
            LastScoreRectangle.Fill = GetColorFromHexa(_challenge.ColorHex);
            LastScoreTextBlock.Text = _challenge.State.LastScore.ToString(CultureInfo.InvariantCulture);

            BestScoreRectangle.Fill = GetColorFromHexa(_challenge.ColorHex);
            BestScoreTextBlock.Text = _challenge.State.BestScore.ToString(CultureInfo.InvariantCulture);

            var uri = new Uri("/BeatIt!;component/Images/icon_challenge_" + _challenge.ChallengeId + ".png", UriKind.Relative);
            IconImage.Source = new BitmapImage(uri);
            StartTimeTextBlock.Text = _challenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
            AttemptsTextBlock.Text = _challenge.State.CurrentAttempt + "/" + _challenge.MaxAttempt;

            _facade = FacadeController.GetInstance();

            if (_facade.ShouldSendScore())
            {
                SendScore();   
            }
        }

        private void SendScore()
        {
            var ws = new WebServicesController();
            var userId = _facade.getCurrentUser().UserId;
            ws.SendScore(userId, _facade.GetRoundScore(), SendScoreFinished);
        }

        private void SendScoreFinished(JObject jsonResponse)
        {
            if ((bool)jsonResponse["error"])
            {
                System.Diagnostics.Debug.WriteLine(AppResources.HomePage_ScoreError);
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["LastScoreSent"] = _facade.GetRoundScore();
                IsolatedStorageSettings.ApplicationSettings.Save();
                System.Diagnostics.Debug.WriteLine(AppResources.HomePage_ScoreSuccess);
            }
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