using System;

namespace Trnsp.Show.Lfm.Models.Components;

[Flags]
public enum EGridOptions
{
    None = 0,
    FixedVertLine = 1,
    FixedHorzLine = 2,
    VertLine = 4,
    HorzLine = 8,
    RangeSelect = 16,
    DrawFocusSelected = 32,
    RowSizing = 64,
    ColSizing = 128,
    RowMoving = 256,
    ColMoving = 512,
    Editing = 1024,
    Tabs = 2048,
    AlwaysShowEditor = 4096,
    Default = FixedVertLine | FixedHorzLine | VertLine | HorzLine | RangeSelect | DrawFocusSelected
}
