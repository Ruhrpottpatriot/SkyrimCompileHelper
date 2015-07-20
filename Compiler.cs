namespace SkyrimCompilerHelper
{
    using System;

    internal class Compiler
    {
        private CompileMode mode;

        private Utilities utilities;

        public Compiler(CompileMode mode)
        {
            this.mode = mode;
            this.utilities = new Utilities();
        }

        public void Compile()
        {
            throw new NotImplementedException();
        }
    }
}