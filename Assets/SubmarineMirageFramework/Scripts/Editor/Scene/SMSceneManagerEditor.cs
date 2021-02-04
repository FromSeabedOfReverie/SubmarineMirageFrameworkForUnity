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
	using Task.Base;
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
					scene._groupManagerBody.GetAllGroups().ForEach( g => ShowGroup( g ) );
					EditorGUI.indentLevel--;
				} );
				EditorGUI.indentLevel--;
			} );
			EditorGUILayout.Space();
		}

		void ShowGroup( SMGroupBody groupBody ) {
			EditorGUI.indentLevel++;

			GUI.SetNextControlName( string.Join( "\n",
				$"{groupBody._id}, {groupBody._managerBody._scene.GetAboutName()}",
				$"↑ {groupBody._previous?.ToLineString()}",
				$"↓ {groupBody._next?.ToLineString()}",

				$"{( groupBody._asyncCanceler._isCancel ? "AsyncCancel, " : "" )}"
					+ $"{( groupBody._isDispose ? "Dispose" : "" )}"
			) );

			EditorGUILayout.SelectableLabel( groupBody.ToLineString(), GUILayout.Height( 16 ) );

			ShowObject( groupBody._objectBody );

			EditorGUI.indentLevel--;
		}

		void ShowObject( SMObjectBody objectBody ) {
			EditorGUI.indentLevel++;

			GUI.SetNextControlName( string.Join( "\n",
				$"{objectBody._id}",

				objectBody._gameObject.name,
				string.Join( "\n", objectBody._behaviourBody.GetBrothers().Select( b =>
					$"    {b.ToLineString()}"
				) ),

				$"← {objectBody._parent?.ToLineString()}",
				$"↑ {objectBody._previous?.ToLineString()}",
				$"↓ {objectBody._next?.ToLineString()}",
				$"→",
				string.Join( "\n", objectBody.GetChildren().Select( o => $"    {o.ToLineString()}" ) ),

				$"{( objectBody._asyncCanceler._isCancel ? "Cancel, " : "" )}"
					+ $"{( objectBody._isDispose ? "Dispose" : "" )}"
			) );

			EditorGUILayout.SelectableLabel( objectBody.ToLineString(), GUILayout.Height( 16 ) );

			objectBody.GetChildren().ForEach( c => ShowObject( c ) );

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