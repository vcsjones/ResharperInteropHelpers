using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace InteropHelpers
{
    [StaticSeverityHighlighting(Severity.WARNING, CSharpLanguage.Name)]
    public class DllImportInt32ForIntPtrHighlighting : IHighlightingWithRange
    {
        public IParameterDeclaration ParameterDeclaration { get; private set; }

        public DllImportInt32ForIntPtrHighlighting(IParameterDeclaration parameterDeclaration)
        {
            ParameterDeclaration = parameterDeclaration;
        }

        public string ToolTip
        {
            get { return "Parameter is declared as 32-bit integer for platform-dependent value."; }
        }

        public string ErrorStripeToolTip
        {
            get { return "Parameter is declared as 32-bit integer for platform-dependent value."; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }

        public DocumentRange CalculateRange()
        {
            return ParameterDeclaration == null ? DocumentRange.InvalidRange : ParameterDeclaration.GetHighlightingRange();
        }

        public bool IsValid()
        {
            return ParameterDeclaration != null && ParameterDeclaration.IsValid();
        }
    }
}
