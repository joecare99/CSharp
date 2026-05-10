using System;

namespace BaseGenClasses.Helper;

/// <summary>
/// Represents the message callback used by genealogical helper implementations.
/// </summary>
/// <param name="sender">The sender raising the message.</param>
/// <param name="type">The message classification.</param>
/// <param name="text">The message text.</param>
/// <param name="reference">The current parser reference.</param>
/// <param name="mode">The parser mode.</param>
public delegate void THlprMsgEvent(object sender, EventType type, string text, string reference, int mode);
