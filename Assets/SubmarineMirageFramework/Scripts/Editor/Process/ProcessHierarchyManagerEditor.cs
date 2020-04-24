//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor.EditorProcess {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Singleton.New;
	using MultiEvent;
	using Process.New;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Type = Process.New.ProcessBody.Type;
	using RanState = Process.New.ProcessBody.RanState;


	// TODO : コメント追加、整頓


	[CustomEditor( typeof( ProcessHierarchyManager ) )]
	public class ProcessHierarchyManagerEditor : Editor {
		ProcessHierarchyManager instance;
		Vector2 scroll;

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			instance = (ProcessHierarchyManager)target;
			scroll = EditorGUILayout.BeginScrollView( scroll );

			ShowAllHierarchies();
			ShowDetail();

			EditorGUILayout.EndScrollView();
		}

		void ShowAllHierarchies() {
			ShowHeading1( "All Process Hierarchies" );

			EditorGUI.indentLevel++;
			instance._hierarchies.ForEach( scenePair => {
				ShowHeading2( scenePair.Key );

				EditorGUI.indentLevel++;
				scenePair.Value.ForEach( typePair => {
					ShowHeading3( typePair.Key.ToString() );
					typePair.Value.ForEach( h => ShowHierarchy( h ) );
				} );
				EditorGUI.indentLevel--;
			} );
			EditorGUI.indentLevel--;
		}

		void ShowHierarchy( ProcessHierarchy hierarchy ) {
			EditorGUI.indentLevel++;

			GUI.SetNextControlName( hierarchy.ToString() );
			var p = hierarchy._processes.First();
			var a = p._isActive ? "→" : "×";
			var g = hierarchy._owner != null ? $"( {hierarchy._owner.name} )" : "";
			EditorGUILayout.SelectableLabel(
				$"{a} {p.GetAboutName()}{g}( {p._body._ranState} )",
				GUILayout.Height( 16 )
			);

			hierarchy._children.ForEach( child => {
				ShowHierarchy( child );
			} );

			EditorGUI.indentLevel--;
		}

		void ShowDetail() {
			ShowHeading1( "Detail" );
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField(
				GUI.GetNameOfFocusedControl(),
				GUILayout.ExpandHeight( true )
			);
			EditorGUI.indentLevel--;
		}

		void ShowLine() {
			GUILayout.Box(
				"",
				GUILayout.Width( EditorGUIUtility.currentViewWidth - 30 ),
				GUILayout.Height( 1 )
			);
		}
		void ShowHeading1( string text ) {
			ShowLine();
			EditorGUILayout.LabelField( $"■ {text}", EditorStyles.boldLabel );
			ShowLine();
		}
		void ShowHeading2( string text ) {
			EditorGUILayout.LabelField( $"● {text}", EditorStyles.boldLabel );
		}
		void ShowHeading3( string text ) {
			EditorGUILayout.LabelField( $"・{text}", EditorStyles.boldLabel );
		}
	}



	public class Tree<T> {
		public T Data	{ get; set; }
		public IEnumerable< Tree<T> > Childs	{ get; set; }
		public bool Opened	{ get; set; }

		public Tree() {
			Childs = Enumerable.Empty< Tree<T> >();
			Opened = true;
		}
	}



	public static class GUIExtension {
		public static void ShowTree( Tree<string> tree ) {
			var text = tree.Data.ToString();
			GUI.SetNextControlName( text );
			if ( !tree.Childs.IsEmpty() ) {
				tree.Opened = EditorGUILayout.Foldout( tree.Opened, text );
			} else {
				EditorGUILayout.SelectableLabel( text, GUILayout.Height( 16 ) );
			}
			EditorGUI.indentLevel++;

			if ( tree.Opened ) {
				foreach ( var child in tree.Childs ) {
					ShowTree( child );
				}
			}
			EditorGUI.indentLevel--;
		}
	}



	public class TreeView : EditorWindow {
		Vector2 scroll;
		Tree<string> tree;

		void OnGUI() {
			if ( tree == null )	{ return; }

			scroll = EditorGUILayout.BeginScrollView( scroll );
			GUIExtension.ShowTree( tree );
			EditorGUILayout.LabelField( GUI.GetNameOfFocusedControl() );
			EditorGUILayout.EndScrollView();
		}

		void OnSelectionChange() {
			tree = GameObjectToTree( Selection.activeGameObject );
			Repaint();
		}

		Tree<string> GameObjectToTree( GameObject go ) {
			var childs = new List< Tree<string> >();

			foreach ( Transform child in go.transform ) {
				childs.Add( GameObjectToTree( child.gameObject ) );
			}

			return new Tree<string> {
				Data = go.name,
				Childs = childs
			};
		}

		[MenuItem( "Window/TreeView" )]
		static void ShowWindow() {
			GetWindow<TreeView>();
		}
	}
}