//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor {
	using System;
	using UnityEditor;
	using UniRx.Async;
	using Process.New;


	// TODO : コメント追加、整頓


	public class PlayerExtensionEditor : EditorWindow {
		public static Action _playStopEvent;
		static bool _isStartEditorPlay;

		[MenuItem( "Edit/PlayExtension _F5", priority = 100000 )]
		static void PlayExtension() {
			if ( EditorApplication.isPlaying ) {
				UniTask.Void( async () => {
					ProcessRunner.DisposeInstance();
					_playStopEvent?.Invoke();
					_playStopEvent = null;
					if ( _isStartEditorPlay ) {
						await UniTask.Yield();
					}
					_isStartEditorPlay = false;
					EditorApplication.isPlaying = false;
				} );
			} else {
				_isStartEditorPlay = true;
				EditorApplication.isPlaying = true;
			}
		}

		[MenuItem( "Edit/PauseExtension _F6", priority = 100001 )]
		static void PauseExtension() {
			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		[MenuItem( "Edit/StepExtension _F7", priority = 100002 )]
		static void StepExtension() {
			EditorApplication.Step();
		}
	}
}