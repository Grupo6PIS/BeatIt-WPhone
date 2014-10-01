using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Threading;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


/* DESPERTAME A TIEMPO */

namespace BeatIt_.AppCode.Pages
{    
    public partial class Challenge2 : PhoneApplicationPage
    {
        const int FREE_TIME = 5;

        private ChallengeDetail2 currentChallenge;    // Instancia del desafio.
        private IFacadeController ifc;                // Interface del controlador Fachada.
         
        private int aciertos;                         // Cantidad de haciertos.

        private int[] seconsToWakeMeUp;
        private DateTime finishTime;
        private DateTime startTime;

        private DispatcherTimer timer;
        private Accelerometer acelerometro;

        private Stopwatch stopwatch = new Stopwatch(); //un objeto tipo Cronómetro

        public Challenge2() 
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


            this.inicializar();
        }


        private void inicializar()
        {
            try
            {
                if (!Accelerometer.IsSupported)
                    throw new Exception("Lamentablemente su dispositivo no tiene las caracteristicas necesarias para jugar este desafio.");

                // OBTENEMOS LA INSTANCIA DEL DESAFIO.
                /* hay que prolijear esto con una factory */
                ifc = FacadeController.getInstance();
                this.currentChallenge = (ChallengeDetail2)ifc.getChallenge(2);

                if (this.currentChallenge.State.getCurrentAttempt() == this.currentChallenge.MaxAttempt)
                    throw new Exception("Ya ha realizado el número máximo de intentos en la ronda actual.");

                // INICIALIZAMOS LAS ETIQUETAS DEL DETALLE DEL DESAFIO
                this.ShowST.Text = this.currentChallenge.getDTChallenge().getStartTime().ToString(); // Tiempo de iniciado el desafio.
                this.ShowToBeat.Text = this.currentChallenge.State.getScore() + " pts";              // Puntaje a vencer.
                this.ShowDuration.Text = this.currentChallenge.getDurationString();                  // Tiempo retante para realizar el desafio.   
                this.ShowTime.Text = "---";                                                          // Tiempo transcurrido.

                // IINICIALIZAMOS EL TIMER
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += timerTick;

                // INICIALIZAMOS EL ACELEROMETRO.
                this.acelerometro = new Accelerometer();

                // ACIERTOS            
                this.aciertos = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                this.Dispatcher.BeginInvoke(delegate()
                {
                    Uri uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);                    
                });                
            }
        }


        private void timerTick(object sender, EventArgs e)
        {
            TimeSpan tiempoTranscurrido = this.stopwatch.Elapsed;
            DateTime aux = this.startTime.Add(tiempoTranscurrido);

            // Si es tiempo de desaparecer el timer, lo borramos.
            if (tiempoTranscurrido.TotalSeconds <= FREE_TIME)
                this.ShowTime.Text = Math.Round((this.seconsToWakeMeUp[aciertos] + FREE_TIME - tiempoTranscurrido.TotalSeconds), 0).ToString();
            else
                this.ShowTime.Text = "Wake Me Up!";


            if (aux > this.finishTime && (aux - this.finishTime).TotalMilliseconds > 500) // Si me pase del tiempo y no desperte a nadie, termina el juego.
            {
                this.timer.Stop();
                this.acelerometro.Stop();
                this.stopwatch.Stop();

                MessageBox.Show("El desafio ha finalizado, ha obtenido " + this.currentChallenge.calculatPuntaje(this.aciertos).ToString() + " puntos.");

                this.currentChallenge.completeChallenge(this.aciertos);
                
                Uri uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
        }


        private void tickTemp(object sender, EventArgs e)
        {

        }


        // VER CUAL ES LA IDEA DE ESTA FUNCION????????????????????????????????????????????????
        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }


        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            this.StartPlayGrid.Visibility = Visibility.Collapsed;
            this.InProgressGrid.Visibility = Visibility.Visible;

            this.seconsToWakeMeUp = this.currentChallenge.getSeconsToWakeMeUp();
            this.startTime = System.DateTime.Now;
            this.finishTime = this.startTime.Add(new TimeSpan(0, 0, this.seconsToWakeMeUp[0] + FREE_TIME));

            // INICIALIZAMOS EL TIMER.
            this.timer.Start();
            this.stopwatch.Start();

            this.acelerometro.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(acelerometro_CurrentValueChanged);
            this.acelerometro.TimeBetweenUpdates = new TimeSpan(0,0,0,0,100);
            this.acelerometro.Start();
        }

        
        /// <summary>
        /// Este metodo es llamado cuando existe un cambio en los datos del acelerometro, en el mismo se determinara
        /// si la aceleración captada es considerable, y en dicho caso:
        /// Si (Me desperto a tiempo)
        ///     aciertos ++
        ///     Si (no quedan por despertar)
        ///         finalizar
        ///     else
        ///         volver a inciar con el siguiente a despertar.
        /// else
        ///     finalizar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acelerometro_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {

            //Transladamos los datos a recoger del BackgroundThread y los enviamos al MainThread
            Dispatcher.BeginInvoke(() =>
            {
                Microsoft.Xna.Framework.Vector3 vector = e.SensorReading.Acceleration;

                if ((Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 1)) > 2) && this.stopwatch.ElapsedMilliseconds > FREE_TIME * 1000) // Si Sacudi el celular lo suficiente.
                {
                    this.acelerometro.Stop();
                    this.timer.Stop();
                    this.stopwatch.Stop();

                    PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");                    

                    bool finalizar = false;

                    if (Math.Abs(FREE_TIME * 1000 + this.seconsToWakeMeUp[this.aciertos] * 1000 - this.stopwatch.ElapsedMilliseconds) <= 500) // Si me desperto a tiempo.
                    {
                        this.aciertos++;
                        if (aciertos == this.seconsToWakeMeUp.Length)
                            finalizar = true;
                        else
                        {
                            this.startTime = System.DateTime.Now;
                            this.finishTime = this.startTime.Add(new TimeSpan(0, 0, this.seconsToWakeMeUp[aciertos] + FREE_TIME));
                            this.timer.Start();
                            this.stopwatch.Reset();
                            this.stopwatch.Start();
                            this.acelerometro.Start();
                        }
                    }
                    else
                        finalizar = true;

                    if (finalizar) // Si el desafio finalizo, ya sea porque se equivoco al despertar o porque acerto en todas las despertadas.
                    {
                        this.currentChallenge.completeChallenge(this.aciertos);

                        MessageBox.Show("El desafio ha finalizado, ha obtenido " + this.currentChallenge.calculatPuntaje(this.aciertos).ToString() + " puntos.");                                                

                        Uri uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }
                }
            });
        }


        private void PlaySound(string path)
        {
            StreamResourceInfo _stream = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            SoundEffect _soundeffect = SoundEffect.FromStream(_stream.Stream);
            SoundEffectInstance soundInstance = _soundeffect.CreateInstance();
            FrameworkDispatcher.Update();
            soundInstance.Play();
        }
    }
}