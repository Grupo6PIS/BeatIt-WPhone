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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Resources;
using BeatIt_.AppCode.Interfaces.Controllers;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;

/* CALLAR AL PERRO */

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge4 : PhoneApplicationPage
    {

        private UsainBolt desafio;                     // Instancia del desafio que se esta corriendo.  

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

            IChallengeController ich = ChallengeController.getInstance();
            this.desafio = (UsainBolt)ich.getChallenge(AppCode.Enums.ChallengeType.CHALLENGE_TYPE.USAIN_BOLT);

            // INICIALIZAMOS LAS ETIQUETAS DEL DETALLE DEL DESAFIO
            this.ShowST.Text = this.desafio.getDTChallenge().getStartTime().ToString(); // Ojo ver el tema de la fecha y hora (Cuando estamos en el limite de una ronda y la otra).
            this.ShowToBeat.Text = this.desafio.getPuntajeObtenido() + " pts";
            DateTime roundDate = new DateTime(2014, 9, 28, 22, 0, 0);
            this.ShowDuration.Text = getDurationString(roundDate);
        }

        private void PlaySound(string path)
        {
            StreamResourceInfo _stream = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            SoundEffect _soundeffect = SoundEffect.FromStream(_stream.Stream);
            SoundEffectInstance soundInstance = _soundeffect.CreateInstance();
            FrameworkDispatcher.Update();
            soundInstance.Play();
        }

        private void hyperlinkButtonPlaySound_Click(object sender, RoutedEventArgs e)
        {
            PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

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
    }
}