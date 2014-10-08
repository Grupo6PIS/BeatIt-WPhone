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
using Facebook;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BeatIt_.AppCode.Pages
{
    public partial class Home
    {
        private readonly IFacadeController _facade;

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

            _facade = FacadeController.getInstance();
            var loggedUser = _facade.getCurrentUser();

            ProfileNameTxtBlock.Text = loggedUser.FirstName + " " + loggedUser.LastName;
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
                var listItem = new ChallenesListItem {backgroundRec = {Fill = GetColorFromHexa(ch.ColorHex)}};
                var uri = new Uri("/BeatIt!;component/Images/icon_challenge_" + ch.ChallengeId + ".png",
                    UriKind.Relative);
                listItem.image.Source = new BitmapImage(uri);
                listItem.linkBtn.Click += LinkBtn_Click;
                listItem.linkBtn.Tag = ch.ChallengeId;
                if (!ch.IsEnabled)
                {
                    listItem.backgroundRec.Opacity = 0.2;
                    listItem.image.Opacity = 0.2;
                }
                listItem.linkBtn.IsEnabled = ch.IsEnabled;
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
                    selectedRec =
                    {
                        Visibility =
                            (_facade.getCurrentUser().UserId == dtr.UserId) ? Visibility.Visible : Visibility.Collapsed
                    },
                    positionTxtBlock = {Text = dtr.Position.ToString(CultureInfo.InvariantCulture)},
                    scoreTxtBlock = {Text = dtr.Score.ToString(CultureInfo.InvariantCulture)},
                    nameTxtBlock = {Text = dtr.Name}
                };
                var uri = new Uri(dtr.ImageUrl, UriKind.Absolute);
                listItem.userImage.Source = new BitmapImage(uri);
                RankingListBox.Items.Add(listItem);
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
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
                    NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Login.xaml", UriKind.Relative));
                }
            };
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
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            _facade.updateRanking(InitRankingListBox);
        }
    }
}