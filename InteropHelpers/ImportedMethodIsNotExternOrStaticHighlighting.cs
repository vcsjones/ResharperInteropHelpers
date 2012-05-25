using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    [StaticSeverityHighlighting(Severity.WARNING, CSharpLanguage.Name)]
    public class ImportedMethodIsNotExternOrStaticHighlighting : IHighlightingWithRange
    {
        public IMethodDeclaration MethodDeclaration { get; private set; }

        public ImportedMethodIsNotExternOrStaticHighlighting(IMethodDeclaration methodDeclaration)
        {
            MethodDeclaration = methodDeclaration;
        }

        public string ToolTip
        {
            get { return "Imported method should be extern static."; }
        }

        public string ErrorStripeToolTip
        {
            get { return "Imported method should be extern static."; }
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