using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Facebook;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using BeatIt_.AppCode.CustomControls;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.Pages
{
    public partial class Home : PhoneApplicationPage
    {

        IFacadeController ifc;

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


            ifc = FacadeController.getInstance();
            User loggedUser = ifc.getCurrentUser();

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
                ChallengesListBox.Items.Add(listItem);
            }
        }

        private void linkBtn_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton linkBtn = sender as HyperlinkButton;
            int tag = Convert.ToInt32(linkBtn.Tag);
            Challenge ch = ifc.getChallenge(tag);

            String pagePath = "";
            if (ch.State.getCurrentAttempt() == 0)
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

            for (int i = 0; i < ranking.Count; i++)
            {
                DTRanking dtr = (DTRanking)ranking[i];
                RankingListItem listItem = new RankingListItem();
                listItem.selectedRec.Visibility = (ifc.getCurrentUser().UserId == dtr.getUserId()) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                listItem.positionTxtBlock.Text = dtr.getPosition().ToString();
                listItem.scoreTxtBlock.Text = dtr.getScore().ToString();
                listItem.nameTxtBlock.Text = dtr.getName();
                Uri uri = new Uri(dtr.getImageUrl(), UriKind.Absolute);
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
    }
}