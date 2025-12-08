using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model
{
    public interface IHasDataItf<T,T2> : IUsesID<T2> where T : IHasID<T2>
    {
        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ReadData(T2 key,out T? data);
        /// <summary>
        /// Reads all records.
        /// </summary>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        IEnumerable<T> ReadAll();
        /// <summary>
        /// Writes the (changed) data to the record.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <param name="asProps">As props.</param>
        void SetData(T2 key,T data, Enum[]? asProps = null);
        /// <summary>
        /// Commits the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Commit(T data);
    }
}
