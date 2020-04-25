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


	[CustomEditor( typeof( ProcessRunner ) )]
	public class ProcessRunnerEditor : Editor {
		ProcessRunner _instance;
		Vector2 _scrollPosition;
		string _focusedText = string.Empty;


		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			_instance = (ProcessRunner)target;

			_scrollPosition = EditorGUILayout.BeginScrollView( _scrollPosition );
			ShowAllHierarchies();
			ShowDetail();
			EditorGUILayout.EndScrollView();

			Repaint();
			if ( Event.current.type == EventType.Repaint ) {
				_focusedText = GUI.GetNameOfFocusedControl();
			}
		}


		void ShowAllHierarchies() {
			ShowHeading1( "All Process Hierarchies" );

			EditorGUI.indentLevel++;
			
			var scenes = SceneManager.s_instance._fsm._states.Select( pair => pair.Value ).ToList();
			scenes.InsertFirst( SceneManager.s_instance._fsm._foreverScene );

			scenes.ForEach( scene => {
				ShowHeading2( scene._name );

				EditorGUI.indentLevel++;
				scene._hierarchies._hierarchies.ForEach( typePair => {
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
			var a = p._isActive ? "◯" : "×";
			var g = hierarchy._owner != null ? $"( {hierarchy._owner.name} )" : "";
			EditorGUILayout.SelectableLabel(
				$"{a} {p.GetAboutName()}{g}( {p._body._ranState} )",
				GUILayout.Height( 16 )
			);

			hierarchy._children
				.ForEach( child => ShowHierarchy( child ) );

			EditorGUI.indentLevel--;
		}

		void ShowDetail() {
			ShowHeading1( "Detail" );
			EditorGUI.indentLevel++;
			_focusedText.Split( "\n" )
				.ForEach( s => EditorGUILayout.LabelField( s ) );
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
}