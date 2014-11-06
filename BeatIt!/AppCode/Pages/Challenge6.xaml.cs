
﻿using System.Globalization;
﻿using System.Windows.Navigation;
﻿using BeatIt_.Resources;
﻿using Microsoft.Phone.Controls;
﻿using System;
using System.Windows;
using Microsoft.Devices.Sensors;
using System.Diagnostics;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
﻿using Microsoft.Xna.Framework;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge6
    {
        private ChallengeDetail6 _currentChallenge;    
        private IFacadeController _ifc;               

        private bool _isFlying;


        private Accelerometer _accelerometer;

        private readonly Stopwatch _stopwatch = new Stopwatch(); 

        public Challenge6()
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

            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge()
                .StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();

            Inicializar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _stopwatch.Reset();
            base.OnNavigatedTo(e);
        }

        private void Inicializar()
        {
            bool finish = false;

            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail6)_ifc.GetChallenge(6);


            if (!Accelerometer.IsSupported)
            {
                MessageBox.Show(AppResources.Challenge_NotSuported);
                finish = true;
            }

           
            if (_currentChallenge.State.CurrentAttempt >= _currentChallenge.MaxAttempt)
            {
               
                _currentChallenge.State.CurrentAttempt = _currentChallenge.MaxAttempt;
                FacadeController.GetInstance().SaveState(_currentChallenge.State);
                MessageBox.Show(AppResources.Challenge_MaxAttemptsExeeded);
                finish = true;
            }

           
            if (finish)
            {
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }


            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);

            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();
            

           
            _accelerometer = new Accelerometer();

           
            _isFlying = false;
        }

        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            StartPlayGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;

            MessageBox.Show(AppResources.Challenge6_Alert);

            _stopwatch.Reset();

            _accelerometer.CurrentValueChanged += _Accelerometer_CurrentValueChanged;
            _accelerometer.TimeBetweenUpdates = new TimeSpan(0, 0, 0, 0, 50);
            _accelerometer.Start();
        }


       
        private void _Accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => ProcessAccelerometerReading(e));            
        }


       
        private void ProcessAccelerometerReading(SensorReadingEventArgs<AccelerometerReading> e)
        {
            var vector = e.SensorReading.Acceleration;
            var normaVector =
                Math.Abs(Math.Round(Math.Sqrt(vector.X*vector.X + vector.Y*vector.Y + vector.Z*vector.Z), 3));

            if ((!_isFlying) && (normaVector < 0.2))
               
            {
                _isFlying = true;
                _stopwatch.Start();
            }
            else
            {
                if (_isFlying && (Math.Abs(normaVector - 1) < 0.2))
                   
                {
                    _stopwatch.Stop();
                    _accelerometer.Stop();

                    var tiempo = Convert.ToDouble(_stopwatch.ElapsedMilliseconds)/1000;

                    _isFlying = false;

                    StartPlayGrid.Visibility = Visibility.Visible;

                    _currentChallenge.CompleteChallenge(tiempo);

                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                }
            }
        }
    }
}