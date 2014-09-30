using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;

/* INVITA A TUS AMIGOS */

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge3 : PhoneApplicationPage
    {
        public Challenge3()
        {
            InitializeComponent();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeIn};
            navigateInTransition.Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeIn};

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeOut};
            navigateOutTransition.Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeOut};
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
        }
        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            this.StartPlayGrid.Visibility = Visibility.Collapsed;
            this.InProgressGrid.Visibility = Visibility.Visible;

            
        }
    }
}