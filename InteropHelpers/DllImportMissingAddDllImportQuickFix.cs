using System;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Application.Progress;
using JetBrains.DocumentManagers;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace InteropHelpers
{
    [QuickFix]
    public sealed class DllImportMissingAddDllImportQuickFix : BulbItemImpl, IQuickFix
    {
        private readonly DllImportMissingHighlighting _highlighting;

        public DllImportMissingAddDllImportQuickFix(DllImportMissingHighlighting highlighting)
        {
            _highlighting = highlighting;
        }

        public bool IsAvailable(IUserDataHolder cache)
        {
            return _highlighting.IsValid();
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var methodDeclaration = _highlighting.MethodDeclaration;
            methodDeclaration.SetStatic(true);
            var factory = CSharpElementFactory.GetInstance(methodDeclaration.GetPsiModule());
            var type = TypeFactory.CreateTypeByCLRName(DllImportMissingAnalyzer.DllImportAttribute, methodDeclaration.GetPsiModule());
            var constantValue = new ConstantValue("name.dll", methodDeclaration.GetPsiModule());
            var attributeValue = new AttributeValue(constantValue);
            var attribute = factory.CreateAttribute(type.GetTypeElement(), new[] { attributeValue }, new Pair<string, AttributeValue>[0]);
            var addedAttribute = methodDeclaration.AddAttributeAfter(attribute, null);
            var firstParameter = addedAttribute.ConstructorArgumentExpressions.FirstOrDefault();
            if (firstParameter == null)
            {
                return null;
            }
            var documentRange = firstParameter.GetDocumentRange();
            documentRange = documentRange.TrimLeft(1).TrimRight(1);
            var rangeMarker =  documentRange.CreateRangeMarker(DocumentManager.GetInstance(solution));
            return control => control.Selection.SetRange(rangeMarker.Range);
        }

        public override string Text
        {
            get { return "Add [DllImport] attribute"; }
        }
    }
}
