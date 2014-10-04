using System;
using System.Windows;
using System.Windows.Resources;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Threading;

/* CALLAR AL PERRO */

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge4 : PhoneApplicationPage
    {

        private ChallengeDetail4 currentChallenge;
        private IFacadeController ifc;
        private DispatcherTimer soundTimer;
        private DispatcherTimer stopTimer;
        private int ms;
        private int currentRound;
        private int[] result;

        public Challenge4()
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

            this.initChallenge();
        }

        private void initChallenge()
        {

            ifc = FacadeController.getInstance();
            this.currentChallenge = (ChallengeDetail4)ifc.getChallenge(4);

            this.PageTitle.Text = this.currentChallenge.Name;
            this.TextDescription.Text = this.currentChallenge.Description;
            this.ShowST.Text = this.currentChallenge.getDTChallenge().StartTime.ToString();
            this.ShowToBeat.Text = this.currentChallenge.State.BestScore + " pts";
            DateTime roundDate = new DateTime(2014, 9, 28, 22, 0, 0);
            this.ShowDuration.Text = this.currentChallenge.getDurationString();

            soundTimer = new DispatcherTimer();
            soundTimer.Tick += tickSoundTimer;

            stopTimer = new DispatcherTimer();
            stopTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            stopTimer.Tick += tickStopTimer;

            result = new int[currentChallenge.TimerValues.Length];
        }

        private void tickSoundTimer(object o, EventArgs e)
        {
            soundTimer.Stop();
            PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");
            stopTimer.Start();
        }

        private void tickStopTimer(object o, EventArgs e)
        {
            ms++;
        }

        private void PlaySound(string path)
        {
            StreamResourceInfo _stream = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            SoundEffect _soundeffect = SoundEffect.FromStream(_stream.Stream);
            SoundEffectInstance soundInstance = _soundeffect.CreateInstance();
            FrameworkDispatcher.Update();
            soundInstance.Play();
        }

        private void hyperlinkButtonStart_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Visibility = System.Windows.Visibility.Collapsed;
            StopGrid.Visibility = System.Windows.Visibility.Visible;

            ms = 0;
            currentRound = 0;

            soundTimer.Interval = new TimeSpan(0, 0, currentChallenge.TimerValues[currentRound]);
            soundTimer.Start();
        }

        private void hyperlinkButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (soundTimer.IsEnabled)
            {
                soundTimer.Stop();
                result[currentRound] = 0;
            }
            else 
            {
                stopTimer.Stop();
                result[currentRound] = ms;
            }

            ms = 0;
            currentRound++;

            if (currentRound == currentChallenge.TimerValues.Length)
            {
                this.currentChallenge.completeChallenge(result);
                this.ShowToBeat.Text = this.currentChallenge.State.BestScore + " pts";

                MessageBox.Show("El desafio ha finalizado, has obtenido " + this.currentChallenge.State.BestScore + " puntos.");

                Uri uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartGrid.Visibility = System.Windows.Visibility.Visible;
                StopGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                soundTimer.Interval = new TimeSpan(0, 0, currentChallenge.TimerValues[currentRound]);
                soundTimer.Start();
            }
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            soundTimer.Stop();
            stopTimer.Stop();
            e.Cancel = false;
            base.OnBackKeyPress(e);
        }
    }
}