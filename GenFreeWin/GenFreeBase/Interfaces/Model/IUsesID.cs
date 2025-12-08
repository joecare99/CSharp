//using DAO;
namespace GenFree.Interfaces.Model
{
    public interface IUsesID<T>

    {
        /// <summary>
        /// Gets the maximum identifier.
        /// </summary>
        /// <value>The maximum identifier.</value>
        T MaxID { get; }
        /// <summary>
        /// Deletes the record with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if deleted, <c>false</c> otherwise.</returns>
        bool Delete(T key);
        /// <summary>
        /// checkes the specified key for existence.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if exists, <c>false</c> otherwise.</returns>
        bool Exists(T key);
    }
}