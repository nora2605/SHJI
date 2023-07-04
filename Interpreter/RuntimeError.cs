using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHJI.Interpreter
{
    internal class RuntimeError : Exception
    {
        public RuntimeError() { }
        public RuntimeError(string message) : base(message) { }
    }
}
