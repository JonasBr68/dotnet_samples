using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp6Console
{
    public class MyStatics
    {
        public static string EchoMe(string toEcho)
        {
            return toEcho;
        }
        public class NestedStatics        {
            public static string EchoMeNested(string toEcho)
            {
                return toEcho;
            }

        }
    }
}
