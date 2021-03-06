﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge4
    {
        private ChallengeDetail4 _currentChallenge;
        private int _currentRound;
        private IFacadeController _ifc;
        private int _ms;
        private int[] _result;
        private DispatcherTimer _soundTimer;
        private DispatcherTimer _stopTimer;

        static SoundEffectInstance _soundEffect;

        public Challenge4()
        {
            InitializeComponent();

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

            InitChallenge();
        }

        private void InitChallenge()
        {
            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail4) _ifc.GetChallenge(4);

            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge()
                .StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();

            _soundTimer = new DispatcherTimer();
            _soundTimer.Tick += TickSoundTimer;

            _stopTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 1)};
            _stopTimer.Tick += TickStopTimer;

            _result = new int[_currentChallenge.TimerValues.Length];
        }

        private void TickSoundTimer(object o, EventArgs e)
        {
            _soundTimer.Stop();
            PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");
            _stopTimer.Start();
        }

        private void TickStopTimer(object o, EventArgs e)
        {
            _ms++;
        }

        private void PlaySound(string path)
        {
            var stream = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            var soundeffect = SoundEffect.FromStream(stream.Stream);
            if (_soundEffect != null) _soundEffect.Dispose();
            _soundEffect = soundeffect.CreateInstance();
            FrameworkDispatcher.Update();
            _soundEffect.Play();
        }

        private void hyperlinkButtonStart_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Visibility = Visibility.Collapsed;
            StopGrid.Visibility = Visibility.Visible;

            _ms = 0;
            _currentRound = 0;

            _soundTimer.Interval = new TimeSpan(0, 0, _currentChallenge.TimerValues[_currentRound]);
            _soundTimer.Start();
        }

        private void hyperlinkButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (_soundEffect != null) _soundEffect.Dispose();

            if (_soundTimer.IsEnabled)
            {
                _soundTimer.Stop();
                _result[_currentRound] = 0;
            }
            else
            {
                _stopTimer.Stop();
                _result[_currentRound] = _ms;
            }

            _ms = 0;
            _currentRound++;

            if (_currentRound == _currentChallenge.TimerValues.Length)
            {
                _currentChallenge.CompleteChallenge(_result);
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartGrid.Visibility = Visibility.Visible;
                StopGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                _soundTimer.Interval = new TimeSpan(0, 0, _currentChallenge.TimerValues[_currentRound]);
                _soundTimer.Start();
            }
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            _soundTimer.Stop();
            _stopTimer.Stop();
            if (_soundEffect != null) _soundEffect.Dispose();
            e.Cancel = false;
            base.OnBackKeyPress(e);
        }
    }
}