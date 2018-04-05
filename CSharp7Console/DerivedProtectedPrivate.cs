using CSharp72DLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp7Console
{
    public class DerivedProtectedPrivate :  BasePrivateProtected
    {
        public override void ForEveryOne()
        {
            // TODO CS72 4.2 private protected not visible from other assembly (exception use InternalsVisibleTo)
            // error CS0122: 'BaseA.OnlyForTrustedDerivedClasses()' is inaccessible due to its protection level
            // base.OnlyForTrustedDerivedClasses();
        }
    }
}
