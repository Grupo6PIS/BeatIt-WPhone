using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
using Microsoft.Phone.Controls;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge7
    {
        private ChallengeDetail7 _currentChallenge;
        private int _currentRound;
        private IFacadeController _ifc;
        private int _ms;
        private int _actual;
        private int[] _result;
        private DispatcherTimer _buttonTimer;
        private DispatcherTimer _stopTimer;

        public Challenge7()
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
            _currentChallenge = (ChallengeDetail7) _ifc.getChallenge(7);
            _ifc.setCurrentChallenge(_currentChallenge);

            PageTitle.Text = _currentChallenge.Name;
            //TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();
            textDescription.Text = ((_currentChallenge.Level == 1)
               ? AppResources.Challenge7_DescriptionTxtBlockText
               : AppResources.Challenge7_DescriptionHardTxtBlockText);

            _buttonTimer = new DispatcherTimer();
            _buttonTimer.Tick += TickButtonTimer;

            _stopTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 1)};
            _stopTimer.Tick += TickStopTimer;

            _result = new int[_currentChallenge.TimerValues.Length];
        }

        private void TickButtonTimer(object o, EventArgs e)
        {
            _buttonTimer.Stop();
            //SET OPACITY
            var selected = (DateTime.Now.Millisecond) % 12;
            _actual = selected;
            switch (selected)
            {
                case 0:
                    Paly0Rectangle.Opacity = 1;
                    break;
                case 1:
                    Paly1Rectangle.Opacity = 1;
                    break;
                case 2:
                    Paly2Rectangle.Opacity = 1;
                    break;
                case 3:
                    Paly3Rectangle.Opacity = 1;
                    break;
                case 4:
                    Paly4Rectangle.Opacity = 1;
                    break;
                case 5:
                    Paly5Rectangle.Opacity = 1;
                    break;
                case 6:
                    Paly6Rectangle.Opacity = 1;
                    break;
                case 7:
                    Paly7Rectangle.Opacity = 1;
                    break;
                case 8:
                    Paly8Rectangle.Opacity = 1;
                    break;
                case 9:
                    Paly9Rectangle.Opacity = 1;
                    break;
                case 10:
                    Paly10Rectangle.Opacity = 1;
                    break;
                default:
                    Paly11Rectangle.Opacity = 1;
                    break;
            }
            _stopTimer.Start();
        }

        private void TickStopTimer(object o, EventArgs e)
        {
            _ms++;
        }


        private void hyperlinkButtonStart_Click(object sender, RoutedEventArgs e)
        {
            StartPlayGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;

            _currentRound = 0;
            _ms = 0;

            _buttonTimer.Interval = new TimeSpan(0, 0, _currentChallenge.TimerValues[_currentRound]);
            _buttonTimer.Start();
        }

        private void CleanButtons()
        {
            Paly0Rectangle.Opacity = 0.3;
            Paly1Rectangle.Opacity = 0.3;
            Paly2Rectangle.Opacity = 0.3;
            Paly3Rectangle.Opacity = 0.3;
            Paly4Rectangle.Opacity = 0.3;
            Paly5Rectangle.Opacity = 0.3;
            Paly6Rectangle.Opacity = 0.3;
            Paly7Rectangle.Opacity = 0.3;
            Paly8Rectangle.Opacity = 0.3;
            Paly9Rectangle.Opacity = 0.3;
            Paly10Rectangle.Opacity = 0.3;
            Paly11Rectangle.Opacity = 0.3;
        }

        private void Cleanitems()
        {
            var mySolidColorBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            Item0Rectangle.Fill = mySolidColorBrush;
            Item1Rectangle.Fill = mySolidColorBrush;
            Item2Rectangle.Fill = mySolidColorBrush;
            Item3Rectangle.Fill = mySolidColorBrush;
            Item4Rectangle.Fill = mySolidColorBrush;
            Item5Rectangle.Fill = mySolidColorBrush;
            Item6Rectangle.Fill = mySolidColorBrush;
            Item7Rectangle.Fill = mySolidColorBrush;
            Item8Rectangle.Fill = mySolidColorBrush;
            Item9Rectangle.Fill = mySolidColorBrush;
            Item10Rectangle.Fill = mySolidColorBrush;
            Item11Rectangle.Fill = mySolidColorBrush;
            Item12Rectangle.Fill = mySolidColorBrush;
            Item13Rectangle.Fill = mySolidColorBrush;
            Item14Rectangle.Fill = mySolidColorBrush;
        }

        private void UpdateRectangle(int round, bool error)
        {

            var aux = round;
            var mySolidColorBrush = new SolidColorBrush
            {
                Color = error
                    ? Color.FromArgb(255, 255, 20, 0)
                    : Color.FromArgb(255, 0, 138, 0)
            };
            if ((aux % 15) == 0)
            {
                Cleanitems();
            }
            if (aux >= 15)
            {
                aux -= 15;
            }
            switch (aux)
            {
                case 0:
                    Item0Rectangle.Fill = mySolidColorBrush;
                    break;
                case 1:
                    Item1Rectangle.Fill = mySolidColorBrush;
                    break;
                case 2:
                    Item2Rectangle.Fill = mySolidColorBrush;
                    break;
                case 3:
                    Item3Rectangle.Fill = mySolidColorBrush;
                    break;
                case 4:
                    Item4Rectangle.Fill = mySolidColorBrush;
                    break;
                case 5:
                    Item5Rectangle.Fill = mySolidColorBrush;
                    break;
                case 6:
                    Item6Rectangle.Fill = mySolidColorBrush;
                    break;
                case 7:
                    Item7Rectangle.Fill = mySolidColorBrush;
                    break;
                case 8:
                    Item8Rectangle.Fill = mySolidColorBrush;
                    break;
                case 9:
                    Item9Rectangle.Fill = mySolidColorBrush;
                    break;
                case 10:
                    Item10Rectangle.Fill = mySolidColorBrush;
                    break;
                case 11:
                    Item11Rectangle.Fill = mySolidColorBrush;
                    break;
                case 12:
                    Item12Rectangle.Fill = mySolidColorBrush;
                    break;
                case 13:
                    Item13Rectangle.Fill = mySolidColorBrush;
                    break;
                case 14:
                    Item14Rectangle.Fill = mySolidColorBrush;
                    break;
            }
        }

        private void hyperlinkButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            CleanButtons();
            var str = "PalyButton" + (_actual) + "HiperLink";
            if ((((HyperlinkButton) sender).Name == str) && (!_buttonTimer.IsEnabled))
            {
                _stopTimer.Stop();
                _result[_currentRound] = _ms;
                UpdateRectangle(_currentRound, false);
            }
            else
            {
                _buttonTimer.Stop();
                _result[_currentRound] = 0;
                UpdateRectangle(_currentRound, true);
            }
            
            _ms = 0;
            _currentRound++;

            if (_currentRound == _currentChallenge.TimerValues.Length)
            {
                _currentChallenge.CompleteChallenge(_result);
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";

                    
                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartPlayGrid.Visibility = Visibility.Visible;
                InProgressGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                _buttonTimer.Interval = new TimeSpan(0, 0, _currentChallenge.TimerValues[_currentRound]);
                _buttonTimer.Start();
            }
        }
    }
}