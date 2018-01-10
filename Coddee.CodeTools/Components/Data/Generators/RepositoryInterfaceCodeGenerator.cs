// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.CodeDom;

namespace Coddee.CodeTools.Components.Data.Generators
{
    public class RepositoryInterfaceCodeGenerator : InterfaceCodeGenerator
    {
        protected override void AddNamepaceImports(CodeNamespace usingsNamespace)
        {

            base.AddNamepaceImports(usingsNamespace);
            usingsNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport("System.Threading.Tasks"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport("Coddee.Data"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport(_solution.ModelProjectConfiguration.DefaultNamespace));


        }

        protected override string GetClassName(TableImportArgumentsViewModel args)
        {
            return $"I{args.SingularName}Repository";
        }

        protected override void AddBaseTypes(CodeTypeDeclaration type, TableImportArgumentsViewModel args)
        {
            base.AddBaseTypes(type, args);
            var primaryKeyType = args.GetPrimaryKeyType();
            if (primaryKeyType != null)
                type.BaseTypes.Add(new CodeTypeReference(args.SelectedBaseRepositoryType.Name, new CodeTypeReference(args.SingularName), new CodeTypeReference(primaryKeyType)));
        }
    }
}