using System;
using System.Runtime.InteropServices;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace InteropHelpers
{
    [QuickFix]
    public sealed class DllImportIncorrectParameterMarshalQuickFix : BulbItemImpl, IQuickFix
    {
        private readonly DllImportIncorrectParameterMarshalHighlighting _highlighting;

        public DllImportIncorrectParameterMarshalQuickFix(DllImportIncorrectParameterMarshalHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public bool IsAvailable(IUserDataHolder cache)
        {
            return _highlighting.IsValid();
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var reference = _highlighting.ParameterExpression as IReferenceExpression;
            if (reference == null)
            {
                return null;
            }
            CSharpImplUtil.ReplaceIdentifier(reference.NameIdentifier, Enum.GetName(typeof(UnmanagedType), _highlighting.UnmanagedType));
            return null;
        }

        public override string Text
        {
            get { return "Marshal to " + _highlighting.UnmanagedType; }
        }
    }
}