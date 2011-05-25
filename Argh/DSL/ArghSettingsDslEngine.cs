namespace Argh.DSL
{
    using Boo.Lang.Compiler.Steps;

    using Rhino.DSL;

    public class ArghSettingsDslEngine : DslEngine
    {
        public ArghSettingsDslEngine()
        {
            this.Storage = new FileSystemDslEngineStorage();
        }

        protected override void CustomizeCompiler(Boo.Lang.Compiler.BooCompiler compiler, Boo.Lang.Compiler.CompilerPipeline pipeline, string[] urls)
        {
            compiler.Parameters.Ducky = true;

            var customStep = new AnonymousBaseClassCompilerStep(typeof(ArghSettings), "Build", "Argh");

            pipeline.Insert(1, customStep);
            pipeline.Insert(2, new UseSymbolsStep());

            pipeline.InsertBefore(typeof(ProcessMethodBodiesWithDuckTyping), new CapitalisationCompilerStep());
            pipeline.InsertBefore(typeof(ProcessMethodBodiesWithDuckTyping), new UnderscorNamingConventionsToPascalCaseCompilerStep());

            compiler.Parameters.Pipeline.Replace(typeof(ProcessMethodBodiesWithDuckTyping), new UnknownHashLiteralKeyToStringLiteral());
        }
    }
}