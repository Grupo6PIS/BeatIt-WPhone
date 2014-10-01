using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
using Facebook;
using Microsoft.Phone.Controls;

/* INVITA A TUS AMIGOS */

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge3 : PhoneApplicationPage
    {
        public Challenge3()
        {
            InitializeComponent();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeIn};
            navigateInTransition.Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeIn};

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeOut};
            navigateOutTransition.Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeOut};
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
        }

        private string _lastMessageId = AppResources.Challenge3_WallMessage;

        private void hyperlinkButtonPublish_Click(object sender, RoutedEventArgs e)
        {
            IFacadeController ifc = FacadeController.getInstance();
            var fb = new FacebookClient(ifc.getCurrentUser().FbAccessToken);

            fb.PostCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)args.GetResultData();
                _lastMessageId = (string)result["id"];

                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Message Posted successfully");
                });
            };

            var parameters = new Dictionary<string, object>();
            parameters["message"] = _lastMessageId;

            fb.PostAsync("me/feed", parameters);
            
        }

        private void hyperlinkButtonStartPlay_Click(object sender, RoutedEventArgs e)
        {
            this.StartPlayGrid.Visibility = Visibility.Collapsed;
            this.InProgressGrid.Visibility = Visibility.Visible;
        }
    }
}