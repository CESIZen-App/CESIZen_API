using CESIZen_API.API.ConfigRespiration.Models;

namespace CESIZen_API.API.Exercice.Factory.Implementations
{
    /// <summary>
    /// Factory pour la technique de cohérence cardiaque 4-6.
    /// Inspiration : 4s / Apnée : 0s / Expiration : 6s
    /// </summary>
    public class Technique46Factory : IExerciceRespirationFactory
    {
        public TypeExerciceRespiration Type => TypeExerciceRespiration.Technique46;

        public ConfigsRespirationModel Create(int exerciceId) => new()
        {
            ExerciceId = exerciceId,
            TempsInspire = 4,
            TempsPause = 0,
            TempsExpire = 6,
            NombreCycles = 5
        };
    }
}
