//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor.EditorSMTask {
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
	using SMTask;
	using Scene;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	[CustomEditor( typeof( SMTaskRunner ) )]
	public class SMTaskRunnerEditor : Editor {
		SMTaskRunner _instance;
		Vector2 _scrollPosition;
		string _focusedText = string.Empty;


		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if ( target == null )				{ return; }
			if ( !SceneManager.s_isCreated )	{ return; }

			_instance = (SMTaskRunner)target;

			_scrollPosition = EditorGUILayout.BeginScrollView( _scrollPosition );
			ShowAllObjects();
			EditorGUILayout.EndScrollView();
			ShowDetail();

			Repaint();
			if ( Event.current.type == EventType.Repaint ) {
				_focusedText = GUI.GetNameOfFocusedControl();
			}
		}


		void ShowAllObjects() {
			SceneManager.s_instance._fsm.GetAllScene().ForEach( scene => {
				ShowHeading1( scene._name );

				EditorGUI.indentLevel++;
				EditorGUILayout.LabelField( $"_isLock : {scene._objects._modifyler._isLock.Value}" );

				scene._objects._objects.ForEach( pair => {
					ShowHeading2( pair.Key.ToString() );
					scene._objects.Get( pair.Key ).ForEach( o => ShowObject( o ) );
				} );
				EditorGUI.indentLevel--;
			} );
			EditorGUILayout.Space();
		}

		void ShowObject( SMObject smObject ) {
			EditorGUI.indentLevel++;

			GUI.SetNextControlName( smObject.ToString() );
			var p = smObject._behavior;
			var a = p._isActive ? "◯" : "×";
			var g = smObject._owner != null ? $"( {smObject._owner.name} )" : "";
			EditorGUILayout.SelectableLabel(
				$"{a} {p.GetAboutName()}{g}( {p._body._ranState} )",
				GUILayout.Height( 16 )
			);

			smObject.GetChildren().ForEach( child => ShowObject( child ) );

			EditorGUI.indentLevel--;
		}

		void ShowDetail() {
			ShowLine();
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
			EditorGUILayout.LabelField( $"● {text}", EditorStyles.boldLabel );
		}
		void ShowHeading2( string text ) {
			EditorGUILayout.LabelField( $"・{text}", EditorStyles.boldLabel );
		}
	}
}