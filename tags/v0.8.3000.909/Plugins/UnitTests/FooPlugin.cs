using System;
using System.Collections.Generic;
using System.Text;

namespace Virtuoso.Miranda.Plugins.UnitTests
{
    class FooPlugin : MirandaPlugin
    {
        public override string Author
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override string Description
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool HasOptions
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override Uri HomePage
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override Version Version
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
    }
}
