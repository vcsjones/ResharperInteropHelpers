using System;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace InteropHelpers
{
    [ContextAction(Description = "Explicitly adds the entry point to an imported method.",
      Group = "C#",
      Name = "ExplicitEntryPoint")]
    public class DllImportAddExplicitEntryPoint : BulbItemImpl, IContextAction
    {
        private readonly ICSharpContextActionDataProvider _provider;
        private IAttribute _dllImportAttribute;
        private IPropertyAssignment _propertyAssignment;

        public DllImportAddExplicitEntryPoint(ICSharpContextActionDataProvider provider)
        {
            _provider = provider;
        }

        public override string Text
        {
            get
            {
                return "Explicit Entry Point";
            }
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var method = _provider.GetSelectedElement<IMethodDeclaration>(true, true);
            if (method == null)
            {
                return null;
            }
            var factory = CSharpElementFactory.GetInstance(_provider.PsiModule);
            var constantValue = new ConstantValue(method.NameIdentifier.Name, method.GetPsiModule());
            var expression = factory.CreateExpressionByConstantValue(constantValue);
            if (_propertyAssignment != null)
            {
                _propertyAssignment.SetSource(expression);
                return null;
            }
            var constructorArgs = _dllImportAttribute.ConstructorArgumentExpressions.Select(cae => new AttributeValue(cae.ConstantValue)).ToArray();
            var namedArgs = _dllImportAttribute.PropertyAssignments.Select(pa => new Pair<string, AttributeValue>(pa.PropertyNameIdentifier.Name, new AttributeValue(pa.Source.ConstantValue))).ToList();
            namedArgs.Add(new Pair<string, AttributeValue>("EntryPoint", new AttributeValue(constantValue)));
            var instance = _dllImportAttribute.GetAttributeInstance();
            var attribute = factory.CreateAttribute(instance.AttributeType.GetTypeElement(), constructorArgs, namedArgs.ToArray());
            method.AddAttributeAfter(attribute, _dllImportAttribute);
            method.RemoveAttribute(_dllImportAttribute);
            _dllImportAttribute = attribute;
            return null;
        }

        public bool IsAvailable(IUserDataHolder cache)
        {
            _propertyAssignment = null;
            _dllImportAttribute = null;
            Func<IAttribute, bool> isDllImportAttribute = a =>
            {
                var typeReference = a.TypeReference;
                if (typeReference != null)
                {
                    var typeElement = typeReference.Resolve().DeclaredElement as ITypeElement;
                    if (typeElement != null && Equals(typeElement.GetClrName(), DllImportMissingAnalyzer.DllImportAttribute))
                    {
                        return true;
                    }
                }
                return false;
            };
            var method = _provider.GetSelectedElement<IMethodDeclaration>(true, true);
            if (method == null)
            {
                return false;
            }
            var attribute = method.Attributes.FirstOrDefault(isDllImportAttribute);
            if (attribute == null || !attribute.IsValid())
            {
                return false;
            }
            _dllImportAttribute = attribute;
            var entryPoint = attribute.PropertyAssignments.FirstOrDefault(pa => pa.PropertyNameIdentifier.Name == "EntryPoint");
            if (entryPoint == null)
            {
                return true;
            }
            _propertyAssignment = entryPoint;
            var literal = entryPoint.Source as ICSharpLiteralExpression;
            if (literal != null)
            {
                if (!literal.IsConstantValue())
                {
                    return true;
                }
                var value = literal.ConstantValue.Value as string;
                return value != method.NameIdentifier.Name;
            }
            return false;
        }
    }
}
