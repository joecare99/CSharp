// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="CColorList.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JCAMS.Core.Extensions;
using JCAMS.Core.Logging;
using JCAMS.Core.System;

namespace JCAMS.Core.Components.Coloring
{
    /// <summary>
    /// Class CColorList.
    /// </summary>
    public class CColorList :IHasDescription //, IXmlSerializable
    {
        #region Properties
        public static Single fFineWidth { get; set; } = 1.0f;
        public static Single fThickWidth { get; set; } = 5.0f;

        /// <summary>
        /// The m label
        /// </summary>
        private string? _Label;

        /// <summary>
        /// The m color
        /// </summary>
        private Color[]? _Color;

        /// <summary>
        /// The m brush
        /// </summary>
        private Brush[] _Brushes;

        /// <summary>
        /// The m pen1
        /// </summary>
        private Pen[] _PenFine;

        /// <summary>
        /// The m pen5
        /// </summary>
        private Pen[] _PenThick;

        /// <summary>
        /// The m nr
        /// </summary>
        private int _Nr;

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get => _Label ?? ""; set => _Label = value; }

        /// <summary>
        /// Gets the nr.
        /// </summary>
        /// <value>The nr.</value>
        [Browsable(false)]
        public int Nr {get=> _Nr;protected set => _Nr = value; }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>The list.</value>
        [Browsable(false)]
        public Color[]? List => _Color;

        [Browsable(false)]
        [XmlIgnore]
        public Brush[] Brushes => _Brushes;

        [Browsable(false)]
        [XmlIgnore]
        public Pen[] Pen1 => _PenFine;

        [Browsable(false)]
        [XmlIgnore]
        public Pen[] Pen5 => _PenThick;

        string IHasDescription.Description => _Label ?? "";
        #endregion
        #region Methods
        public CColorList()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CColorList" /> class.
        /// </summary>
        /// <param name="LabelName">Name of the label.</param>
        /// <param name="Clr">The color.</param>
        public CColorList(string LabelName, params Color[] Clr)
        {
            try
            {
                Label = LabelName;
   /*             if (CConfiguration.General?.InstancePool != null)
                    if (!CConfiguration.General.InstancePool.IsInstanceTheServiceRunsIn)
                        Label = T.sTr(LabelName);*/
                int N = Clr.Length;
                _Color = new Color[N];
                _Brushes = new Brush[N];
                _PenFine = new Pen[N];
                _PenThick = new Pen[N];
                for (int I = 0; I < _Color.Length; I++)
                {
                    _Color[I] = Clr[I];
                    _Brushes[I] = new SolidBrush(Clr[I]);
                    _PenFine[I] = new Pen(Clr[I], fFineWidth);
                    _PenThick[I] = new Pen(Clr[I], fThickWidth);
                }
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CColorList" /> class.
        /// </summary>
        ~CColorList()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            int I = 0;
            while (_Color != null && I < _Color.Length)
            {
                _Brushes[I].Dispose();
                _PenFine[I].Dispose();
                _PenThick[I].Dispose();
                I++;
            }
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool SaveConfiguration(string AppName, string SectionName, string KeyName)
        {
            for (int I = 0; I < List.Length; I++)
            {
                SVariableHandling.SaveVar(AppName, SectionName, $"{KeyName} Idx {I}", _Color[I].ToArgb());
            }
            return true;
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool LoadConfiguration(string AppName, string SectionName, string KeyName)
        {
            for (int I = 0; I < List.Length; I++)
            {
                SVariableHandling.LoadVar(AppName, SectionName, $"{KeyName} Idx {I}", out int C);
                _Color[I] = Color.FromArgb(C);
                _Brushes[I] = new SolidBrush(_Color[I]);
                _PenFine[I] = new Pen(_Color[I], fFineWidth);
                _PenThick[I] = new Pen(_Color[I], fThickWidth);
            }
            return true;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => $"#{_Nr} {_Label}";

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is CColorList cl && cl.List?.Length == List?.Length)
            {
                for (var i = 0; i < (List?.Length ?? 0); i++)
                    if (cl.List[i].ToArgb() != List[i].ToArgb()) return false;
                return (cl.Label == Label);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public XmlSchema? GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Label=reader.GetAttribute(nameof(Label));
            Nr = reader.GetAttribute(nameof(Nr)).AsInt32();

            //          throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nameof(Label), Label);
            writer.WriteAttributeString(nameof(Nr), Nr.ToString());
            writer.WriteAttributeString($"{nameof(Color)}.Count", (_Color?.Length??0).ToString());
            for (var i = 0; i < _Color?.Length; i++)
            {
                _Color[i].WriteToXML(writer,true);
            }
        }
        #endregion
    }
}
