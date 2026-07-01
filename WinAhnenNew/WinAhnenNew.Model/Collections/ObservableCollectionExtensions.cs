using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WinAhnenNew.Collections
{
    /// <summary>
    /// Provides helper methods for updating observable collections without rebuilding them completely.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Synchronizes the target collection with the provided source values while preserving unchanged entries where possible.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="lstTarget">The target collection to update.</param>
        /// <param name="lstValues">The source values to apply.</param>
        public static void SynchronizeWith<T>(this ObservableCollection<T> lstTarget, IEnumerable<T> lstValues)
        {
            ArgumentNullException.ThrowIfNull(lstTarget);
            ArgumentNullException.ThrowIfNull(lstValues);

            var arrValues = lstValues as IList<T> ?? new List<T>(lstValues);
            if (arrValues.Count == 0)
            {
                if (lstTarget.Count > 0)
                {
                    lstTarget.Clear();
                }

                return;
            }

            var cmpComparer = EqualityComparer<T>.Default;
            var iSharedCount = arrValues.Count < lstTarget.Count ? arrValues.Count : lstTarget.Count;

            for (var iIndex = 0; iIndex < iSharedCount; iIndex++)
            {
                if (!cmpComparer.Equals(lstTarget[iIndex], arrValues[iIndex]))
                {
                    lstTarget[iIndex] = arrValues[iIndex];
                }
            }

            for (var iIndex = lstTarget.Count - 1; iIndex >= arrValues.Count; iIndex--)
            {
                lstTarget.RemoveAt(iIndex);
            }

            for (var iIndex = lstTarget.Count; iIndex < arrValues.Count; iIndex++)
            {
                lstTarget.Add(arrValues[iIndex]);
            }
        }
    }
}
