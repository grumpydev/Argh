namespace Argh.DSL
{
    using System;

    public class SettingsLoadException : Exception
    {
        public string DslFilename { get; private set; }

        public SettingsLoadException(string dslFilename)
        {
            this.DslFilename = dslFilename;
        }
    }
}