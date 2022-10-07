// ***********************************************************************
// Assembly         : MVVM_17_1_CSV_Laden
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="CsvService.cs" company="MVVM_17_1_CSV_Laden">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper.Configuration;
using System.Globalization;

namespace MVVM_17_1_CSV_Laden.Model
{
    /// <summary>
    /// Class CsvService.
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class CsvService : IDisposable
    {

        /// <summary>
        /// The reader
        /// </summary>
        TextReader reader;
        /// <summary>
        /// The CSV reader
        /// </summary>
        CsvHelper.CsvReader csvReader;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            reader?.Dispose();
            csvReader?.Dispose();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvService"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public CsvService(string filename)
        {
            reader = new StreamReader(filename);
            CsvConfiguration config = new CsvConfiguration(CultureInfo.CurrentCulture);
            config.Delimiter = ";";
            config.HasHeaderRecord = true;
            csvReader = new CsvHelper.CsvReader(reader, config);
            
        }

        /// <summary>
        /// Reads the CSV.
        /// </summary>
        /// <returns>IAsyncEnumerable&lt;DataPoint&gt;.</returns>
        public IAsyncEnumerable<DataPoint> ReadCSV()
        {
            var datapoints = csvReader.GetRecordsAsync<DataPoint>();
            return datapoints;
        }
    }
}
