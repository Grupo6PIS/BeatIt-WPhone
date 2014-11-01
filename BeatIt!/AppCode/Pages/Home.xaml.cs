using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.CustomControls;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
using Facebook;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;

namespace BeatIt_.AppCode.Pages
{
    public partial class Home
    {
        private readonly IFacadeController _facade;
        private bool _isRefreshingRanking;
        private bool _isSendingScore;

        public Home()
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

            ProgressBar.IsIndeterminate = true;

            ApplicationBar = new ApplicationBar
            {
                Mode = ApplicationBarMode.Minimized,
                Opacity = 0.7,
                IsVisible = false,
                IsMenuEnabled = false
            };

            var refreshBtn = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/appbar_refresh.png", UriKind.Relative),
                Text = "Actualizar"
            };
            ApplicationBar.Buttons.Add(refreshBtn);
            refreshBtn.Click += RefreshBtn_Click;

            ////////////////////////////////////////////////////////////////////////////
            var sendBtn = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/appbar_upload.png", UriKind.Relative),
                Text = "Enviar Puntaje",
                //IsEnabled = FacadeController.GetInstance().ShouldSendScore
            };
            ApplicationBar.Buttons.Add(sendBtn);
            sendBtn.Click += SendBtn_Click;
            ////////////////////////////////////////////////////////////////////////////


            _facade = FacadeController.GetInstance();
            var loggedUser = _facade.getCurrentUser();

            ProfileNameTxtBlock.Text = loggedUser.Name;
            ProfileCountryTxtBlock.Text = loggedUser.Country;
            ProfileEmailTextBlock.Text = loggedUser.Email;
            var uri = new Uri(loggedUser.ImageUrl, UriKind.Absolute);
            ProfileImage.Source = new BitmapImage(uri);

            InitChallengesListBox();
            InitRankingListBox();
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

        private void InitChallengesListBox()
        {
            foreach (var entry in _facade.getChallenges())
            {
                var ch = entry.Value;
                var listItem = new ChallenesListItem {BackgroundRec = {Fill = GetColorFromHexa(ch.ColorHex)}};
                var str = "/BeatIt!;component/Images/icon_challenge_" + ch.ChallengeId + ".png";
                var uri = new Uri(str,UriKind.Relative);
                listItem.Image.Source = new BitmapImage(uri);
                listItem.LinkBtn.Click += LinkBtn_Click;
                listItem.LinkBtn.Tag = ch.ChallengeId;
                if (!ch.IsEnabled)
                {
                    listItem.BackgroundRec.Opacity = 0.2;
                    listItem.Image.Opacity = 0.2;
                }
                listItem.CompletedImage.Visibility = ch.State.CurrentAttempt > 0 ? Visibility.Visible : Visibility.Collapsed;
                listItem.LinkBtn.IsEnabled = ch.IsEnabled;
                ChallengesListBox.Items.Add(listItem);
            }
        }

        private void LinkBtn_Click(object sender, RoutedEventArgs e)
        {
            var linkBtn = sender as HyperlinkButton;
            if (linkBtn == null) return;
            var tag = Convert.ToInt32(linkBtn.Tag);
            var ch = _facade.getChallenge(tag);

            _facade.setCurrentChallenge(ch);

            String pagePath;
            if (ch.State.CurrentAttempt == 0)
            {
                pagePath = "/BeatIt!;component/AppCode/Pages/Challenge" + tag + ".xaml";
            }
            else
            {
                pagePath = "/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml";
            }
            NavigationService.Navigate(new Uri(pagePath, UriKind.Relative));
        }

        private void InitRankingListBox()
        {
            var ranking = _facade.getRanking();
            RankingListBox.Items.Clear();
            foreach (var dtr in ranking)
            {
                var listItem = new RankingListItem
                {
                    SelectedRec =
                    {
                        Visibility = _facade.getCurrentUser().UserId.Equals(dtr.UserId) ? Visibility.Visible : Visibility.Collapsed
                    },
                    PositionTxtBlock = {Text = dtr.Position.ToString(CultureInfo.InvariantCulture)},
                    ScoreTxtBlock = {Text = dtr.Score.ToString(CultureInfo.InvariantCulture)},
                    NameTxtBlock = {Text = dtr.Name}
                };
                var uri = new Uri(dtr.ImageUrl, UriKind.Absolute);
                listItem.UserImage.Source = new BitmapImage(uri);
                RankingListBox.Items.Add(listItem);
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var m = MessageBox.Show(AppResources.HomePage_LogoutMessage, "Logout", MessageBoxButton.OKCancel);
            
            if (m == MessageBoxResult.OK)
            {
                ProgressBar.Visibility = Visibility.Visible;
                // user clicked yes
                var fb = new FacebookClient();
                var parameters = new Dictionary<string, object>();
                parameters["next"] = "https://www.facebook.com/connect/login_success.html";
                parameters["access_token"] = _facade.getCurrentUser().FbAccessToken;
                var logoutUrl = fb.GetLogoutUrl(parameters);
                var webBrowser = new WebBrowser();
                webBrowser.Navigate(logoutUrl);
                webBrowser.Navigated += (o, args) =>
                {
                    if (args.Uri.AbsoluteUri == "https://www.facebook.com/connect/login_success.html")
                    {
                        _facade.logoutUser();
                        NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Login.xaml",
                            UriKind.Relative));
                    }
                };
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedIndex = Pivot.SelectedIndex;
            ApplicationBar.IsVisible = selectedIndex == 1;
            ProgressBar.Visibility = (selectedIndex == 1 && (_isRefreshingRanking || _isSendingScore )) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            if (_isRefreshingRanking) return;
            _isRefreshingRanking = true;
            ProgressBar.Visibility = Visibility.Visible;
            var ws = new WebServicesController();
            ws.GetRanking(GetRankingFinished);
        }

        private void GetRankingFinished(JObject jsonResponse)
        {
            _isRefreshingRanking = false;
            ProgressBar.Visibility = Visibility.Collapsed;
            if (!(bool) jsonResponse["error"])
            {
                _facade.updateRanking(jsonResponse);
                InitRankingListBox();
            }
            else
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show(AppResources.HomePage_RankingError));   
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        private void SendBtn_Click(object sender, EventArgs e)
        {
            if (_isSendingScore) return;
            _isSendingScore = true;

            ProgressBar.Visibility = Visibility.Visible;
            var ws = new WebServicesController();
            var userId = FacadeController.GetInstance().getCurrentUser().UserId;
            var score = FacadeController.GetInstance().GetRoundScore();
            ws.SendScore(userId, score, SendScoreFinished);
        }

        private void SendScoreFinished(JObject jsonResponse)
        {

            _isSendingScore = false;
            ProgressBar.Visibility = Visibility.Collapsed;
            if ((bool)jsonResponse["error"])
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show(AppResources.HomePage_ScoreError));
            }
            else
            {
                FacadeController.GetInstance().ShouldSendScore = false;
                Dispatcher.BeginInvoke(() => MessageBox.Show(AppResources.HomePage_ScoreSuccess));   
            }
        }
        ////////////////////////////////////////////////////////////////////////////
    }
}