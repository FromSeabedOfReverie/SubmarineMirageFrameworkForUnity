//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorTask {
	using System.Linq;
	using UnityEngine;
	using UnityEditor;
	using KoganeUnityLib;
	using Task;
	using Service;
	using Extension;
	using Debug;
	using EditorExtension;



	public class SMTaskManagerEditor : EditorWindowSMExtension {
		Vector2 _scrollPosition	{ get; set; }
		string _focusedText	{ get; set; } = string.Empty;
		SMTaskManager _taskManager;


		public override void Dispose()	{}


		[MenuItem( "Window/SubmarineMirage/TaskManager" )]
		static void Open() => GetWindow<SMTaskManagerEditor>();


		void OnGUI() {
			var name = nameof( SMTaskManager );
			titleContent = new GUIContent( name );
			ShowHeading1( name );

			_taskManager = SMServiceLocator.Resolve<SMTaskManager>();
			if ( _taskManager == null ) {
				ShowDispose();
				return;
			}

			_scrollPosition = EditorGUILayout.BeginScrollView( _scrollPosition );
			ShowAllGroups();
			EditorGUILayout.EndScrollView();
			ShowDetail();

			Repaint();
			if ( Event.current.type == EventType.Repaint ) {
				_focusedText = GUI.GetNameOfFocusedControl();
			}
		}


		void ShowDispose() {
			ShowHeading2( nameof( SMServiceLocator.s_isDisposed ) );
		}


		void ShowAllGroups() {
			_taskManager._fsm.GetFSMs().ForEach( fsm => {
				ShowHeading1( fsm._fsmType.GetAboutName() );

				EditorGUI.indentLevel++;
				fsm.GetStates()
					.Select( s => ( SMScene )s )
					.ForEach( scene => {
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

				objectBody._gameObject != null ? objectBody._gameObject.name : "null",
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