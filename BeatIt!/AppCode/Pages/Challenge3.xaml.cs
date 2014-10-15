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

            ShowST.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
                // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
            ShowToBeat.Text = _currentChallenge.State.BestScore + " pts";
            ShowDuration.Text = _currentChallenge.GetDurationString();
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
            _currentChallenge.AddFacebook();
            var fb = new FacebookClient(_ifc.getCurrentUser().FbAccessToken);
            _currentChallenge = (ChallengeDetail3) _ifc.getChallenge(3);

            fb.PostCompleted +=
                (o, args) => Dispatcher.BeginInvoke(() => MessageBox.Show("Message Posted successfully"));

            var parameters = new Dictionary<string, object>();
            parameters["message"] = _message;
            fb.PostAsync("me/feed", parameters);

            //Refresh countFacebook
            
        }


        //onClick send SMS
        private void hyperlinkButtonSMS_Click(object sender, RoutedEventArgs e)
        {
            _phoneNumberChooserTask.Show();
        }

        private void hyperlinkButtonFinish_Click(object sender, RoutedEventArgs e)
        {
            _currentChallenge = (ChallengeDetail3) _ifc.getChallenge(3);
            var ks = _currentChallenge.CompleteChallenge(false);
            MessageBox.Show(ks.Key ? "Nuevo puntaje mas alto!" : "No has superado tu puntaje actual.");
            var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }



        private void PhoneNumberChooserTask_Completed(object sender, PhoneNumberResult e)
        {
            //Refresh countSMS
            _currentChallenge.AddSms();
            if (e.TaskResult != TaskResult.OK) return;

            var smsComposeTask = new SmsComposeTask
            {
                To = e.PhoneNumber,
                Body = _message
            };
            smsComposeTask.Show();

            
        }
    }
}