using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    [StaticSeverityHighlighting(Severity.WARNING, CSharpLanguage.Name)]
    public class DllImportMissingHighlighting : IHighlightingWithRange
    {
        public IMethodDeclaration MethodDeclaration { get; private set; }

        public DllImportMissingHighlighting(IMethodDeclaration methodDeclaration)
        {
            MethodDeclaration = methodDeclaration;
        }

        public string ToolTip
        {
            get { return "Extern method does not have a DllImportAttribute."; }
        }

        public string ErrorStripeToolTip
        {
            get { return "Extern method does not have a DllImportAttribute."; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }

        public DocumentRange CalculateRange()
        {
            return MethodDeclaration == null ? DocumentRange.InvalidRange : MethodDeclaration.GetHighlightingRange();
        }

        public bool IsValid()
        {
            return MethodDeclaration != null && MethodDeclaration.IsValid();
        }
    }
}
