using System;
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
        private ChallengeDetail3 _currentChallenge;

        public Challenge3()
        {
            InitializeComponent();

            _ifc = FacadeController.getInstance();
            _currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);
            _ifc.setCurrentChallenge(_currentChallenge);
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

            ShowST.Text = _currentChallenge.getDTChallenge().StartTime.ToString(); // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
            ShowToBeat.Text = _currentChallenge.State.BestScore + " pts";
            ShowDuration.Text = _currentChallenge.getDurationString();
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
            _currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);

            fb.PostCompleted += (o, args) =>
            {

                Dispatcher.BeginInvoke(() => MessageBox.Show("Message Posted successfully"));
            };

            //Facebook no deja prublicar 2 veces lo mismo
            //var str = "("+ _numEspacios +") " + _message; 

            var parameters = new Dictionary<string, object>();
            parameters["message"] = _message;
            fb.PostAsync("me/feed", parameters);

            //Refresh countFacebook
            _currentChallenge.AddFacebook();

        }

        
        //onClick send SMS
        private void hyperlinkButtonSMS_Click(object sender, RoutedEventArgs e)
        {
            
            var sms = new SmsComposeTask();
            _currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);
            sms.Body = _message;
            var contacts = new Contacts();
            _addressTask.Show();

            //Refresh countSMS
            _currentChallenge.AddSms();
        }

        private void hyperlinkButtonFinish_Click(object sender, RoutedEventArgs e)
        {
            _currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);
            var ks = _currentChallenge.CompleteChallenge(false);
            if (ks.Key)
            {
                MessageBox.Show("Nuevo puntaje mas alto!");
            }
            else
            {
                MessageBox.Show("No has superado tu puntaje actual.");
            }
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}