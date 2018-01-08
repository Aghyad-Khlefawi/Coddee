// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace Coddee.CodeTools.Components.Data.Generators
{
    public abstract class TypeCodeGenerator
    {
        protected ProjectConfiguration _project;
        protected CoddeeSolutionInfo _solution;

        public void Generate(CoddeeSolutionInfo solution,ProjectConfiguration project, TableImportArgumentsViewModel args, Stream stream)
        {
            _project = project;
            _solution = solution;
            var compileUnit = new CodeCompileUnit();
            var nameSpace = new CodeNamespace(project.DefaultNamespace);
            var usingsNamespace = new CodeNamespace();
            usingsNamespace.Imports.Add(new CodeNamespaceImport("System"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
            usingsNamespace.Imports.Add(new CodeNamespaceImport("Coddee"));
            AddNamepaceImports(usingsNamespace);
            var type = CreateType(args);

            AddBaseTypes(type, args);
            AddMembers(type, args);

            nameSpace.Types.Add(type);
            compileUnit.Namespaces.Add(usingsNamespace);
            compileUnit.Namespaces.Add(nameSpace);

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BlankLinesBetweenMembers = false,
                BracingStyle = "C"
            };
            using (var sw = new StreamWriter(stream))
            {
                provider.GenerateCodeFromCompileUnit(compileUnit, sw, options);
            }
        }

        protected abstract CodeTypeDeclaration CreateType(TableImportArgumentsViewModel args);

        protected virtual void AddMembers(CodeTypeDeclaration type, TableImportArgumentsViewModel args)
        {

        }

        protected virtual void AddBaseTypes(CodeTypeDeclaration type, TableImportArgumentsViewModel args)
        {

        }

        protected abstract string GetClassName(TableImportArgumentsViewModel args);

        public byte[] Generate(CoddeeSolutionInfo solution,ProjectConfiguration project, TableImportArgumentsViewModel args)
        {
            using (var ms = new MemoryStream())
            {
                Generate(solution,project, args, ms);
                return ms.ToArray();
            }
        }

        protected virtual void AddNamepaceImports(CodeNamespace usingsNamespace)
        {

        }

        protected virtual CodeSnippetTypeMember CreateAutoProperty(string type, string name)
        {
            var snippet = new CodeSnippetTypeMember();
            if (name == "ID" && type == typeof(Int32).ToString())
                snippet.Text = $"\t\tpublic {type} {name} {{ get; set; }} = -1;\n";
            else
                snippet.Text = $"\t\tpublic {type} {name} {{ get; set; }}\n";
            return snippet;
        }
    }
}