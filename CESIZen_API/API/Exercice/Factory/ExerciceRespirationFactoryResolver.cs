using CESIZen_API.API.ConfigRespiration.Models;

namespace CESIZen_API.API.Exercice.Factory
{
    /// <summary>
    /// Résolveur de factory : sélectionne la bonne IExerciceRespirationFactory
    /// en fonction du TypeExerciceRespiration demandé.
    /// Toutes les factories enregistrées dans le conteneur DI sont injectées automatiquement.
    /// </summary>
    public class ExerciceRespirationFactoryResolver
    {
        private readonly Dictionary<TypeExerciceRespiration, IExerciceRespirationFactory> _factories;

        public ExerciceRespirationFactoryResolver(IEnumerable<IExerciceRespirationFactory> factories)
        {
            _factories = factories.ToDictionary(f => f.Type);
        }

        /// <summary>
        /// Résout la factory correspondant au type et crée une ConfigsRespirationModel.
        /// </summary>
        /// <exception cref="ArgumentException">Si le type n'est pas supporté.</exception>
        public ConfigsRespirationModel Create(TypeExerciceRespiration type, int exerciceId)
        {
            if (!_factories.TryGetValue(type, out var factory))
                throw new ArgumentException($"Type d'exercice de respiration non supporté : {type}");

            return factory.Create(exerciceId);
        }
    }
}
