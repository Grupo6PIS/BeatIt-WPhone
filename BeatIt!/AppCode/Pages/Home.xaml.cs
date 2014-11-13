using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
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
        private readonly ApplicationBarIconButton _sendBtn;

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

            _facade = FacadeController.GetInstance();
            var loggedUser = _facade.GetCurrentUser();

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
                Text = AppResources.HomePage_Update
            };
            ApplicationBar.Buttons.Add(refreshBtn);
            refreshBtn.Click += RefreshBtn_Click;

            _sendBtn = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Images/appbar_upload.png", UriKind.Relative),
                Text = AppResources.HomePage_Send,
                IsEnabled = _facade.ShouldSendScore()
            };

            ApplicationBar.Buttons.Add(_sendBtn);
            _sendBtn.Click += SendBtn_Click;

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
            foreach (var entry in _facade.GetChallenges())
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
            var ch = _facade.GetChallenge(tag);

            _facade.SetCurrentChallenge(ch);

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
            var ranking = _facade.GetRanking();
            RankingListBox.Items.Clear();
            foreach (var dtr in ranking)
            {
                var listItem = new RankingListItem
                {
                    SelectedRec =
                    {
                        Visibility = _facade.GetCurrentUser().UserId.Equals(dtr.UserId) ? Visibility.Visible : Visibility.Collapsed
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
                Pivot.IsEnabled = false;
                Pivot.Opacity = 0.3;
                ProgressBar.Visibility = Visibility.Visible;
                // user clicked yes
                var fb = new FacebookClient();
                var parameters = new Dictionary<string, object>();
                parameters["next"] = "https://www.facebook.com/connect/login_success.html";
                parameters["access_token"] = _facade.GetCurrentUser().FbAccessToken;
                var logoutUrl = fb.GetLogoutUrl(parameters);
                var webBrowser = new WebBrowser();
                webBrowser.Navigate(logoutUrl);
                webBrowser.Navigated += (o, args) =>
                {
                    if (args.Uri.AbsoluteUri == "https://www.facebook.com/connect/login_success.html")
                    {
                        _facade.LogoutUser();
                        NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Login.xaml",
                            UriKind.Relative));
                    }
                };
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            e.Cancel = false;
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
                _facade.UpdateRanking(jsonResponse);
                InitRankingListBox();
            }
            else
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show(AppResources.HomePage_RankingError));   
            }
        }

        private void SendBtn_Click(object sender, EventArgs e)
        {
            if (_isSendingScore) return;
            ProgressBar.Visibility = Visibility.Visible;
            var ws = new WebServicesController();
            var userId = _facade.GetCurrentUser().UserId;
            ws.SendScore(userId, _facade.GetRoundScore(), SendScoreFinished);
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
                IsolatedStorageSettings.ApplicationSettings["LastScoreSent"] = _facade.GetRoundScore();
                IsolatedStorageSettings.ApplicationSettings.Save();
                _sendBtn.IsEnabled = _facade.ShouldSendScore();
                Dispatcher.BeginInvoke(() => MessageBox.Show(AppResources.HomePage_ScoreSuccess));  
            }
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/AboutUs.xaml", UriKind.Relative));
        }
    }
}