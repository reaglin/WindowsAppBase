using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsAppBase.DomainCode;

namespace WindowsAppBase
{
    // This should really be in the DomainCode directory as it is domain specific
    static class Globals
    {
        private static DomainCode.SampleProject project = new SampleProject();

        public static SampleProject Project { get => project; set => project = value; }

    }
}
