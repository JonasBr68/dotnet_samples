using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace CSharp7Project
{
    interface IValue<T>
    {
        T Value { get; }
    }
    interface IRow<T>
    {
        IEnumerable<IValue<T>> Values { get; }
    }
    interface IBaseDoc<T>
    {
        IEnumerable<IRow<string>> Fields { get; }
        IEnumerable<IRow<T>> Rows { get; }
    }

    struct Doc<T> : IBaseDoc<T>
    {
        public IEnumerable<IRow<string>> Fields => throw new NotImplementedException();

        public IEnumerable<IRow<T>> Rows => throw new NotImplementedException();
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            var myDoc = new Doc<int>();


        }
    }
}
