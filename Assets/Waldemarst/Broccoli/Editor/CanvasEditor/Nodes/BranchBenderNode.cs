﻿using UnityEngine;
using UnityEditor;
using Broccoli.NodeEditorFramework;
using Broccoli.NodeEditorFramework.Utilities;
using Broccoli.Pipe;

namespace Broccoli.TreeNodeEditor
{
	/// <summary>
	/// Branch bender node.
	/// </summary>
	[Node (false, "Structure Transformer/Branch Bender", 50)]
	public class BranchBenderNode : BaseNode
	{
		#region Vars
		/// <summary>
		/// Get the Id of the Node.
		/// </summary>
		/// <value>Id of the node.</value>
		public override string GetID { 
			get { return typeof (BranchBenderNode).ToString(); } 
		}
		/// <summary>
		/// Gets the category of the node.
		/// </summary>
		/// <value>Category of the node.</value>
		public override Category category { get { return Category.StructureTransformer; } }
		/// <summary>
		/// The branch bender pipeline element.
		/// </summary>
		public BranchBenderElement branchBenderElement;
		#endregion

		#region Constructor
		/// <summary>
		/// Node constructor.
		/// </summary>
		public BranchBenderNode () {
			this.nodeName = "Branch Bender";
			this.nodeHelpURL = "https://docs.google.com/document/d/1Nr6Z808i7X2zMFq8PELezPuSJNP5IvRx9C5lJxZ_Z-A/edit#heading=h.387o5fyl8jjk";
			this.nodeDescription = "This node contains the parameters to bend existing branches on the tree structure.";
		}
		#endregion

		#region Base Node
		/// <summary>
		/// Called when creating the node.
		/// </summary>
		/// <returns>The created node.</returns>
		protected override BaseNode CreateExplicit () {
			BranchBenderNode node = CreateInstance<BranchBenderNode> ();
			node.rectSize.x = 130;
			node.name = "Branch Bender";
			return node;
		}
		/// <summary>
		/// Sets the pipeline element of this node.
		/// </summary>
		/// <param name="pipelineElement">Pipeline element.</param>
		public override void SetPipelineElement (PipelineElement pipelineElement = null) {
			if (pipelineElement == null) {
				branchBenderElement = ScriptableObject.CreateInstance<BranchBenderElement> ();
			} else {
				branchBenderElement = (BranchBenderElement)pipelineElement;
			}
			this.pipelineElement = branchBenderElement;
		}
		#endregion
	}
}