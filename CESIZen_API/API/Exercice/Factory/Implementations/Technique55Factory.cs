using CESIZen_API.API.ConfigRespiration.Models;

namespace CESIZen_API.API.Exercice.Factory.Implementations
{
    /// <summary>
    /// Factory pour la technique de cohérence cardiaque 5-5.
    /// Inspiration : 5s / Apnée : 0s / Expiration : 5s
    /// </summary>
    public class Technique55Factory : IExerciceRespirationFactory
    {
        public TypeExerciceRespiration Type => TypeExerciceRespiration.Technique55;

        public ConfigsRespirationModel Create(int exerciceId) => new()
        {
            ExerciceId = exerciceId,
            TempsInspire = 5,
            TempsPause = 0,
            TempsExpire = 5,
            NombreCycles = 6
        };
    }
}
