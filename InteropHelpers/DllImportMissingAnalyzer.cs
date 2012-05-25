using System;
using System.Linq;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    [ElementProblemAnalyzer(new[] { typeof(IMethodDeclaration) }, HighlightingTypes = new[] {typeof(DllImportMissingHighlighting)})]
    public class DllImportMissingAnalyzer : ElementProblemAnalyzer<IMethodDeclaration>
    {
        internal static readonly ClrTypeName DllImportAttribute = new ClrTypeName("System.Runtime.InteropServices.DllImportAttribute");
        protected override void Run(IMethodDeclaration element, ElementProblemAnalyzerData analyzerData, IHighlightingConsumer consumer)
        {
            Func<IAttribute, bool> isDllImportAttribute = a =>
            {
                var typeReference = a.TypeReference;
                if (typeReference != null)
                {
                    var typeElement = typeReference.Resolve().DeclaredElement as ITypeElement;
                    if (typeElement != null && Equals(typeElement.GetClrName(), DllImportAttribute))
                    {
                        return true;
                    }
                }
                return false;
            };
            var attributes = element.Attributes;
            var hasDllImportAttribute = attributes.Any(isDllImportAttribute);
            if (element.IsExtern && !hasDllImportAttribute)
            {
                consumer.AddHighlighting(new DllImportMissingHighlighting(element));
            }
            if ((!element.IsStatic || !element.IsExtern) && hasDllImportAttribute)
            {
                consumer.AddHighlighting(new ImportedMethodIsNotExternOrStaticHighlighting(element));
            }
        }
    }
}
