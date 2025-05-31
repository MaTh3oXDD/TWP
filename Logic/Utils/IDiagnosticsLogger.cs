using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Utils
{
    public interface IDiagnosticsLogger
    {
        void Log(string message);
    }

}
