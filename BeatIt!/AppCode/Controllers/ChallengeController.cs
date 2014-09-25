using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BeatIt_.AppCode.Datatypes;
using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Enums;
using BeatIt_.AppCode.Interfaces.Controllers;

namespace BeatIt_.AppCode.Controllers
{
    public class ChallengeController : IChallengeController
    {
        /***************************************************************************************/
        /*                          PROPIEDADES                                                */
        /***************************************************************************************/

        // Instancia del Controlador.
        private static ChallengeController instance = null;

        private Dictionary<ChallengeType.CHALLENGE_TYPE, Challenge> chalengs;




        /***************************************************************************************/
        /*                            CONSTRUCTOR                                              */
        /***************************************************************************************/
        private ChallengeController()
        {
            this.chalengs = new Dictionary<ChallengeType.CHALLENGE_TYPE, Challenge>();

            // Para el prototipo se agregara a la lista de desafios, solo UsainBolt con un mismo estado siempre.
            // Luego, la lista de desafios asociadas a las correspondientes rondas, con sus correspondientes estados
            // se deberan levantar o bien de la persitencia local o bien del servidor.
            UsainBolt usain = new UsainBolt();

            /// Aca habria que levantar el puntaje de el desafio ub.


            this.chalengs[ChallengeType.CHALLENGE_TYPE.USAIN_BOLT] = usain;
        }




        /***************************************************************************************/
        /*                            METODOS                                                  */
        /***************************************************************************************/

        /// <summary>
        /// Si ya no existe una instancia en el sistema del controlador, la crea.
        /// </summary>
        /// <returns>La instancia unica del cotrolador en el sistema.</returns>
        public static ChallengeController getInstance()
        {
            if (ChallengeController.instance == null)
                ChallengeController.instance = new ChallengeController();

            return ChallengeController.instance;
        }




        /// <summary>
        /// Para el prototipo, va a retornar una lista con un unico dataType que es
        /// el del desafio Usain Bolt, luego habra que deteriminar, de donde se obtiene la 
        /// lista.
        /// </summary>
        /// <param name="fecha">Fecha que se utilizara para ubicar la ronda.</param>
        /// <returns>Una lista con los datos de todos los desafios correspondientes a la roda de la fecha "fecha"</returns>
        public List<DTChallenge> getListaDTChallengeActualRound(DateTime fecha)
        {
            List<DTChallenge> desafiosDeLaRonda = new List<DTChallenge>();

            try
            {
                foreach (KeyValuePair<ChallengeType.CHALLENGE_TYPE, Challenge> ch in this.chalengs)
                {
                    desafiosDeLaRonda.Add(ch.Value.getDTChallenge()); // para cada desafio que el controlador tenga, agregamos el DTChallenge 
                    // generado para el desafio en la ronda correspondiente a la fecha "fecha".
                }
            }
            catch (Exception)
            {
                // Si ocurre un excepcion que pasa? devuelve  la lista vacia?
            }

            return desafiosDeLaRonda;
        }




        /// <summary>
        /// Retorna la interface del desafio de tipo "type".
        /// </summary>
        /// <param name="type">Tipo de desafio del cual se requiere su interface.</param>
        /// <returns></returns>
        public Challenge getChallenge(ChallengeType.CHALLENGE_TYPE type)
        {
            return this.chalengs[type];
        }
    }
}
