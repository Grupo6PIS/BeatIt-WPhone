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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge9
    {
        private ChallengeDetail9 _currentChallenge;
        private IFacadeController _ifc;
        private DispatcherTimer _timer;
        private int _currentRound;
        private int[] _result;

        static SoundEffectInstance _soundEffect;

        public Challenge9()
        {
            InitializeComponent();

            var navigateInTransition = new NavigationInTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn }
            };

            var navigateOutTransition = new NavigationOutTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut }
            };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            Option1Button.Style = (Style)Application.Current.Resources["HyperlinkButtonStyle"];
            Option2Button.Style = (Style)Application.Current.Resources["HyperlinkButtonStyle"];
            Option3Button.Style = (Style)Application.Current.Resources["HyperlinkButtonStyle"];

            InitChallenge();
        }

        private void InitChallenge()
        {
            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail9)_ifc.GetChallenge(9);

            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge()
                .StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();

            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _timer.Tick += TickTimer;

            _result = new int[5];
        }

        private void NextStep(bool error)
        {
            bool songError;
            _timer.Stop();
            if (_soundEffect != null) _soundEffect.Dispose();

            if (error)
            {
                songError = true;
                _result[_currentRound] = 0;   
            }
            else
            {
                songError = false;
                _result[_currentRound] = 40;
            }
            _currentRound++;
            UpdateRectangle(_currentRound, songError);

            if (_currentRound == 5)
            {
                _currentChallenge.CompleteChallenge(_result);
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

                StartGrid.Visibility = Visibility.Visible;
                InProgressGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                UpdateButtons();

                ProgressBar.Value = 0;
                ProgressBar.Minimum = 0;
                ProgressBar.Maximum = _currentChallenge.TimerValue;

                _timer.Start();

                PlaySound("/BeatIt!;component/Sounds/" + _currentChallenge.Songs[_currentRound].SongName);
            }
        }

        private void TickTimer(object o, EventArgs e)
        {
            ProgressBar.Value = ProgressBar.Value + 1;

            if ((int)ProgressBar.Value == _currentChallenge.TimerValue)
            {
                NextStep(true);
            }
        }

        private void hyperlinkButtonStart_Click(object sender, RoutedEventArgs e)
        {
            _currentRound = 0;

            UpdateButtons();

            StartGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;

            ProgressBar.Value = 0;
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = _currentChallenge.TimerValue;

            _timer.Start();

            PlaySound("/BeatIt!;component/Sounds/" + _currentChallenge.Songs[_currentRound].SongName);
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void Option1Button_Click(object sender, RoutedEventArgs e)
        {

            NextStep(_currentChallenge.Songs[_currentRound].SelectedIndex != 0);
        }

        private void Option2Button_Click(object sender, RoutedEventArgs e)
        {
            NextStep(_currentChallenge.Songs[_currentRound].SelectedIndex != 1);
        }

        private void Option3Button_Click(object sender, RoutedEventArgs e)
        {
            NextStep(_currentChallenge.Songs[_currentRound].SelectedIndex != 2);
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

        private void UpdateRectangle(int round, bool error)
        {

            var mySolidColorBrush = new SolidColorBrush
            {
                Color = error
                    ? System.Windows.Media.Color.FromArgb(255, 255, 20, 0)
                    : System.Windows.Media.Color.FromArgb(255, 0, 138, 0)
            };
            switch (round)
            {
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
            }
        }

        private void UpdateButtons()
        {
            Option1Button.Content = _currentChallenge.Songs[_currentRound].OptionsName[0];
            Option2Button.Content = _currentChallenge.Songs[_currentRound].OptionsName[1];
            Option3Button.Content = _currentChallenge.Songs[_currentRound].OptionsName[2];
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            _timer.Stop();
            if (_soundEffect != null) _soundEffect.Dispose();
            e.Cancel = false;
            base.OnBackKeyPress(e);
        }
    }
}