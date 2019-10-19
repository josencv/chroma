using System;

namespace Chroma.Configuration
{
    [Flags]
    public enum BindingIdentifiers
    {
        FromScene = 1,
        FromSiblingComponent = 2,
        FromParents = 4,
        WithPlayerTag = 8,
        FromChildren = 16,
        Multiple = 32,
    }
}
