using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatIt_.AppCode.Datatypes;
using BeatIt_.AppCode.Enums;
using BeatIt_.AppCode.Classes;

namespace BeatIt_.AppCode.Interfaces.Controllers
{
    public interface IChallengeController
    {
        /// <summary>
        /// Para el prototipo, va a retornar una lista con un unico dataType que es
        /// el del desafio Usain Bolt, luego habra que deteriminar, de donde se obtiene la 
        /// lista.
        /// </summary>
        /// <param name="fecha">Fecha que se utilizara para ubicar la ronda.</param>
        /// <returns>Una lista con los datos de todos los desafios correspondientes a la roda de la fecha "fecha"</returns>
        List<DTChallenge> getListaDTChallengeActualRound(DateTime fecha);




        /// <summary>
        /// Retorna la interface del desafio de tipo "type".
        /// </summary>
        /// <param name="type">Tipo de desafio del cual se requiere su interface.</param>
        /// <returns></returns>
        Challenge getChallenge(ChallengeType.CHALLENGE_TYPE type);
    }
}
