using System.Collections.Generic;
using System.Windows;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
using Facebook;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
/* INVITA A TUS AMIGOS */
using Microsoft.Phone.UserData;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge3 : PhoneApplicationPage
    {
        public Challenge3()
        {
            InitializeComponent();


            //Codigo tasker
            _addressTask = new AddressChooserTask();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeIn};
            navigateInTransition.Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeIn};

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeOut};
            navigateOutTransition.Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeOut};
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
        }

        private string _message = AppResources.Challenge3_Message;

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

                Dispatcher.BeginInvoke(() => MessageBox.Show("Message Posted successfully"));
            };

            var parameters = new Dictionary<string, object>();
            parameters["message"] = _message;

            fb.PostAsync("me/feed", parameters);
            
        }

        private void hyperlinkButtonStartPlay_Click(object sender, RoutedEventArgs e)
        {
            this.StartPlayGrid.Visibility = Visibility.Collapsed;
            this.InProgressGrid.Visibility = Visibility.Visible;
        }

        private void hyperlinkButtonSMS_Click(object sender, RoutedEventArgs e)
        {
            
            var sms = new SmsComposeTask();
            var ifc = FacadeController.getInstance();
            var currentChallenge = (ChallengeDetail3)ifc.getChallenge(3);
            sms.Body = _message;
            var contacts = new Contacts();
            _addressTask.Show();
        }

        /*codigo copiado*/
        private readonly AddressChooserTask _addressTask;
        // Constructor

    }
}