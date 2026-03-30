namespace CESIZen_API.API.Exercice.Factory
{
    /// <summary>
    /// Types de presets de respiration disponibles.
    /// 748 : Inspiration 7s / Apnée 4s / Expiration 8s
    /// 55  : Inspiration 5s / Apnée 0s / Expiration 5s
    /// 46  : Inspiration 4s / Apnée 0s / Expiration 6s
    /// </summary>
    public enum TypeExerciceRespiration
    {
        Technique748,
        Technique55,
        Technique46
    }
}
