//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor.EditorExtension {
	using UnityEditor;
	using Main;
	using Task;
	using Utility;


	// TODO : コメント追加、整頓


	public class PlayerSMExtensionEditorManager : ScriptableSingleton<PlayerSMExtensionEditorManager> {
		public PlayerSMExtensionEditor.PlayType _playType;
		PlayerSMExtensionEditor _windowObject;

		public PlayerSMExtensionEditor _window {
			get {
				if ( _windowObject == null )	{ _windowObject = CreateInstance<PlayerSMExtensionEditor>(); }
				return _windowObject;
			}
		}
	}


	[InitializeOnLoad]
	public class PlayerSMExtensionEditor : EditorWindow {
		public enum PlayType {
			Stop,
			Runtime,
			Editor,
			Test,
		}


		static PlayerSMExtensionEditor() {
			EditorApplication.playModeStateChanged += mode => {
				switch ( mode ) {
					case PlayModeStateChange.EnteredPlayMode:
						if ( PlayerSMExtensionEditorManager.instance._playType == PlayType.Stop ) {
							PlayerSMExtensionEditorManager.instance._playType = PlayType.Runtime;
						}
						break;
					case PlayModeStateChange.ExitingPlayMode:
						PlayerSMExtensionEditorManager.instance._playType = PlayType.Stop;
						break;
				}
			};
		}


		[MenuItem( "Edit/PlayExtension _F5", priority = 100000 )]
		static void PlayExtension() {
			if ( !EditorApplication.isPlaying ) {
				SubmarineMirage.DisposeInstance();
				PlayerSMExtensionEditorManager.instance._playType = PlayType.Editor;
				EditorApplication.isPlaying = true;

			} else {
				UTask.Void( async () => {
					var canceler = new SMTaskCanceler();
					SubmarineMirage.DisposeInstance();
					if ( PlayerSMExtensionEditorManager.instance._playType != PlayType.Test ) {
						await UTask.NextFrame( canceler );
					}
					EditorApplication.isPlaying = false;
					canceler.Dispose();
				} );
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