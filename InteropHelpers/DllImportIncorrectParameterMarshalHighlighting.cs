using System.Runtime.InteropServices;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace InteropHelpers
{
    [StaticSeverityHighlighting(Severity.WARNING, CSharpLanguage.Name)]
    public class DllImportIncorrectParameterMarshalHighlighting : IHighlightingWithRange
    {
        public IParameterDeclaration ParameterDeclaration { get; private set; }
        public ICSharpExpression ParameterExpression { get; private set; }
        public UnmanagedType UnmanagedType { get; private set; }

        public DllImportIncorrectParameterMarshalHighlighting(IParameterDeclaration parameterDeclaration, ICSharpExpression parameterExpression, UnmanagedType unmanagedType)
        {
            ParameterDeclaration = parameterDeclaration;
            ParameterExpression = parameterExpression;
            UnmanagedType = unmanagedType;
        }

        public string ToolTip
        {
            get { return "Incorrect marshalling for parameter."; }
        }

        public string ErrorStripeToolTip
        {
            get { return "Incorrect marshalling for parameter."; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }

        public DocumentRange CalculateRange()
        {
            if (ParameterDeclaration == null)
            {
                return DocumentRange.InvalidRange;
            }
            return ParameterExpression.GetDocumentRange();
        }

        public bool IsValid()
        {
            return ParameterDeclaration != null && ParameterDeclaration.IsValid();
        }
    }
}