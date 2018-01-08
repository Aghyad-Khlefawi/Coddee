using System.CodeDom;
using System.Reflection;

namespace Coddee.CodeTools.Components.Data.Generators
{
    public abstract class ClassCodeGenerator : TypeCodeGenerator
    {
        protected override CodeTypeDeclaration CreateType(TableImportArgumentsViewModel args)
        {
            return new CodeTypeDeclaration(GetClassName(args))
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Class
            };
        }
    }
}