using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace CSharp72DLL
{
    public class BasePrivateProtected
    {
        // TODO CS72 4.0 new protection level private protected maps to the CLR notion of protectedAndInternal so can be exposed via InternalsVisibleTo
        virtual private protected void OnlyForTrustedDerivedClasses()
        {
            WriteLine(nameof(this.OnlyForTrustedDerivedClasses));
        }
        virtual internal protected void InternalProtected()
        {

        }
        virtual protected void Protected()
        {

        }
        virtual public void ForEveryOne()
        {
            WriteLine(nameof(this.ForEveryOne));
        }
    }

    class Test
    {
        Test()
        {
            var bp = new BasePrivateProtected();
            bp.InternalProtected();
            
        }
    }

    public class DerivedInternalA : BasePrivateProtected
    {
        protected private override void OnlyForTrustedDerivedClasses()
        {
            base.OnlyForTrustedDerivedClasses();
        }
        
    }
}
