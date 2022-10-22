//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Editor {
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using KoganeUnityLib;



	public class SMTaskManagerEditor : EditorWindowSMExtension {
		Vector2 _tasksScrollPosition	{ get; set; }
		Vector2 _detailScrollPosition	{ get; set; }
		string _focusedText	{ get; set; } = string.Empty;
		int _focusedID { get; set; } = int.MinValue;
		SMTaskManager _taskManager;


		public override void Dispose()	{}


		[MenuItem( "Window/SubmarineMirage/SMTaskManager" )]
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

			_tasksScrollPosition = EditorGUILayout.BeginScrollView( _tasksScrollPosition );
			ShowTasks();
			ShowModifyler();
			EditorGUILayout.EndScrollView();

			_detailScrollPosition = EditorGUILayout.BeginScrollView( _detailScrollPosition );
			ShowDetail();
			EditorGUILayout.EndScrollView();

			Repaint();
			if ( Event.current.type == EventType.Repaint ) {
				var last = _focusedID;
				_focusedID = GUI.GetNameOfFocusedControl().ToIntOrDefault( int.MinValue );
				if ( _focusedID != last )	{ _focusedText = string.Empty; }
			}
		}


		void ShowDispose() {
			ShowHeading2( nameof( SMServiceLocator.s_isDisposed ) );
		}


		void ShowTasks() {
			_taskManager.GetAlls().ForEach( task => {
				if ( task._type != task._previous?._type ) {
					ShowHeading2( task._type.ToString() );
					EditorGUI.indentLevel++;
				}

				var id = task.GetHashCode();
				if ( id == _focusedID ) {
					_focusedText = string.Join( "\n",
						$"{task._id}",
						$"↑ {task._previous?.ToLineString()}",
						$"↓ {task._next?.ToLineString()}",
						$"{( task._isDispose ? "Dispose" : "" )}"
					);
				}
				GUI.SetNextControlName( id.ToString() );
				EditorGUILayout.SelectableLabel( task.ToLineString(), GUILayout.Height( 16 ) );

				if ( task._type != task._next?._type ) {
					EditorGUI.indentLevel--;
					EditorGUILayout.Space();
				}
			} );
		}


		void ShowModifyler() {
			ShowLine();
			ShowHeading1( "Modifyler" );

			var datas = new Dictionary< string, LinkedList<SMModifyData> > {
				{ "RunData",	_taskManager._modifyler._runDatas },
				{ "Data",		_taskManager._modifyler._datas },
			};
			datas.ForEach( pair => {
				ShowHeading2( pair.Key );
				EditorGUI.indentLevel++;
				pair.Value.ForEach( d => {
					var id = d.GetHashCode();
					if ( id == _focusedID ) {
						_focusedText = d.ToString();
					}
					GUI.SetNextControlName( id.ToString() );
					EditorGUILayout.SelectableLabel( d.ToLineString(), GUILayout.Height( 16 ) );
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