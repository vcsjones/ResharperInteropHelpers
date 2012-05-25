using System;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace InteropHelpers
{
    [QuickFix]
    public sealed class DllImportMissingRemoveExternQuickFix : BulbItemImpl, IQuickFix
    {
        private readonly DllImportMissingHighlighting _highlighting;

        public DllImportMissingRemoveExternQuickFix(DllImportMissingHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var methodDeclaration = _highlighting.MethodDeclaration;
            methodDeclaration.SetExtern(false);
            var textRange = LanguageManager.Instance.GetService<IMemberBodyOperations>(methodDeclaration.Language).SetBodyToDefault(methodDeclaration);
            var rangeMarker = new DocumentRange(methodDeclaration.GetDocumentRange().Document, textRange).CreateRangeMarker(DocumentManager.GetInstance(solution));
            return control => control.Selection.SetRange(rangeMarker.Range);
        }

        public override string Text
        {
            get { return "Make the method non-extern"; }
        }
    }
}
