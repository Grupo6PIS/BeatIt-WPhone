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
        //Private Variables
        private readonly AddressChooserTask _addressTask;
        private readonly IFacadeController _ifc;
        private readonly string _message = AppResources.Challenge3_Message;
        private ChallengeDetail3 currentChallenge;

        public Challenge3()
        {
            InitializeComponent();

            _ifc = FacadeController.getInstance();
            currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);
            //Codigo tasker
            _addressTask = new AddressChooserTask();

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

            ShowST.Text = currentChallenge.getDTChallenge().StartTime.ToString(); // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
            ShowToBeat.Text = currentChallenge.State.BestScore + " pts";
            ShowDuration.Text = currentChallenge.getDurationString();
        }

        //onClick start playing
        private void hyperlinkButtonStartPlay_Click(object sender, RoutedEventArgs e)
        {
            StartPlayGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;
        }

        //onClick publish wall facebook
        private void hyperlinkButtonPublish_Click(object sender, RoutedEventArgs e)
        {
            var fb = new FacebookClient(_ifc.getCurrentUser().FbAccessToken);
            currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);

            fb.PostCompleted += (o, args) =>
            {
                
                if (args.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                    return;
                }

                //var result = (IDictionary<string, object>)args.GetResultData();
                Dispatcher.BeginInvoke(() => MessageBox.Show("Message Posted successfully"));
            };

            var parameters = new Dictionary<string, object>();
            parameters["message"] = _message;
            fb.PostAsync("me/feed", parameters);

            //Refresh countFacebook
            currentChallenge.AddFacebook();
        }

        
        //onClick send SMS
        private void hyperlinkButtonSMS_Click(object sender, RoutedEventArgs e)
        {
            
            var sms = new SmsComposeTask();
            currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);
            sms.Body = _message;
            var contacts = new Contacts();
            _addressTask.Show();

            //Refresh countSMS
            currentChallenge.AddSms();
        }

        private void hyperlinkButtonFinish_Click(object sender, RoutedEventArgs e)
        {
            currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);
            var ks = currentChallenge.CompleteChallenge(false);
            if (ks.Key)
            {
                MessageBox.Show("Nuevo alto el puntaje!\n" + ks.Value + "puntos");
            }
            else
            {
                MessageBox.Show("No has superado el puntaje.\n Tienes" + ks.Value + "puntos");
            }
        }
    }
}