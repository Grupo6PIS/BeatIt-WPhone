using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge8
    {
        private ChallengeDetail8 _currentChallenge;
        private IFacadeController _ifc;
        private DispatcherTimer _timer;
        private int _seconds;
        private int _hits;
        private Random _rnd;
        private int _colorNameIndex;
        private int _colorHexIndex;

        public Challenge8()
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
            _currentChallenge = (ChallengeDetail8)_ifc.getChallenge(8);

            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge()
                .StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();

            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _timer.Tick += TickTimer;

            _rnd = new Random();
        }

        private void TickTimer(object o, EventArgs e)
        {
            _seconds--;
            if (_seconds == 0)
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                }
                _currentChallenge.CompleteChallenge(_hits);
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartGrid.Visibility = Visibility.Visible;
                InProgressGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                UpdateTimer();
            }
        }

        private void UpdateTimer()
        {
            TimerTextBlock.Text = string.Format("{0:0}:{1:00}", _seconds / 60, _seconds % 60);
        }

        private void UpdateColor()
        {
            _colorNameIndex = _rnd.Next(_currentChallenge.ColorNamesStrings.Length);
            _colorHexIndex = _rnd.Next(_currentChallenge.ColorHexStrings.Length);

            ColorNameRectangle.Fill = GetColorFromHexa(_currentChallenge.ColorHexStrings[_colorHexIndex]);
            ColorNameTextBlock.Text = _currentChallenge.ColorNamesStrings[_colorNameIndex];
        }

        private void hyperlinkButtonStart_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;

            HitsTextBlock.Text = "0";

            _seconds = _currentChallenge.TimerValue;
            UpdateColor();
            UpdateTimer();
            _timer.Start();
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            if (_colorNameIndex == _colorHexIndex)
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                }
                _currentChallenge.CompleteChallenge(_hits);
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartGrid.Visibility = Visibility.Visible;
                InProgressGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                _hits++;
                HitsTextBlock.Text = _hits.ToString(CultureInfo.InvariantCulture);
                UpdateColor();
            }
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            if (_colorNameIndex == _colorHexIndex)
            {
                _hits++;
                HitsTextBlock.Text = _hits.ToString(CultureInfo.InvariantCulture);
                UpdateColor();
            }
            else
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                }
                _currentChallenge.CompleteChallenge(_hits);
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartGrid.Visibility = Visibility.Visible;
                InProgressGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                    )
                );
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            _timer.Stop();
            e.Cancel = false;
            base.OnBackKeyPress(e);
        }
    }
}