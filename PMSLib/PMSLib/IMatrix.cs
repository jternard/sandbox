using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PMSLib
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMatrix
    {
        object Mult(object VBinput, object VBinput2);
        object Inv(object VBinput);
    }
}
