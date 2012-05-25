using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;

namespace InteropHelpers
{
    [QuickFix]
    public sealed class DllImportInt32ForIntPtrQuickFix : BulbItemImpl, IQuickFix
    {
        private readonly DllImportInt32ForIntPtrHighlighting _highlighting;

        public DllImportInt32ForIntPtrQuickFix(DllImportInt32ForIntPtrHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public bool IsAvailable(IUserDataHolder cache)
        {
            return _highlighting.IsValid();
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var type = TypeFactory.CreateTypeByCLRName("System.IntPtr", _highlighting.ParameterDeclaration.GetPsiModule());
            _highlighting.ParameterDeclaration.SetType(type);
            return null;
        }

        public override string Text
        {
            get { return "Change to System.IntPtr"; }
        }
    }
}
