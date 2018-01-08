using System.CodeDom;
using System.Reflection;

namespace Coddee.CodeTools.Components.Data.Generators
{
    public abstract class InterfaceCodeGenerator : TypeCodeGenerator
    {
        protected override CodeTypeDeclaration CreateType(TableImportArgumentsViewModel args)
        {
            return new CodeTypeDeclaration(GetClassName(args))
            {
                IsClass = false,
                IsInterface = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Interface
            };
        }
    }
}