namespace Argh.DSL
{
    using System;

    using Rhino.DSL;

    public static class Settings
    {
        private static readonly DslFactory DslFactory;

        static Settings()
        {
            DslFactory = new DslFactory();
            DslFactory.Register<ArghSettings>(new ArghSettingsDslEngine());
            DslFactory.BaseDirectory = Environment.CurrentDirectory;
        }

        /// <summary>
        /// Load settings from the given dsl file
        /// </summary>
        /// <param name="dslFilename">Filename of the dsl</param>
        /// <returns>Settings object</returns>
        public static ArghSettings Load(string dslFilename)
        {
            var settings = DslFactory.TryCreate<ArghSettings>(dslFilename);
            if (settings == null)
            {
                throw new SettingsLoadException(dslFilename);
            }

            settings.Build();

            return settings;
        }
    }
}