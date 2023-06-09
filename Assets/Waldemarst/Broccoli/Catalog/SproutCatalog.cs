﻿using System.Collections.Generic;

using UnityEngine;

using Broccoli.Base;
using Broccoli.Utils;

namespace Broccoli.Catalog
{
	/// <summary>
	/// Sprout lab manager class.
	/// </summary>
	[System.Serializable]
	public class SproutCatalog {
		#region CatalogItem class
		/// <summary>
		/// Catalog item.
		/// </summary>
		[System.Serializable]
		public class CatalogItem {
			/// <summary>
			/// Name of the item.
			/// </summary>
			public string name = "";
			/// <summary>
			/// The thumbnail texture.
			/// </summary>
			public Texture2D thumb = null;
			/// <summary>
			/// The path to the pipeline asset.
			/// </summary>
			public string path = "";
			/// <summary>
			/// The name of the category the item belongs to.
			/// </summary>
			public string category = "";
			/// <summary>
			/// The order within the category.
			/// </summary>
			public int order = 0;
		}
		#endregion

		#region Vars
		/// <summary>
		/// The total items.
		/// </summary>
		int _totalItems = 0;
		/// <summary>
		/// The total categories.
		/// </summary>
		int _totalCategories = 0;
		/// <summary>
		/// Gets the total items.
		/// </summary>
		/// <value>The total items.</value>
		public int totalItems {
			get { return _totalItems; }
		}
		/// <summary>
		/// Gets the total categories.
		/// </summary>
		/// <value>The total categories.</value>
		public int totalCategories {
			get { return _totalCategories; }
		}
		/// <summary>
		/// The GUI contents.
		/// </summary>
		[System.NonSerialized]
		public Dictionary<string, List<GUIContent>> contents = new Dictionary<string, List<GUIContent>> ();
		/// <summary>
		/// The catalog items.
		/// </summary>
		public Dictionary<string, List<CatalogItem>> items = new Dictionary<string, List<CatalogItem>>();
		/// <summary>
		/// The catalog asset relative path.
		/// </summary>
		private static string catalogAssetRelativePath = "Catalog";
		/// <summary>
		/// Gets the catalog asset path.
		/// </summary>
		/// <value>The catalog asset path.</value>
		private static string catalogAssetPath { get { return ExtensionManager.extensionPath + catalogAssetRelativePath; } }
		#endregion

		#region Singleton
		/// <summary>
		/// The catalog singleton.
		/// </summary>
		static SproutCatalog _catalog = null;
		/// <summary>
		/// Gets a catalog singleton instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static SproutCatalog GetInstance () {
			if (_catalog == null) {
				_catalog = new SproutCatalog ();
				_catalog.LoadPackages ();
			}
			return _catalog;
		}
		#endregion

		#region Catalog Operations
		/// <summary>
		/// Loads the packages available for the catalog.
		/// </summary>
		public void LoadPackages () {
			Clear ();
			#if UNITY_EDITOR
			string[] catalogPath = {catalogAssetPath};
			string[] packagesPath = UnityEditor.AssetDatabase.FindAssets ("t:sproutCatalogPackage", catalogPath);
			for (int i = 0; i < packagesPath.Length; i++) {
				SproutCatalogPackage package = UnityEditor.AssetDatabase.LoadAssetAtPath<SproutCatalogPackage> (
					UnityEditor.AssetDatabase.GUIDToAssetPath (packagesPath [i]));
				if (package) {
					for (int j = 0; j < package.catalogItems.Count; j++) {
						AddCatalogItem (package.catalogItems[j]);
					}
				}
			}
			#endif
			PrepareGUIContents ();
		}
		/// <summary>
		/// Adds a category to the catalog.
		/// </summary>
		/// <param name="name">Name of the category.</param>
		/// <param name="order">Order.</param>
		void AddCatalogCategory (string name, int order = 0) {
		}
		/// <summary>
		/// Adds an item to the catalog.
		/// </summary>
		/// <param name="catalogItem">Catalog item.</param>
		void AddCatalogItem (CatalogItem catalogItem) {
			//if (catalogItem != null && !string.IsNullOrEmpty (catalogItem.path)) {
            if (catalogItem != null) {
				if (catalogItem.category == null) {
					catalogItem.category = "";
				}
				AddCatalogCategory (catalogItem.category);
				if (!items.ContainsKey (catalogItem.category)) {
					items [catalogItem.category] = new List<CatalogItem> ();
				}
				items [catalogItem.category].Add (catalogItem);
			}
		}
		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear () {
			_totalCategories = 0;
			_totalItems = 0;
			var enumerator = items.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				enumerator.Current.Value.Clear ();
			}
			items.Clear ();
			ClearContent ();
		}
		/// <summary>
		/// Clears the GUI content.
		/// </summary>
		private void ClearContent () {
			var enumerator = contents.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				enumerator.Current.Value.Clear ();
			}
			contents.Clear ();
		}
		/// <summary>
		/// Prepares the GUI contents.
		/// </summary>
		public void PrepareGUIContents () {
			ClearContent ();
			_totalItems = 0;
			_totalCategories = 0;
			var enumerator = items.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				var itemsPair = enumerator.Current;
				if (!contents.ContainsKey (itemsPair.Key)) {
					contents [itemsPair.Key] = new List<GUIContent> ();
					_totalCategories++;
				}
				for (int i = 0; i < itemsPair.Value.Count; i++) {
					if (itemsPair.Value[i].thumb != null) {
						contents [itemsPair.Key].Add (new GUIContent (itemsPair.Value[i].name, itemsPair.Value[i].thumb));
					} else {
						contents [itemsPair.Key].Add (new GUIContent (itemsPair.Value[i].name, GUITextureManager.GetLogoBox ()));
					}
					_totalItems++;
				}
			}
		}
		/// <summary>
		/// Gets the GUI contents.
		/// </summary>
		/// <returns>The GUI contents.</returns>
		public Dictionary<string, List<GUIContent>> GetGUIContents () {
			return contents;
		}
		/// <summary>
		/// Gets a catalog item at a given index.
		/// </summary>
		/// <returns>The item at index.</returns>
		/// <param name="categoryName">Category name.</param>
		/// <param name="index">Index.</param>
		public CatalogItem GetItemAtIndex (string categoryName, int index) {
			CatalogItem item = null;
			if (items.ContainsKey(categoryName) && index >= 0 && index < items[categoryName].Count) {
				item = items[categoryName][index];
			}
			return item;
		}
		/// <summary>
		/// Gets the first item at the catalog.
		/// </summary>
		/// <returns>The first catalog item in the catalog.</returns>
		public CatalogItem GetFirstItem () {
			var catalogEnum = items.GetEnumerator ();
			if (catalogEnum.MoveNext ()) {
				if (catalogEnum.Current.Value.Count > 0) {
					return catalogEnum.Current.Value [0];
				}
			}
			return null;
		}
		#endregion
	}
}