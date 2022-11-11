// ***********************************************************************
// Assembly         : CommonDialogs
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="ColorDialog.cs" company="CommonDialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// The CommonDialogs namespace.
/// </summary>
namespace CommonDialogs
{
	/// <summary>
	/// Class ColorDialog.
	/// Implements the <see cref="Component" />
	/// </summary>
	/// <seealso cref="Component" />
	public partial class ColorDialog : Component
	{
		/// <summary>
		/// The cd
		/// </summary>
		private System.Windows.Forms.ColorDialog _cd = new System.Windows.Forms.ColorDialog();

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorDialog"/> class.
		/// </summary>
		public ColorDialog()
        {
        }

		/// <summary>
		/// Ruft einen Wert ab, der angibt, ob im Dialogfeld benutzerdefinierte Farben definiert werden können, oder legt diesen fest.
		/// </summary>
		/// <value><c>true</c> if [allow full open]; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public virtual bool AllowFullOpen { get => _cd.AllowFullOpen;set => _cd.AllowFullOpen= value; }

		/// <summary>
		/// Ruft einen Wert ab, der angibt, ob im Dialogfeld bei den Grundfarben alle verfügbaren Farben angezeigt werden, oder legt diesen fest.
		/// </summary>
		/// <value><c>true</c> if [any color]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public virtual bool AnyColor { get => _cd.AnyColor; set => _cd.AnyColor = value; }


		/// <summary>
		/// Ruft die von den Benutzern ausgewählte Farbe ab oder legt diese fest.
		/// </summary>
		/// <value>The color.</value>
		public Color Color { get => _cd.Color; set => _cd.Color = value; }


		/// <summary>
		/// Ruft den im Dialogfeld angezeigten Satz benutzerdefinierter Farben ab oder legt diesen fest.
		/// </summary>
		/// <value>The custom colors.</value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int[] CustomColors { get => _cd.CustomColors; set => _cd.CustomColors = value; }

		/// <summary>
		/// Ruft einen Wert ab, der angibt, ob die Steuerelemente für das Erstellen benutzerdefinierter Farben beim Öffnen des Dialogfelds angezeigt werden, oder legt diesen fest.
		/// </summary>
		/// <value><c>true</c> if [full open]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public virtual bool FullOpen { get => _cd.FullOpen; set => _cd.FullOpen = value; }


		/// <summary>
		/// Ruft einen Wert ab, der angibt, ob im Farbendialogfeld eine Hilfeschaltfläche angezeigt wird, oder legt diesen Wert fest.
		/// </summary>
		/// <value><c>true</c> if [show help]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public virtual bool ShowHelp { get => _cd.ShowHelp; set => _cd.ShowHelp = value; }


		/// <summary>
		/// Ruft einen Wert ab, der angibt, ob Benutzer im Dialogfeld ausschließlich Volltonfarben auswählen können, oder legt diesen fest.
		/// </summary>
		/// <value><c>true</c> if [solid color only]; otherwise, <c>false</c>.</value>
		[DefaultValue(false)]
		public virtual bool SolidColorOnly { get => _cd.SolidColorOnly; set => _cd.SolidColorOnly = value; }

		/// <summary>
		/// Setzt alle Optionen auf die Standardwerte zurück, die zuletzt ausgewählte Farbe auf Schwarz und die benutzerdefinierten Farben auf die Standardwerte.
		/// </summary>
		public void Reset() => _cd.Reset();

		/// <summary>
		/// Gibt eine Zeichenfolge zurück, die den <see cref="T:System.Windows.Forms.ColorDialog" /> darstellt.
		/// </summary>
		/// <returns>Ein <see cref="T:System.String" />, der den aktuellen <see cref="T:System.Windows.Forms.ColorDialog" /> darstellt.</returns>
		public override string ToString() => _cd.ToString();

		/// <summary>
		/// Ruft ein Objekt ab, das Daten bezüglich des Steuerelements enthält, oder legt dieses Objekt fest.
		/// </summary>
		/// <value>The tag.</value>
		[Localizable(false)]
		[Bindable(true)]
		[DefaultValue(null)]
		[TypeConverter(typeof(StringConverter))]
		public object Tag { get => _cd.Tag; set => _cd.Tag = value; }

		/// <summary>
		/// Führt ein Standarddialogfeld mit einem Standardbesitzer aus.
		/// </summary>
		/// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
		public bool? ShowDialog()
		{
			return _cd.ShowDialog() == DialogResult.OK;
		}

		/// <summary>
		/// Führt ein Standarddialogfeld mit dem angegebenen Besitzer aus.
		/// </summary>
		/// <param name="owner">Ein beliebiges Objekt, das <see cref="T:System.Windows.Forms.IWin32Window" /> implementiert, das das Fenster der obersten Ebene und damit den Besitzer des modalen Dialogfelds darstellt.</param>
		/// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
		public bool? ShowDialog(IWin32Window owner)
		{
			return _cd.ShowDialog(owner) == DialogResult.OK;
		}

	}
}
