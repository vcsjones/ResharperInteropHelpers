using System;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    [ElementProblemAnalyzer(new[] { typeof(IMethodDeclaration) }, HighlightingTypes = new[] { typeof(DllImportMissingHighlighting) })]
    public class DllImportParameterMarshallingAnalyzer : DllImportMethodProblemAnalyzerBase
    {
        private static readonly ClrTypeName MarshalAsAttribute = new ClrTypeName("System.Runtime.InteropServices.MarshalAsAttribute");
        private static readonly ClrTypeName IntPtrClrType = new ClrTypeName("System.IntPtr");
        protected override void Run(IDllImportMethodDeclaration importMethod, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {

            var element = importMethod.MethodDeclaration;
            var libraryName = importMethod.ImportedDll;
            var factory = new LibraryFactory();
            var library = factory.LoadLibrary(libraryName);
            var export = library[element.NameIdentifier.Name];
            if (export == null || export.Parameters.Count != element.ParameterDeclarations.Count)
            {
                return;
            }

            for (var i = 0; i < element.ParameterDeclarations.Count; i++ )
            {
                var parameter = element.ParameterDeclarations[i];
                var knownParameter = export.Parameters[i];
                var exportedType = new ClrTypeName(knownParameter.CLRType.FullName);
                if (parameter.Type.IsInt() && exportedType.Equals(IntPtrClrType))
                {
                    consumer.AddHighlighting(new DllImportInt32ForIntPtrHighlighting(parameter));
                }
                var marshalAs = parameter.Attributes.GetAttibuteOfCLRType(MarshalAsAttribute);
                if (knownParameter.UnmanagedType.HasValue && marshalAs != null && marshalAs.ConstructorArgumentExpressions.Count == 1)
                {
                    var argumentExpression = marshalAs.ConstructorArgumentExpressions[0];
                    if (!argumentExpression.IsConstantValue())
                    {
                        continue;
                    }
                    var shortType = argumentExpression.ConstantValue.IsShort();
                    UnmanagedType unmanagedType;
                    if (shortType)
                    {
                        unmanagedType = (UnmanagedType)(short)argumentExpression.ConstantValue.Value;
                    }
                    else
                    {
                        unmanagedType = (UnmanagedType)argumentExpression.ConstantValue.Value;
                    }
                    if (knownParameter.UnmanagedType.Value != unmanagedType)
                    {
                        consumer.AddHighlighting(new DllImportIncorrectParameterMarshalHighlighting(parameter, argumentExpression, knownParameter.UnmanagedType.Value));
                    }
                }
            }
        }
    }
}
