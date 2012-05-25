using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;
using JetBrains.Util;

namespace InteropHelpers
{
    [QuickFix]
    public sealed class ImportedMethodIsNotExternOrStaticQuickFix : BulbItemImpl, IQuickFix
    {
        private readonly ImportedMethodIsNotExternOrStaticHighlighting _highlighting;

        public ImportedMethodIsNotExternOrStaticQuickFix(ImportedMethodIsNotExternOrStaticHighlighting highlighting)
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
            methodDeclaration.SetExtern(true);
            methodDeclaration.SetStatic(true);
            return null;
        }

        public override string Text
        {
            get { return "Make the imported method extern static"; }
        }
    }
}
