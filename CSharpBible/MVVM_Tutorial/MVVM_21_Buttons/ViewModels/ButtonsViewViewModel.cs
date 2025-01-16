// ***********************************************************************
// Assembly         : MVVM_21_Buttons
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="ButtonsViewViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;

namespace MVVM_21_Buttons.ViewModels;

/// <summary>
/// Class ButtonsViewViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public class ButtonsViewViewModel : BaseViewModel
{
    #region Properties
#if NULLABLE || NET5_0_OR_GREATER
    private object? _lastPara;
#else
    /// <summary>
    /// The last para
    /// </summary>
    private object _lastPara;
#endif
    /// <summary>
    /// The flip
    /// </summary>
    private bool[] _flip = new bool[10];
  
    /// <summary>
    /// Gets or sets the play button.
    /// </summary>
    /// <value>The play button.</value>
    public DelegateCommand PlayButton { get; set; }
    /// <summary>
    /// Gets or sets the reset button.
    /// </summary>
    /// <value>The reset button.</value>
    public DelegateCommand ResetButton { get; set; }

#if NULLABLE || NET5_0_OR_GREATER
    public object? LastPara { get=>_lastPara; set=>SetProperty(ref _lastPara, value); }
#else
    /// <summary>
    /// Gets or sets the last para.
    /// </summary>
    /// <value>The last para.</value>
    public object LastPara { get=>_lastPara; set=>SetProperty(ref _lastPara, value); }
#endif
    /// <summary>
    /// Gets or sets the flip.
    /// </summary>
    /// <value>The flip.</value>
    public bool[] Flip { get => _flip; set => SetProperty(ref _flip, value); }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonsViewViewModel"/> class.
    /// </summary>
    public ButtonsViewViewModel()
    {
        PlayButton = new DelegateCommand(DoPlay, CanPlay);
        AddPropertyDependency(nameof(PlayButton), nameof(CanPlay));

        ResetButton = new DelegateCommand(DoReset);

        AddPropertyDependency(nameof(CanPlay), nameof(LastPara));
        _flip[1] = true;
    }

#if NULLABLE || NET5_0_OR_GREATER
    public bool CanPlay(object? obj)
#else
    /// <summary>
    /// Determines whether this instance can play the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns><c>true</c> if this instance can play the specified object; otherwise, <c>false</c>.</returns>
    public bool CanPlay(object obj)
#endif
    {
        AppendKnownParams(obj);
        return obj != LastPara;
    }

    /// <summary>
    /// Does the play.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    public void DoPlay<T>(T obj)
    {
        if (obj == null) return;
        LastPara = obj;
        if (obj is string s && int.TryParse(s, out int ix))
        {
            Flip[ix] = !Flip[ix];
            foreach (var ixo in new int[] {-1,1,-3,3 })
            {
                if (ix+ixo <= 9 && ix + ixo>0 )
                    if (Math.Abs(ixo)>1 || (ix-1)/3 == (ix+ixo-1)/3)
                        Flip[ix + ixo] = !Flip[ix + ixo];
            }
            RaisePropertyChanged(nameof(Flip));
        }

    }

#if NULLABLE || NET5_0_OR_GREATER
    private void DoReset(object? obj)
#else
    /// <summary>
    /// Does the reset.
    /// </summary>
    /// <param name="obj">The object.</param>
    private void DoReset(object obj)
#endif
    {
        foreach (var ix in new int[] { 1,2,3,4,5,6,7,8,9 })
            Flip[ix] = false;
        _flip[1] = true;
        RaisePropertyChanged(nameof(Flip));
        LastPara = "0";
    }
#endregion

}
