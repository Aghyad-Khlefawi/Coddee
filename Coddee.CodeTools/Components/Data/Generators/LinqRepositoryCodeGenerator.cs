// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.CodeDom;
using System.IO;
using Coddee.Data;
using Coddee.Data.LinqToSQL;

namespace Coddee.CodeTools.Components.Data.Generators
{
    public class LinqRepositoryCodeGenerator : ClassCodeGenerator
    {
        protected override void AddNamepaceImports(CodeNamespace usingsNamespace)
        {
            base.AddNamepaceImports(usingsNamespace);
            usingsNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport("System.Threading.Tasks"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport("Coddee.Data"));
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
            var dbmlClass = Path.GetFileName(_solution.DatabaseConfigurations.DbmlPath) + "DataContext";
            var dbClass = $"DB.{args.SingularName}";
            if (dbClass.EndsWith("Status"))
                dbClass = dbClass.Replace("Status", "Statuse");

            if (args.SelectedBaseRepositoryType == typeof(ICRUDRepository<,>))
            {
                if (_solution.LinqProjectConfiguration.UseCutomCrudBase)
                    type.BaseTypes.Add(new CodeTypeReference(_solution.LinqProjectConfiguration.SelectedLinqCrudBase, new CodeTypeReference(dbClass), new CodeTypeReference(args.SingularName), new CodeTypeReference(primaryKeyType)));
                else
                    type.BaseTypes.Add(new CodeTypeReference(typeof(CRUDLinqRepositoryBase<,,,>).Name, new CodeTypeReference($"DB.{dbmlClass}"), new CodeTypeReference(dbClass), new CodeTypeReference(args.SingularName), new CodeTypeReference(primaryKeyType)));
            }
            else if (args.SelectedBaseRepositoryType == typeof(IReadOnlyRepository<,>))
            {
                if (_solution.LinqProjectConfiguration.UseCutomCrudBase)
                    type.BaseTypes.Add(new CodeTypeReference(_solution.LinqProjectConfiguration.SelectedLinqReadBase, new CodeTypeReference(dbClass), new CodeTypeReference(args.SingularName), new CodeTypeReference(primaryKeyType)));
                else
                    type.BaseTypes.Add(new CodeTypeReference(typeof(ReadOnlyLinqRepositoryBase<,,,>).Name, new CodeTypeReference($"DB.{dbmlClass}"), new CodeTypeReference(dbClass), new CodeTypeReference(args.SingularName), new CodeTypeReference(primaryKeyType)));
            }
            else
            {
                type.BaseTypes.Add(new CodeTypeReference(typeof(LinqRepositoryBase<,,,>).Name, new CodeTypeReference($"DB.{dbmlClass}"), new CodeTypeReference(dbClass), new CodeTypeReference(args.SingularName), new CodeTypeReference(primaryKeyType)));
            }

            var interfaceName = $"I{args.SingularName}Repository";
            type.BaseTypes.Add(new CodeTypeReference(interfaceName));

            var attr = new CodeAttributeDeclaration(new CodeTypeReference(typeof(RepositoryAttribute)));
            attr.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(interfaceName)));
            type.CustomAttributes.Add(attr);
        }
    }
}