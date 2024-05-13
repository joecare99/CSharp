﻿// ***********************************************************************
// Assembly         : All_Graphics
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="AllExampleModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using All_Graphics.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace All_Graphics.Models
{
    /// <summary>
    /// Class AllExampleModel.
    /// Implements the <see cref="ObservableObject" />
    /// Implements the <see cref="All_Graphics.Models.ITemplateModel" />
    /// </summary>
    /// <seealso cref="ObservableObject" />
    /// <seealso cref="All_Graphics.Models.ITemplateModel" />
    /// <autogeneratedoc />
    public partial class AllExampleModel : ObservableObject, ITemplateModel
    {
        #region Properties
        /// <summary>
        /// The timer
        /// </summary>
        /// <autogeneratedoc />
        private readonly Timer _timer;
        /// <summary>
        /// Gets or sets the get now.
        /// </summary>
        /// <value>The get now.</value>
        /// <autogeneratedoc />
        public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
        /// <autogeneratedoc />
        public DateTime Now { get => GetNow(); }

        public List<ExItem> Examples { get; } = [
            ("CanvasWPF", typeof(CanvasWPF.Views.CanvasWPFView), null),
            ("CanvasWPF2", typeof(CanvasWPF_CT.Views.CanvasWPFView), null),
            ("CanvasWPF3", typeof(CanvasWPF2_ItemTemplateSelector.Views.CanvasWPFView), null),
            ("CanvasWPF4", typeof(CanvasWPF2_CTItemTemplateSelector.Views.CanvasWPFView), null),
            ("DynamicShape", typeof(DynamicShapeWPF.Views.DynamicShapeView), null),
            ("DrawGrid_CT", typeof(MVVM_Converter_CTDrawGrid.Views.DrawGridView), null),
            ("DrawGrid2_CT", typeof(MVVM_Converter_CTDrawGrid2.Views.DrawGridView), null),
            ("ImgGrid_CT", typeof(MVVM_Converter_CTImgGrid.Views.PlotFrame), null),
            ("DrawGrid", typeof(MVVM_Converter_DrawGrid.Views.DrawGridView), null),
            ("DrawGrid2", typeof(MVVM_Converter_DrawGrid2.Views.DrawGridView), null),
            ("ImgGrid", typeof(MVVM_Converter_ImgGrid.Views.ImgGridView), null),
            ("ImgGrid2", typeof(MVVM_Converter_ImgGrid2.Views.ImgGridView), null),
            ("ImageHandling", typeof(MVVM_ImageHandling.Views.TemplateView), null),
            ("Lines_on_Grid", typeof(MVVM_Lines_on_Grid2.View.PlotFrame), null),
            ("PolyLine", typeof(Polyline.Views.PolyLineView), null),
          

            
        ]; 
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="AllExampleModel"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public AllExampleModel()
        {
            _timer = new(250d);
            _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
            _timer.Start();

            foreach (var ex in Examples)
                try
                {
                    Debug.WriteLine($"{ex.Description} {ex.ExType}");
                    var desc = new Dictionary<string, string>();
                    Type? t = ex.ExType.Assembly.GetTypes().First((t) => t.Name.EndsWith(nameof(Resources)));
                    if (t != null)
                    {
                        foreach (var prop in t.GetProperties())
                            if (prop.PropertyType == typeof(string))
                            {
                                Debug.WriteLine($"  {prop.Name} {prop.PropertyType} ");
                                desc[prop.Name] = (string)prop.GetValue(null);
                            }
                        ex.Additionals = desc;
                    }
                }
                catch { }   
        }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Finalizes an instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        ~AllExampleModel()
        {
            _timer.Stop();
            return;
        }
#endif
        #endregion
    }
}
