using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActsConsoleKirill.Interfaces
{
    public interface IReporter
    {
        void Write(string message);
        void WriteLine();
        void WriteLine(string message);
    }
}
