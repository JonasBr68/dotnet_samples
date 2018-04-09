using System;

namespace ProjectCS7
{
    public interface IElementFactory<T>
    {
        T MakeItem(dynamic c, IFormatProvider culture);
        string ToString(IFormatProvider provider, T item);
    }
}

