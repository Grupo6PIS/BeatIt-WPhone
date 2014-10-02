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
        private bool soundPlayed;
        private int ms;

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

            this.ShowST.Text = this.currentChallenge.getDTChallenge().getStartTime().ToString();
            this.ShowToBeat.Text = this.currentChallenge.State.getScore() + " pts";
            DateTime roundDate = new DateTime(2014, 9, 28, 22, 0, 0);
            this.ShowDuration.Text = getDurationString(roundDate);

            soundTimer = new DispatcherTimer();
            soundTimer.Interval = new TimeSpan(0, 0, 3);
            soundTimer.Tick += tickSoundTimer;

            stopTimer = new DispatcherTimer();
            stopTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            stopTimer.Tick += tickStopTimer;
        }

        private void tickSoundTimer(object o, EventArgs e)
        {
            soundTimer.Stop();
            soundPlayed = true;
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

            soundPlayed = false;
            ms = 0;

            soundTimer.Start();
        }

        private void hyperlinkButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (soundPlayed)
            {
                stopTimer.Stop();
                this.currentChallenge.completeChallenge(false, ms);
                this.ShowToBeat.Text = this.currentChallenge.State.getScore() + " pts";
                MessageBox.Show("Desafio completado!");
            }
            else 
            {
                soundTimer.Stop();
                this.currentChallenge.completeChallenge(true, 0);
                MessageBox.Show("Desafio no completado.");
            }

            StartGrid.Visibility = System.Windows.Visibility.Visible;
            StopGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private string getDurationString(DateTime roundDate)
        {
            String result = "";
            DateTime dateToday = DateTime.Now;
            TimeSpan dif = roundDate - dateToday;
            int days = dif.Days;
            if (days > 0)
            {
                result = days + " dias";
            }
            else
            {
                int hours = dif.Hours % 24;
                if (hours > 0)
                {
                    result = hours + " horas";
                }
                else
                {
                    int minutes = dif.Minutes % 60;
                    if (minutes > 0)
                    {
                        result = minutes + " minutos";
                    }
                    else
                    {
                        result = "Menos de un minuto!!";
                    }
                }
            }
            return result;
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