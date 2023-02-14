namespace MVVM_22_WpfCap.Model
{
    /// <summary>Abstract interface for a random-(numbers) class </summary>
    public interface IRandom
    {
        /// <summary>Returns the <strong>next</strong> random number with the specified maximum (excl.).</summary>
        /// <param name="max">The maximum.</param>
        /// <returns>The random value</returns>
        int Next(int max);
    }
}