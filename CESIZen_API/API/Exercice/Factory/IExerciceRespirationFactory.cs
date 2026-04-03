using CESIZen_API.API.ConfigRespiration.Models;

namespace CESIZen_API.API.Exercice.Factory
{
    /// <summary>
    /// Interface de la Factory pour les configurations de respiration.
    /// Chaque implémentation crée une ConfigsRespirationModel avec des valeurs prédéfinies.
    /// </summary>
    public interface IExerciceRespirationFactory
    {
        /// <summary>
        /// Type de technique géré par cette factory.
        /// </summary>
        TypeExerciceRespiration Type { get; }

        /// <summary>
        /// Crée une ConfigsRespirationModel prédéfinie pour l'exercice donné.
        /// </summary>
        /// <param name="exerciceId">Identifiant de l'exercice auquel rattacher la config.</param>
        /// <returns>Une instance de ConfigsRespirationModel non persistée.</returns>
        ConfigsRespirationModel Create(int exerciceId);
    }
}
