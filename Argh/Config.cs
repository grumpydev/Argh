namespace Argh
{
    using System;
    using System.IO;

    public class Config
    {
        public string InputFile { get; private set; }

        public string OutputFile { get; private set; }

        public Config(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            var filenamePosition = 0;
            if (args[0].StartsWith("-o:"))
            {
                filenamePosition = 1;
                this.OutputFile = args[0].Substring("-o:".Length);
            }

            if (args.Length < filenamePosition-1)
            {
                return;
            }

            this.InputFile = args[filenamePosition];
        }
    }
}