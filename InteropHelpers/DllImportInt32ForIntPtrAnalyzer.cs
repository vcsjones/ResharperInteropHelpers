using System;
using System.Linq;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    [ElementProblemAnalyzer(new[] { typeof(IMethodDeclaration) }, HighlightingTypes = new[] { typeof(DllImportMissingHighlighting) })]
    public class DllImportInt32ForIntPtrAnalyzer : ElementProblemAnalyzer<IMethodDeclaration>
    {
        private static readonly ClrTypeName IntPtrClrType = new ClrTypeName("System.IntPtr");
        protected override void Run(IMethodDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            Func<IAttribute, bool> isDllImportAttribute = a =>
            {
                var typeReference = a.TypeReference;
                if (typeReference != null)
                {
                    var typeElement = typeReference.Resolve().DeclaredElement as ITypeElement;
                    if (typeElement != null && Equals(typeElement.GetClrName(), DllImportMissingAnalyzer.DllImportAttribute))
                    {
                        return true;
                    }
                }
                return false;
            };
            var attributes = element.Attributes;
            var dllImportAttribute = attributes.FirstOrDefault(isDllImportAttribute);
            if (!element.IsExtern || !element.IsStatic || dllImportAttribute == null || dllImportAttribute.ConstructorArgumentExpressions.Count == 0)
            {
                return;
            }
            var libraryName = dllImportAttribute.ConstructorArgumentExpressions[0].ConstantValue.Value as string;
            if (libraryName == null)
            {
                return;
            }
            var factory = new LibraryFactory();
            var library = factory.LoadLibrary(libraryName);
            var export = library.GetExport(element.NameIdentifier.Name);
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
                var exportedType = new ClrTypeName(knownParameter.ClrType);
                
                if (exportedType.Equals(IntPtrClrType))
                {
                    consumer.AddHighlighting(new DllImportInt32ForIntPtrHighlighting(parameter));
                }
            }
        }
    }
}
