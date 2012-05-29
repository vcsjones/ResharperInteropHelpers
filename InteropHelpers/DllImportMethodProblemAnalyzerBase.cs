using System;
using System.Linq;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    public abstract class DllImportMethodProblemAnalyzerBase : ElementProblemAnalyzer<IMethodDeclaration>
    {
        private static readonly ClrTypeName DllImportAttribute = new ClrTypeName("System.Runtime.InteropServices.DllImportAttribute");
        protected sealed override void Run(IMethodDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
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
            Run(new DllImportMethodDeclarationImpl {ImportedDll = libraryName, MethodDeclaration = element}, data, consumer);
        }

        protected abstract void Run(IDllImportMethodDeclaration importMethod, ElementProblemAnalyzerData data, IHighlightingConsumer consumer);

        private class DllImportMethodDeclarationImpl : IDllImportMethodDeclaration
        {
            public IMethodDeclaration MethodDeclaration { get; set; }
            public string ImportedDll { get; set; }
        }
    }

    public interface IDllImportMethodDeclaration
    {
        IMethodDeclaration MethodDeclaration { get; }
        string ImportedDll { get;  }
    }
}
