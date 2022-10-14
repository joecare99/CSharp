// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 10-09-2022
//
// Last Modified By : Mir
// Last Modified On : 10-09-2022
// ***********************************************************************
// <copyright file="INotifyPropertyChangedEx.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary>Advanced interface for notifocation of property changes</summary>
// ***********************************************************************

using System.ComponentModel;

namespace JCAMS.Core.DataOperations
{
    public interface INotifyPropertyChangedEx
    {
        event PropertyChangedExEventHandler OnPropertyChangedEx;
    }

    public delegate void PropertyChangedExEventHandler(object Sender, PropertyChangedExEventArgs e);

    public class PropertyChangedExEventArgs : PropertyChangedEventArgs
    {
        private object oldVal;
        private object newVal;

        public object OldVal => oldVal;
        public object NewVal => newVal;
        public PropertyChangedExEventArgs(string propertyName,object oldVal,object newVal) : base(propertyName)
        {
            this.oldVal = oldVal;
            this.newVal = newVal;    
        }
    }
}