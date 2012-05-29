using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    [ElementProblemAnalyzer(new[] { typeof(IMethodDeclaration) }, HighlightingTypes = new[] { typeof(DllImportMissingHighlighting) })]
    public class DllImportInt32ForIntPtrAnalyzer : DllImportMethodProblemAnalyzer
    {
        private static readonly ClrTypeName IntPtrClrType = new ClrTypeName("System.IntPtr");
        protected override void Run(IDllImportMethodDeclaration importMethod, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var element = importMethod.MethodDeclaration;
            var libraryName = importMethod.ImportedDll;
            var factory = new LibraryFactory();
            var library = factory.LoadLibrary(libraryName);
            var export = library[element.NameIdentifier.Name];
            if (export == null)
            {
                return;
            }
            for (var i = 0; i < element.ParameterDeclarations.Count; i++ )
            {
                var parameter = element.ParameterDeclarations[i];
                if (!parameter.Type.IsInt())
                {
                    continue;
                }
                var knownParameter = export.Parameters[i];
                var exportedType = new ClrTypeName(knownParameter.CLRType.FullName);
                
                if (exportedType.Equals(IntPtrClrType))
                {
                    consumer.AddHighlighting(new DllImportInt32ForIntPtrHighlighting(parameter));
                }
            }
        }
    }
}
