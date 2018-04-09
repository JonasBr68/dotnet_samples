using System;

namespace ProjectCS7
{
    interface IDocFactory
    {
        void FillMember(dynamic dynamicChild, IFormatProvider culture);
        string ToString(IFormatProvider provider);
    }
}

