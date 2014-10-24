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
        private int _actual;
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
            TextDescription.Text = ((_currentChallenge.Level == 1)
               ? AppResources.Challenge7_DescriptionTxtBlockText
               : AppResources.Challenge7_DescriptionHardTxtBlockText);

            _buttonTimer = new DispatcherTimer();
            _buttonTimer.Tick += TickButtonTimer;

            _good = 0;
            _loose = false;
            _stopTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, _currentChallenge.TimerValues)};
            _stopTimer.Tick += TickStopTimer;
            _stopTimer.Start();
        }

        private int _good;
        private void TickButtonTimer(object o, EventArgs e)
        {
            _buttonTimer.Stop();
            //Set Opacity
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
            
        }

        private void TickStopTimer(object o, EventArgs e)
        {
            _loose = true;
        }


        private void hyperlinkButtonStart_Click(object sender, RoutedEventArgs e)
        {
            StartPlayGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;

            _currentRound = 1;

            _buttonTimer.Interval = new TimeSpan(0, 0, 1);
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

        private void SetColor(SolidColorBrush mc)
        {
            Item01Rectangle.Fill = mc;
            Item02Rectangle.Fill = mc;
            Item03Rectangle.Fill = mc;
            Item04Rectangle.Fill = mc;
            Item05Rectangle.Fill = mc;

            Item01Rectangle.Opacity = 0.3;
            Item02Rectangle.Opacity = 0.3;
            Item03Rectangle.Opacity = 0.3;
            Item04Rectangle.Opacity = 0.3;
            Item05Rectangle.Opacity = 0.3;

        }

        private void UpdateRectangle(int round)
        {

            var mc = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            if ( (round < 5))
            {
                mc.Color = Color.FromArgb(255, 255, 20, 0);
            }
            else if ( round < 10)
            {
                mc.Color = Color.FromArgb(255, 255, 255, 0);
            }
            else
            {
                mc.Color = Color.FromArgb(255, 0, 138, 0);
            }
            var aux = (round%5);
            if (aux == 1)
            {
                SetColor(mc);
            }

            switch (aux)
            {
                case 1:
                    Item01Rectangle.Opacity = 1;
                    break;
                case 2:
                    Item02Rectangle.Opacity = 1;
                    break;
                case 3:
                    Item03Rectangle.Opacity = 1;
                    break;
                case 4:
                    Item04Rectangle.Opacity = 1;
                    break;
                case 0:
                    Item05Rectangle.Opacity = 1;
                    break;
                
            }
        }

        private bool _loose;

        private void hyperlinkButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            CleanButtons();
            var str = "PalyButton" + (_actual) + "HiperLink";
            if ((((HyperlinkButton) sender).Name == str) && (!_buttonTimer.IsEnabled))
            {
                _good++;
                UpdateRectangle(_currentRound);
            }
            else
            {
                _buttonTimer.Stop();
                _loose = true;
                UpdateRectangle(_currentRound);
            }
            
            _currentRound++;

            if (_loose)
            {
                _currentChallenge.CompleteChallenge(_good);
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
                _buttonTimer.Start();
                _stopTimer.Stop();
                    
                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartPlayGrid.Visibility = Visibility.Visible;
                InProgressGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                _buttonTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                _buttonTimer.Start();
            }
        }
    }
}