// ***********************************************************************
// Assembly         : CommonDialogs
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-13-2022
// ***********************************************************************
// <copyright file="FolderBrowserDialog.cs" company="CommonDialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommonDialogs.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CommonDialogs;

    // System.Windows.Forms.FolderBrowserDialog

    /// <summary>
    /// Fordert den Benutzer auf, einen Ordner auszuwählen. Diese Klasse kann nicht vererbt werden.
    /// </summary>
    [DefaultEvent("HelpRequest")]
[DefaultProperty("SelectedPath")]
public class FolderBrowserDialog : Component, IFileDialog
{
	/// <summary>
	/// The FBD
	/// </summary>
	private readonly System.Windows.Forms.FolderBrowserDialog _fbd =new System.Windows.Forms.FolderBrowserDialog();

	/// <summary>
	/// Gets or sets a value indicating whether [show new folder button].
	/// </summary>
	/// <value><c>true</c> if [show new folder button]; otherwise, <c>false</c>.</value>
	public bool ShowNewFolderButton { get => _fbd.ShowNewFolderButton; set => _fbd.ShowNewFolderButton = value; }

	/// <summary>
	/// Ruft den von den Benutzern ausgewählten Pfad ab oder legt diesen fest.
	/// </summary>
	/// <value>The selected path.</value>
	public string SelectedPath { get => _fbd.SelectedPath; set => _fbd.SelectedPath = value; }

        public string FileName { get => _fbd.SelectedPath; set => _fbd.SelectedPath =value; }

        /// <summary>
        /// Ruft den Stammordner ab, von dem aus eine Suche gestartet wird, oder legt diesen fest.
        /// </summary>
        /// <value>The root folder.</value>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Der zugewiesene Wert ist keiner der <see cref="T:System.Environment.SpecialFolder" />-Werte.</exception>
        public Environment.SpecialFolder RootFolder { get => _fbd.RootFolder; set => _fbd.RootFolder = value; }

	/// <summary>
	/// Ruft den beschreibenden Text ab, der im Dialogfeld über dem Strukturansichts-Steuerelement angezeigt wird, oder legt diesen fest.
	/// </summary>
	/// <value>The description.</value>
	public string Description { get => _fbd.Description; set => _fbd.Description = value; }

	/// <summary>
	/// Tritt ein, wenn der Benutzer im Dialogfeld auf die Schaltfläche Hilfe klickt.
	/// </summary>
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public event EventHandler HelpRequest { add => _fbd.HelpRequest += value; remove => _fbd.HelpRequest -= value; }



	/// <summary>
	/// Initialisiert eine neue Instanz der <see cref="T:System.Windows.Forms.FolderBrowserDialog" />-Klasse.
	/// </summary>
	public FolderBrowserDialog()
	{
		Reset();
	}

	/// <summary>
	/// Setzt Eigenschaften auf die Standardwerte zurück.
	/// </summary>
	public void Reset()
	{
		_fbd.Reset();
	}
	/// <summary>
	/// Ruft ein Objekt ab, das Daten bezüglich des Steuerelements enthält, oder legt dieses Objekt fest.
	/// </summary>
	/// <value>The tag.</value>
	[Localizable(false)]
	[Bindable(true)]
	[DefaultValue(null)]
	[TypeConverter(typeof(StringConverter))]
	public object? Tag { get => _fbd.Tag; set => _fbd.Tag = value; }

        /// <summary>
        /// Führt ein Standarddialogfeld mit einem Standardbesitzer aus.
        /// </summary>
        /// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
        public bool? ShowDialog()
        {
		return _fbd.ShowDialog()==DialogResult.OK;
        }

	/// <summary>
	/// Führt ein Standarddialogfeld mit dem angegebenen Besitzer aus.
	/// </summary>
	/// <param name="owner">Ein beliebiges Objekt, das <see cref="T:System.Windows.Forms.IWin32Window" /> implementiert, das das Fenster der obersten Ebene und damit den Besitzer des modalen Dialogfelds darstellt.</param>
	/// <returns><see cref="F:System.Windows.Forms.DialogResult.OK" />, wenn der Benutzer im Dialogfeld auf OK klickt, andernfalls <see cref="F:System.Windows.Forms.DialogResult.Cancel" />.</returns>
	public bool? ShowDialog(object owner)
	{
		return _fbd.ShowDialog((IWin32Window)owner) == DialogResult.OK; 
	}

}
