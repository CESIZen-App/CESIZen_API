using CESIZen_API.API.ConfigRespiration.Models;

namespace CESIZen_API.API.Exercice.Factory.Implementations
{
    /// <summary>
    /// Factory pour la technique de cohérence cardiaque 7-4-8.
    /// Inspiration : 7s / Apnée : 4s / Expiration : 8s
    /// </summary>
    public class Technique748Factory : IExerciceRespirationFactory
    {
        public TypeExerciceRespiration Type => TypeExerciceRespiration.Technique748;

        public ConfigsRespirationModel Create(int exerciceId) => new()
        {
            ExerciceId = exerciceId,
            TempsInspire = 7,
            TempsPause = 4,
            TempsExpire = 8,
            NombreCycles = 4
        };
    }
}
