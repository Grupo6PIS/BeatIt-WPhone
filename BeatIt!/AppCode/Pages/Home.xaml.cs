using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.CustomControls;
using BeatIt_.AppCode.Datatypes;
using BeatIt_.AppCode.Interfaces;
using Facebook;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BeatIt_.Pages
{
    public partial class Home : PhoneApplicationPage
    {
        IFacadeController ifc;
        private WebServicesController ws;

        public Home()
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

            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 0.7;
            ApplicationBar.IsVisible = false;
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton refreshBtn = new ApplicationBarIconButton();
            refreshBtn.IconUri = new Uri("/Images/appbar_refresh.png", UriKind.Relative);
            refreshBtn.Text = "Actualizar";
            ApplicationBar.Buttons.Add(refreshBtn);
            refreshBtn.Click += new EventHandler(refreshBtn_Click);

            ifc = FacadeController.getInstance();
            User loggedUser = ifc.getCurrentUser();

            ws = new WebServicesController();

            profileNameTxtBlock.Text = loggedUser.FirstName + " " + loggedUser.LastName;
            profileCountryTxtBlock.Text = loggedUser.Country;
            profileEmailTextBlock.Text = loggedUser.Email;
            Uri uri = new Uri(loggedUser.ImageUrl, UriKind.Absolute);
            profileImage.Source = new BitmapImage(uri);

            this.initChallengesListBox();
            this.InitRankingListBox();
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

        private void initChallengesListBox()
        {
            foreach (KeyValuePair<int, Challenge> entry in ifc.getChallenges())
            {
                Challenge ch = entry.Value;
                ChallenesListItem listItem = new ChallenesListItem();
                listItem.backgroundRec.Fill = GetColorFromHexa(ch.ColorHex);
                Uri uri = new Uri("/BeatIt!;component/Images/icon_challenge_" + ch.ChallengeId + ".png", UriKind.Relative);
                listItem.image.Source = new BitmapImage(uri);
                listItem.linkBtn.Click += linkBtn_Click;
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

        private void linkBtn_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton linkBtn = sender as HyperlinkButton;
            int tag = Convert.ToInt32(linkBtn.Tag);
            Challenge ch = ifc.getChallenge(tag);

            ifc.setCurrentChallenge(ch);

            String pagePath = "";
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
            List<DTRanking> ranking = ifc.getRanking();
            RankingListBox.Items.Clear();
            for (int i = 0; i < ranking.Count; i++)
            {
                DTRanking dtr = (DTRanking)ranking[i];
                RankingListItem listItem = new RankingListItem();
                listItem.selectedRec.Visibility = (ifc.getCurrentUser().UserId == dtr.UserId) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                listItem.positionTxtBlock.Text = dtr.Position.ToString();
                listItem.scoreTxtBlock.Text = dtr.Score.ToString();
                listItem.nameTxtBlock.Text = dtr.Name;
                Uri uri = new Uri(dtr.ImageUrl, UriKind.Absolute);
                listItem.userImage.Source = new BitmapImage(uri);
                RankingListBox.Items.Add(listItem);
            }
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var fb = new FacebookClient();
            var parameters = new Dictionary<string, object>();
            parameters["next"] = "https://www.facebook.com/connect/login_success.html";
            parameters["access_token"] = ifc.getCurrentUser().FbAccessToken;
            var logoutUrl = fb.GetLogoutUrl(parameters);
            var webBrowser = new WebBrowser();
            webBrowser.Navigate(logoutUrl);
            webBrowser.Navigated += (o, args) =>
            {
                if (args.Uri.AbsoluteUri == "https://www.facebook.com/connect/login_success.html") 
                {
                    ifc.logoutUser();
                    NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Login.xaml", UriKind.Relative));
                }
            };
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int no = Pivot.SelectedIndex;
            if (no == 1)
            {
                ApplicationBar.IsVisible = true;
            }
            else
            {
                ApplicationBar.IsVisible = false;
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e) 
        {
            ifc.updateRanking(InitRankingListBox);
        }
    }
}