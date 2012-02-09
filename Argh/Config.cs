namespace Argh
{
    public class Config
    {
        public string InputFile { get; private set; }

        public string OutputFile { get; private set; }

        public string ErrorFile { get; private set; }

        public Config(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            var filenamePosition = 0;
            if (args[filenamePosition].StartsWith("-o:"))
            {
                this.OutputFile = args[filenamePosition].Substring("-o:".Length);
                filenamePosition++;
            }

            if (args.Length < filenamePosition - 1)
            {
                return;
            }

            if (args[filenamePosition].StartsWith("-e:"))
            {
                this.ErrorFile = args[filenamePosition].Substring("-e:".Length);
                filenamePosition++;
            }

            if (args.Length < filenamePosition-1)
            {
                return;
            }

            this.InputFile = args[filenamePosition];
        }
    }
}