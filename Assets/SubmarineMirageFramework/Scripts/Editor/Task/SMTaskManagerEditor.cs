//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorTask {
	using UnityEngine;
	using UnityEditor;
	using KoganeUnityLib;
	using Task;
	using Service;
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
			ShowTasks();
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


		void ShowTasks() {
			ShowHeading1( nameof( SMTask ) );

			SMTaskManager.CREATE_TASK_TYPES.ForEach( type => {
				ShowHeading2( type.ToString() );
				EditorGUI.indentLevel++;

				_taskManager.GetAlls( type ).ForEach( task => {
					GUI.SetNextControlName( string.Join( "\n",
						$"{task._id}",
						$"↑ {task._previous?.ToLineString()}",
						$"↓ {task._next?.ToLineString()}",
						$"{( task._isDispose ? "Dispose" : "" )}"
					) );
					EditorGUILayout.SelectableLabel( task.ToLineString(), GUILayout.Height( 16 ) );
				} );

				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			} );
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