using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace InteropHelpers
{
    internal static class AttributeHelpers
    {
        public static IAttribute GetAttibuteOfCLRType(this IEnumerable<IAttribute> attributes, ClrTypeName clrTypeName)
        {
            return attributes.FirstOrDefault(a =>
            {
                var typeReference = a.TypeReference;
                if (typeReference != null)
                {
                    var typeElement =
                        typeReference.Resolve().DeclaredElement as ITypeElement;
                    if (typeElement != null && Equals(typeElement.GetClrName(), clrTypeName))
                    {
                        return true;
                    }
                }
                return false;
            });
        }
    }
}