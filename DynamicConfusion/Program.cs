using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DynamicConfusion
{
    class Program
    {
        static public string MakeString(dynamic dyn)
        {
            return dyn.str;
        }
        static void Main(string[] args)
        {
            IList<string> list = new List<string>();

            dynamic input = new ExpandoObject();
            input.str = "string value";

            var str = MakeString(input);

            list.Add(str); // Microsoft.CSharp.RuntimeBinder.RuntimeBinderException 'System.Collections.Generic.IList<string>' does not contain a definition for 'Add'
        }
    }
}
