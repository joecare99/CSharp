// ***********************************************************************
// Assembly         : MVVM_Converter_ImgGrid2
// Author           : Mir
// Created          : 08-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="Model.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using System;
using Sokoban_Base.Model;
using System.Drawing;

namespace MVVM_Converter_ImgGrid2.Model
{
    /// <summary>
    /// Struct LabDefData
    /// </summary>
    public struct LabDefData
    {
        /// <summary>
        /// The fields
        /// </summary>
        public FieldDef[] fields;
        /// <summary>
        /// The l size
        /// </summary>
        public Size lSize;
        /// <summary>
        /// Initializes a new instance of the <see cref="LabDefData"/> struct.
        /// </summary>
        /// <param name="par">The par.</param>
        public LabDefData((FieldDef[],Size) par)
        {
            fields = par.Item1;
            lSize = par.Item2;
        }
    }

    /// <summary>
    /// Class Model.
    /// </summary>
    public static class Model
    {
        /// <summary>
        /// The act level
        /// </summary>
        private static int actLevel;
        /// <summary>
        /// The level data
        /// </summary>
        private static LabDefData? _levelData;

        /// <summary>
        /// Gets or sets the level data.
        /// </summary>
        /// <value>The level data.</value>
        public static LabDefData? LevelData { get => _levelData; set => PropertyHelper.SetProperty(ref _levelData,value, OnPropertyChanged); }
        /// <summary>
        /// Gets or sets the act level.
        /// </summary>
        /// <value>The act level.</value>
        public static int ActLevel { get => actLevel; set => PropertyHelper.SetProperty(ref actLevel, value, OnPropertyChanged); }
        /// <summary>
        /// Gets or sets the property changed.
        /// </summary>
        /// <value>The property changed.</value>
        public static EventHandler<(string, object, object)>? PropertyChanged { get; set; }

        /// <summary>
        /// Loads the level.
        /// </summary>
        public static void LoadLevel()
        {
            actLevel = 0;
            LevelData = new LabDefData(LabDefs.GetLevel(0));
             
        }

        /// <summary>
        /// Nexts the level.
        /// </summary>
        public static void NextLevel()
        {
            if (ActLevel < LabDefs.Count)
                ActLevel += 1;
            LevelData = new LabDefData(LabDefs.GetLevel(ActLevel));
        }

        /// <summary>
        /// Previouses the level.
        /// </summary>
        public static void PrevLevel()
        {
            if (ActLevel > 0) 
               ActLevel -= 1;
            LevelData = new LabDefData(LabDefs.GetLevel(ActLevel));
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        private static void OnPropertyChanged<T>(string arg1, T arg2, T arg3)
        {
            PropertyChanged?.Invoke(null, (arg1, arg2!, arg3!));
        }
    }
}
