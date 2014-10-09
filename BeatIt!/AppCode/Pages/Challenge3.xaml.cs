using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
using Facebook;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

/* INVITA A TUS AMIGOS */

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge3
    {
        //Private Variables
        private readonly IFacadeController _ifc;
        private readonly string _message = AppResources.Challenge3_Message;
        private readonly PhoneNumberChooserTask _phoneNumberChooserTask;
        private ChallengeDetail3 _currentChallenge;

        public Challenge3()
        {
            InitializeComponent();

            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail3) _ifc.getChallenge(3);
            _ifc.setCurrentChallenge(_currentChallenge);

            _phoneNumberChooserTask = new PhoneNumberChooserTask();
            _phoneNumberChooserTask.Completed += PhoneNumberChooserTask_Completed;

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

            ShowST.Text = _currentChallenge.getDTChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
                // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
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
            _currentChallenge = (ChallengeDetail3) _ifc.getChallenge(3);

            fb.PostCompleted +=
                (o, args) => Dispatcher.BeginInvoke(() => MessageBox.Show("Message Posted successfully"));

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
            _phoneNumberChooserTask.Show();

            //var sms = new SmsComposeTask();
            //_currentChallenge = (ChallengeDetail3)_ifc.getChallenge(3);
            //sms.Body = _message;
            //var contacts = new Contacts();
            //_addressTask.Show();

            ////Refresh countSMS
            //_currentChallenge.AddSms();
        }

        private void hyperlinkButtonFinish_Click(object sender, RoutedEventArgs e)
        {
            _currentChallenge = (ChallengeDetail3) _ifc.getChallenge(3);
            KeyValuePair<bool, int> ks = _currentChallenge.CompleteChallenge(false);
            MessageBox.Show(ks.Key ? "Nuevo puntaje mas alto!" : "No has superado tu puntaje actual.");
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void PhoneNumberChooserTask_Completed(object sender, PhoneNumberResult e)
        {
            if (e.TaskResult != TaskResult.OK) return;

            //MessageBox.Show("The phone number for " + e.DisplayName + " is " + e.PhoneNumber);

            var smsComposeTask = new SmsComposeTask
            {
                To = e.PhoneNumber,
                Body = _message
            };
            smsComposeTask.Show();

            //Refresh countSMS
            _currentChallenge.AddSms();
        }
    }
}