using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace CSharp7Console
{
    /* Local function over lambdas - the advantages are:
     * From https://stackoverflow.com/questions/40943117/local-function-vs-lambda-c-sharp-7-0
     * 
     * Performance:
     * With a lambda, a delegate has to be created
     * Local functions are more efficient with capturing local variables
     * Local functions can be inlined
     * 
     * Local functions can be recursive:
     * Lambdas can be recursive too, but it requires awkward code
     * 
     * Local functions can be generic
     * 
     * Local functions can be implemented as an iterator using yield return/yield break
     * 
     * Local functions are hoisted, can be declared after the return statement
     * 
     */
    class LocalFunctions
    {
        public static async Task ThrowsAsync(string param)
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentNullException(nameof(param));

            var localMsg = "Hello from local";
            await FakeAsync();

            async Task FakeAsync()
            {
                await Task.Yield();
                WriteLine(localMsg);
            }

        }

        // TODO CS70 6.0 Local function
        public static Task LocalFunctionAsync(string param)
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentNullException(nameof(param));

            return FakeAsync(param);

            async Task FakeAsync(string msg)
            {
                await Task.Yield();
                WriteLine(msg);
            }
        }

        public static Action ReturnLocalFunction()
        {
            return MyLocalFunc;

            void MyLocalFunc()
            {
                WriteLine("MyLocalFunc");
            }
        }

        public static Action ReturnLocalLambda()
        {
            Action MyLocalFunc = () =>
            {
                WriteLine("MyLocalFunc");
            };

            return MyLocalFunc;

        }

    }
}
