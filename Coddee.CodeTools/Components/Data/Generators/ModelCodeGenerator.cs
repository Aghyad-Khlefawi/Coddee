// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.CodeDom;
using System.Linq;

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
    public class ModelCodeGenerator : ClassCodeGenerator
    {
        protected override void AddMembers(CodeTypeDeclaration type, TableImportArgumentsViewModel args)
        {
            var modelConfig = (ModelProjectConfiguration)_project;
            foreach (var column in args.Columns)
            {
                type.Members.Add(CreateAutoProperty(column.Type.ToString(), column.Name));
            }

            if (modelConfig.AdditionalProperties != null && modelConfig.AdditionalProperties.Any())
            {
                foreach (var additionalProperty in modelConfig.AdditionalProperties)
                {
                    type.Members.Add(CreateAutoProperty(additionalProperty.Type, additionalProperty.Name));
                }
            }
            if (args.Columns.Count(e => e.IsPrimaryKey) == 1)
            {
                var primary = args.Columns.First(e => e.IsPrimaryKey);
                var getKey = new CodeSnippetTypeMember();
                getKey.Text = $"\t\t[IgnoreDataMember]\n\t\tpublic {primary.Type} GetKey =>{primary.Name};\n";
                type.Members.Add(getKey);
                var unique = new CodeTypeReference(typeof(IUniqueObject<>).Name, new CodeTypeReference(primary.Type));
                GenerateEqualsMethod(type, primary.Type);
                GenerateGetHashCodeMethod(type);
                GenerateEqualsIUniqueMethod(type, unique, primary.Type);
                GenerateEqualsModelMethod(type, primary.Type);
            }
        }

        protected override void AddBaseTypes(CodeTypeDeclaration type, TableImportArgumentsViewModel args)
        {
            var modelConfig = (ModelProjectConfiguration)_project;
            if (!string.IsNullOrWhiteSpace(modelConfig.AdditionalInterfaces))
            {
                foreach (var interf in modelConfig.AdditionalInterfaces.Split(';'))
                {
                    type.BaseTypes.Add(new CodeTypeReference(interf));
                }
            }
            if (args.Columns.Count(e => e.IsPrimaryKey) == 1)
            {
                var primary = args.Columns.First(e => e.IsPrimaryKey);
                var unique = new CodeTypeReference(typeof(IUniqueObject<>).Name, new CodeTypeReference(primary.Type));
                type.BaseTypes.Add(unique);
                type.BaseTypes.Add(new CodeTypeReference(typeof(IEquatable<>).Name, unique));
                type.BaseTypes.Add(new CodeTypeReference(typeof(IEquatable<>).Name, new CodeTypeReference(GetClassName(args))));
            }
        }

        private static void GenerateEqualsMethod(CodeTypeDeclaration targetClass, Type primaryType)
        {
            var equals = new CodeMemberMethod();
            equals.Name = "Equals";
            equals.ReturnType = new CodeTypeReference(typeof(bool));
            equals.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            var equalsParam = new CodeParameterDeclarationExpression(typeof(object), "obj");
            equals.Parameters.Add(equalsParam);
            var declareOther = new CodeSnippetStatement($"\t\t\tvar other = obj as IUniqueObject<{primaryType}>;");
            equals.Statements.Add(declareOther);
            var checkOther = new CodeConditionStatement(
                                                        new CodeVariableReferenceExpression("other != null"),
                                                        new CodeStatement[]
                                                        {
                                                            new
                                                                CodeMethodReturnStatement(new
                                                                                              CodeMethodInvokeExpression(new
                                                                                                                             CodeMethodReferenceExpression(new
                                                                                                                                                               CodeThisReferenceExpression(),
                                                                                                                                                           "Equals"),
                                                                                                                         new
                                                                                                                             CodeVariableReferenceExpression("other"))),
                                                        },
                                                        new CodeStatement[]
                                                        {
                                                            new
                                                                CodeMethodReturnStatement(new
                                                                                              CodeMethodInvokeExpression(new
                                                                                                                             CodeMethodReferenceExpression(new
                                                                                                                                                               CodeBaseReferenceExpression(),
                                                                                                                                                           "Equals"),
                                                                                                                         new
                                                                                                                             CodeVariableReferenceExpression("obj"))),
                                                        });
            equals.Statements.Add(checkOther);
            targetClass.Members.Add(equals);
        }
        private static void GenerateGetHashCodeMethod(CodeTypeDeclaration targetClass)
        {
            var getHashCode = new CodeMemberMethod();
            getHashCode.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            getHashCode.ReturnType = new CodeTypeReference(typeof(int));
            getHashCode.Name = "GetHashCode";
            getHashCode.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "GetKey"), "GetHashCode")));
            targetClass.Members.Add(getHashCode);
        }
        private static void GenerateEqualsModelMethod(CodeTypeDeclaration targetClass, Type modelKeyType)
        {
            var equalKeys = new CodeMethodReturnStatement(new
                                                              CodeBinaryOperatorExpression(new
                                                                                               CodePropertyReferenceExpression(new
                                                                                                                                   CodeThisReferenceExpression(),
                                                                                                                               "GetKey"),
                                                                                           CodeBinaryOperatorType
                                                                                               .ValueEquality,
                                                                                           new
                                                                                               CodePropertyReferenceExpression(new
                                                                                                                                   CodeVariableReferenceExpression("other"),
                                                                                                                               "GetKey")));

            var equalRef = new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(object)), "ReferenceEquals", new CodeThisReferenceExpression(), new CodeVariableReferenceExpression("other")));


            var equals = new CodeMemberMethod();
            equals.Name = "Equals";
            equals.ReturnType = new CodeTypeReference(typeof(bool));
            equals.Attributes = MemberAttributes.Public;
            var equalsParam = new CodeParameterDeclarationExpression(new CodeTypeReference(targetClass.Name), "other");
            equals.Parameters.Add(equalsParam);

            string checkValue = String.Empty;
            if (modelKeyType == typeof(Int64) || modelKeyType == typeof(Int32) || modelKeyType == typeof(Int16))
                checkValue = "-1";
            else if (modelKeyType == typeof(Guid))
                checkValue = "Guid.Empty";
            else if (modelKeyType == typeof(string))
                checkValue = "string.Empty";

            var firstCheck =
                new
                    CodeBinaryOperatorExpression(new
                                                     CodePropertyReferenceExpression(new
                                                                                         CodeThisReferenceExpression(),
                                                                                     "GetKey"),
                                                 CodeBinaryOperatorType.IdentityInequality,
                                                 new CodeSnippetExpression(checkValue));
            var secondCheck =
                new
                    CodeBinaryOperatorExpression(new
                                                     CodePropertyReferenceExpression(new
                                                                                         CodeVariableReferenceExpression("other"),
                                                                                     "GetKey"),
                                                 CodeBinaryOperatorType.IdentityInequality,
                                                 new CodeSnippetExpression(checkValue));
            var check = new CodeConditionStatement(new CodeBinaryOperatorExpression(firstCheck, CodeBinaryOperatorType.BooleanAnd, secondCheck),
                                                   new[] { equalKeys },
                                                   new[] { equalRef });
            if (!string.IsNullOrWhiteSpace(checkValue))
                equals.Statements.Add(check);
            else
                equals.Statements.Add(firstCheck);

            targetClass.Members.Add(equals);
        }

        private static void GenerateEqualsIUniqueMethod(CodeTypeDeclaration targetClass, CodeTypeReference unique, Type modelKeyType)
        {


            var equalKeys = new CodeMethodReturnStatement(new
                                              CodeBinaryOperatorExpression(new
                                                                               CodePropertyReferenceExpression(new
                                                                                                                   CodeThisReferenceExpression(),
                                                                                                               "GetKey"),
                                                                           CodeBinaryOperatorType
                                                                               .ValueEquality,
                                                                           new
                                                                               CodePropertyReferenceExpression(new
                                                                                                                   CodeVariableReferenceExpression("other"),
                                                                                                               "GetKey")));

            var equalRef = new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(object)), "ReferenceEquals", new CodeThisReferenceExpression(), new CodeVariableReferenceExpression("other")));

            var equals = new CodeMemberMethod();
            equals.Name = "Equals";
            equals.ReturnType = new CodeTypeReference(typeof(bool));
            equals.Attributes = MemberAttributes.Public;
            var equalsParam = new CodeParameterDeclarationExpression(unique, "other");
            equals.Parameters.Add(equalsParam);


            string checkValue = String.Empty;
            if (modelKeyType == typeof(Int64) || modelKeyType == typeof(Int32) || modelKeyType == typeof(Int16))
                checkValue = "-1";
            else if (modelKeyType == typeof(Guid))
                checkValue = "Guid.Empty";
            else if (modelKeyType == typeof(string))
                checkValue = "string.Empty";

            var firstCheck =
                new
                    CodeBinaryOperatorExpression(new
                                                     CodePropertyReferenceExpression(new
                                                                                         CodeThisReferenceExpression(),
                                                                                     "GetKey"),
                                                 CodeBinaryOperatorType.IdentityInequality,
                                                 new CodeSnippetExpression(checkValue));
            var secondCheck =
                new
                    CodeBinaryOperatorExpression(new
                                                     CodePropertyReferenceExpression(new
                                                                                         CodeVariableReferenceExpression("other"),
                                                                                     "GetKey"),
                                                 CodeBinaryOperatorType.IdentityInequality,
                                                 new CodeSnippetExpression(checkValue));
            var check = new CodeConditionStatement(new CodeBinaryOperatorExpression(firstCheck, CodeBinaryOperatorType.BooleanAnd, secondCheck),
                                                   new[] { equalKeys },
                                                   new[] { equalRef });

            if (!string.IsNullOrWhiteSpace(checkValue))
                equals.Statements.Add(check);
            else
                equals.Statements.Add(firstCheck);
            targetClass.Members.Add(equals);
        }

        protected override string GetClassName(TableImportArgumentsViewModel args)
        {
            return args.SingularName;
        }
    }
}
