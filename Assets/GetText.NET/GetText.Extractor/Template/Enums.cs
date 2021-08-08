using System;

namespace Assets.GetText.NET.GetText.Extractor.Template
{
    [Flags]
    internal enum MessageFlags
    {
        None = 0x0,
        Fuzzy = 0x1,
        CSharpFormat = 0x2,
    }
}
