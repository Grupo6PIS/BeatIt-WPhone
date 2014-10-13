using Microsoft.Phone.Controls;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Controllers;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using System;
using System.Windows.Controls;
using System.Windows;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge5 : PhoneApplicationPage
    {
        private int challengeID = 5,
                    timeTop = 60; // segundos

        private ChallengeDetail5 challenge;
        private int timeCounter, collisionCounter;
        private double canvasWidth, 
            canvasHeight, 
            positionRedX = 0, 
            positionRedY = 0,
            positionBlackX =0,
            positionBlackY = 0, fraccion;

        private DispatcherTimer timer;
        private Accelerometer acelerometer;
        private Random randomNumber;

        public Challenge5()
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

            initChallenge();
            
        }

        private void initChallenge()
        {
            challenge =(ChallengeDetail5) FacadeController.GetInstance().getChallenge(challengeID);
            randomNumber = new Random();
        }

        private void StartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!Accelerometer.IsSupported)
            {
                MessageBox.Show("Lamentablemente su dispositivo no tiene las caracteristicas necesarias para jugar este desafio.");
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
                return;
            }

            if (challenge.State.CurrentAttempt == challenge.MaxAttempt)
            {
                MessageBox.Show("Ya ha realizado el número máximo de intentos en la ronda actual.");
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });

                return;
            }

            timeCounter = timeTop;
            collisionCounter = 0;

            fraccion = challenge.Level == 2 ? 0.7 : 0.9;

            canvasHeight = canvasPanel.ActualHeight;
            canvasWidth = canvasPanel.Width;
            
            acelerometer = new Accelerometer();
            acelerometer.CurrentValueChanged += acelerometer_ReadingChanged;
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;

            acelerometer.Start();
            timer.Start();

            ContentPanel.Visibility = System.Windows.Visibility.Visible;

            generateRandomPosition();
        }


        void timer_Tick(object sender, System.EventArgs e)
        {
            timeCounter--;

            if (timeCounter < 0)
            {
                ContentPanel.Visibility = System.Windows.Visibility.Collapsed;

                timeCounter = 0;
                timer.Stop();
                acelerometer.Stop();

                timer = null;
                acelerometer = null;

                challenge.ChanllengeComplete(collisionCounter);
                MessageBox.Show("Time finished!! You got " + challenge.State.BestScore + " points");

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

            }
            else
            {
                timerLabel.Text = timeCounter + "s";
            }
        }

        void acelerometer_ReadingChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Dispatcher.BeginInvoke(() => { updatePositions(e.SensorReading.Acceleration.X * 9.8, e.SensorReading.Acceleration.Y * 9.8); });

        }

        private void updatePositions(double x,double y ){

            positionBlackX += x;
            positionBlackY -= y;

            if (positionBlackX < 0)
            {
                positionBlackX = 0;
            }
            else if (positionBlackX > (canvasPanel.ActualWidth - blackBall.Width))
            {
                positionBlackX = canvasPanel.ActualWidth - blackBall.Width;
            }

            if (positionBlackY < 0)
            {
                positionBlackY = 0;
            }
            else if (positionBlackY > (canvasPanel.ActualHeight - blackBall.Height))
            {
                positionBlackY = canvasPanel.ActualHeight - blackBall.Height;
            }

            Canvas.SetLeft(blackBall, positionBlackX);

            Canvas.SetTop(blackBall, positionBlackY);

            if (collision())
            {
                if (blackBall.Width > 10 && randomNumber.Next(0,2) == 1)
                {
                    blackBall.Width = blackBall.Width * fraccion;
                    blackBall.Height = blackBall.Height * fraccion;
                }

                collisionCounter++;
                counterLabel.Text = collisionCounter.ToString();
                generateRandomPosition();
            }
        }


        private bool collision()
        {
            var x = positionBlackX + (blackBall.Width / 2);
            var y = positionBlackY + (blackBall.Height / 2);

            var sumRadius = (blackBall.Width + redBall.Width) / 2;

            var module = Math.Sqrt((x - positionRedX)*(x - positionRedX) + (y - positionRedY)*(y - positionRedY) );

            return (module < sumRadius );
        }

        private void generateRandomPosition()
        {
            if (canvasPanel.ActualWidth == 0)
            {
                canvasPanel.UpdateLayout();
            }

            var x = randomNumber.Next(0, (int)(canvasPanel.ActualWidth - redBall.Width));
            var y = randomNumber.Next(0, (int)(canvasPanel.ActualHeight - redBall.Height));

            positionRedX = x + (redBall.Width / 2);
            positionRedY = y + (redBall.Height / 2);

            Canvas.SetLeft(redBall, x);
            Canvas.SetTop(redBall, y);
        }
    }
}