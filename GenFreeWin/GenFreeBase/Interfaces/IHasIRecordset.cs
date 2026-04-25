//using DAO;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces
{
    public interface IHasIRecordset
    {
        /// <summary>
        /// Deletes the record with this ID.
        /// </summary>
        void Delete();
        /// <summary>
        /// Fills the (local) data with values from the recordset.
        /// </summary>
        /// <param name="dB_Table">The recordset.</param>
        void FillData(IRecordset dB_Table);
        /// <summary>
        /// Reads the identifier.
        /// </summary>
        /// <param name="dB_Table">The recordset.</param>
        void ReadID(IRecordset dB_Table);
        /// <summary>
        /// Writes values to the recoedset.
        /// </summary>
        /// <param name="dB_Table">The recordset.</param>
        /// <param name="asProps">As props.</param>
        void SetDBValues(IRecordset dB_Table, Enum[]? asProps);
    }
}