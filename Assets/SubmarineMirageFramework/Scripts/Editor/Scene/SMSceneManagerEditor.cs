//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorScene {
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	using KoganeUnityLib;
	using Task.Object;
	using Task.Group;
	using Scene;
	using Extension;
	using Debug;
	using EditorExtension;



	// TODO : コメント追加、整頓



	[CustomEditor( typeof( SMSceneManager ) )]
	public class SMSceneManagerEditor : EditorSMExtension {
		SMSceneManager _instance	{ get; set; }
		Vector2 _scrollPosition	{ get; set; }
		string _focusedText	{ get; set; } = string.Empty;


		public override void Dispose()	{}


		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if ( target == null )	{ return; }
			_instance = (SMSceneManager)target;

			_scrollPosition = EditorGUILayout.BeginScrollView( _scrollPosition );
			ShowAllGroups();
			EditorGUILayout.EndScrollView();
			ShowDetail();

			Repaint();
			if ( Event.current.type == EventType.Repaint ) {
				_focusedText = GUI.GetNameOfFocusedControl();
			}
		}


		void ShowAllGroups() {
			_instance._fsm.GetFSMs().ForEach( fsm => {
				ShowHeading1( fsm._fsmType.ToString() );

				EditorGUI.indentLevel++;
				fsm.GetScenes().ForEach( scene => {
					ShowHeading2( scene._name );

					EditorGUI.indentLevel++;
					scene._groups.GetAllGroups().ForEach( g => ShowGroup( g ) );
					EditorGUI.indentLevel--;
				} );
				EditorGUI.indentLevel--;
			} );
			EditorGUILayout.Space();
		}

		void ShowGroup( SMGroup group ) {
			EditorGUI.indentLevel++;

			GUI.SetNextControlName( string.Join( "\n",
				$"{group._id}, {group._lifeSpan}( {group._scene.GetAboutName()} )",
				$"↑ {group._previous?.ToLineString()}",
				$"↓ {group._next?.ToLineString()}",

				$"{( group._asyncCanceler._isCancel ? "AsyncCancel, " : "" )}"
					+ $"{( group._isDispose ? "Dispose" : "" )}"
			) );

			EditorGUILayout.SelectableLabel( group.ToLineString(), GUILayout.Height( 16 ) );

			ShowObject( group._topObject );

			EditorGUI.indentLevel--;
		}

		void ShowObject( SMObject smObject ) {
			EditorGUI.indentLevel++;

			GUI.SetNextControlName( string.Join( "\n",
				$"{smObject._id}",

				$"{( smObject._gameObject != null ? smObject._gameObject.name : "null" )}",
				string.Join( "\n", smObject.GetBehaviours().Select( b => $"    {b.ToLineString()}" ) ),

				$"← {smObject._parent?.ToLineString()}",
				$"↑ {smObject._previous?.ToLineString()}",
				$"↓ {smObject._next?.ToLineString()}",
				$"→",
				string.Join( "\n", smObject.GetChildren().Select( o => $"    {o.ToLineString()}" ) ),

				$"{( smObject._asyncCanceler._isCancel ? "AsyncCancel, " : "" )}"
					+ $"{( smObject._isDispose ? "Dispose" : "" )}"
			) );

			EditorGUILayout.SelectableLabel( smObject.ToLineString(), GUILayout.Height( 16 ) );

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