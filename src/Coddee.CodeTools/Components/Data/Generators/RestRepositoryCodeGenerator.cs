// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.CodeDom;
using Coddee.Data;
using Coddee.Data.REST;

namespace Coddee.CodeTools.Components.Data.Generators
{
    public class RestRepositoryCodeGenerator : ClassCodeGenerator
    {
        protected override void AddNamepaceImports(CodeNamespace usingsNamespace)
        {
            base.AddNamepaceImports(usingsNamespace);
            usingsNamespace.Imports.Add(new CodeNamespaceImport("Coddee.Data.REST"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport(_solution.ModelProjectConfiguration.DefaultNamespace));
            usingsNamespace.Imports.Add(new CodeNamespaceImport(_solution.DataProjectConfiguration.DefaultNamespace));
        }

        protected override string GetClassName(TableImportArgumentsViewModel args)
        {
            return $"{args.SingularName}Repository";
        }

        protected override void AddBaseTypes(CodeTypeDeclaration type, TableImportArgumentsViewModel args)
        {
            base.AddBaseTypes(type, args);
            var primaryKeyType = args.GetPrimaryKeyType();
            if (args.SelectedBaseRepositoryType == typeof(ICRUDRepository<,>))
            {
                type.BaseTypes.Add(new CodeTypeReference(typeof(CRUDRESTRepositoryBase<,>).Name, new CodeTypeReference(args.ModelName), new CodeTypeReference(primaryKeyType)));
            }
            else if (args.SelectedBaseRepositoryType == typeof(IReadOnlyRepository<,>))
            {
                type.BaseTypes.Add(new CodeTypeReference(typeof(ReadOnlyRESTRepositoryBase<,>).Name, new CodeTypeReference(args.ModelName), new CodeTypeReference(primaryKeyType)));
            }
            else
            {
                type.BaseTypes.Add(new CodeTypeReference(typeof(RESTRepositoryBase<,>).Name, new CodeTypeReference(args.ModelName), new CodeTypeReference(primaryKeyType)));
            }

            var interfaceName = $"I{args.SingularName}Repository";
            type.BaseTypes.Add(new CodeTypeReference(interfaceName));

            var attr = new CodeAttributeDeclaration(new CodeTypeReference(typeof(RepositoryAttribute)));
            attr.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(interfaceName)));
            type.CustomAttributes.Add(attr);

            var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
            constructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(args.SingularName));
            type.Members.Add(constructor);
        }
    }
}