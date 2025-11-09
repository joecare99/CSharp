// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir (extended by AI)
// Created          : 09-26-2025
// ***********************************************************************
namespace ConsoleLib.Interfaces;

public interface IGroupControl : IControl
{
    void BringToFront(IControl menuPopup);
}