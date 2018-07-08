﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Lists the sub types of a class.
	/// </summary>
	sealed class DerivedTypesTreeNode : ILSpyTreeNode
	{
		readonly AssemblyList list;
		readonly ITypeDefinition type;
		readonly ThreadingSupport threading;

		public DerivedTypesTreeNode(AssemblyList list, ITypeDefinition type)
		{
			this.list = list;
			this.type = type;
			this.LazyLoading = true;
			this.threading = new ThreadingSupport();
		}

		public override object Text => "Derived Types";

		public override object Icon => Images.SubTypes;

		protected override void LoadChildren()
		{
			threading.LoadChildren(this, FetchChildren);
		}

		IEnumerable<ILSpyTreeNode> FetchChildren(CancellationToken cancellationToken)
		{
			// FetchChildren() runs on the main thread; but the enumerator will be consumed on a background thread
			var assemblies = list.GetAssemblies().Select(node => node.GetPEFileOrNull()).Where(asm => asm != null).ToArray();
			return FindDerivedTypes(type, assemblies, cancellationToken);
		}

		internal static IEnumerable<DerivedTypesEntryNode> FindDerivedTypes(ITypeDefinition type, PEFile[] assemblies, CancellationToken cancellationToken)
		{
			/*foreach (var module in assemblies) {
				var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
				foreach (var td in typeSystem.MainAssembly.TypeDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					var td = new TypeDefinition(module, h);
					var typeDefinition = metadata.GetTypeDefinition(h);
					foreach (var iface in typeDefinition.GetInterfaceImplementations()) {
						var ifaceImpl = metadata.GetInterfaceImplementation(iface);
						if (IsSameType(metadata, ifaceImpl.Interface, type))
							yield return new DerivedTypesEntryNode(td, assemblies);
					}
					if (!typeDefinition.BaseType.IsNil && IsSameType(metadata, typeDefinition.BaseType, type)) {
						yield return new DerivedTypesEntryNode(td, assemblies);
					}
				}
			}*/
			yield break;
		}
		/*
		static bool IsSameType(SRM.MetadataReader referenceMetadata, SRM.EntityHandle typeRef, ITypeDefinition type)
		{
			// FullName contains only namespace, name and type parameter count, therefore this should suffice.
			return typeRef.GetFullTypeName(referenceMetadata) == type.Handle.GetFullTypeName(type.Module.Metadata);
		}*/

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			threading.Decompile(language, output, options, EnsureLazyChildren);
		}
	}
}