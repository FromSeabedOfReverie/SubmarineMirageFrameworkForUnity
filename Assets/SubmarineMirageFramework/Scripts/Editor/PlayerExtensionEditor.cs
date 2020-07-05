//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Editor {
	using UnityEditor;
	using Cysharp.Threading.Tasks;
	using Main.New;


	// TODO : コメント追加、整頓


	public class PlayerExtensionEditorManager : ScriptableSingleton<PlayerExtensionEditorManager> {
		public PlayerExtensionEditor.PlayType _playType;
		PlayerExtensionEditor _windowObject;

		public PlayerExtensionEditor _window {
			get {
				if ( _windowObject == null )	{ _windowObject = CreateInstance<PlayerExtensionEditor>(); }
				return _windowObject;
			}
		}
	}


	[InitializeOnLoad]
	public class PlayerExtensionEditor : EditorWindow {
		public enum PlayType {
			Stop,
			Runtime,
			Editor,
			Test,
		}


		static PlayerExtensionEditor() {
			EditorApplication.playModeStateChanged += mode => {
				switch ( mode ) {
					case PlayModeStateChange.EnteredPlayMode:
						if ( PlayerExtensionEditorManager.instance._playType == PlayType.Stop ) {
							PlayerExtensionEditorManager.instance._playType = PlayType.Runtime;
						}
						break;
					case PlayModeStateChange.ExitingPlayMode:
						PlayerExtensionEditorManager.instance._playType = PlayType.Stop;
						break;
				}
			};
		}


		[MenuItem( "Edit/PlayExtension _F5", priority = 100000 )]
		static void PlayExtension() {
			if ( !EditorApplication.isPlaying ) {
				SubmarineMirage.DisposeInstance();
				PlayerExtensionEditorManager.instance._playType = PlayType.Editor;
				EditorApplication.isPlaying = true;

			} else {
				UniTask.Void( async () => {
					SubmarineMirage.DisposeInstance();
					if ( PlayerExtensionEditorManager.instance._playType != PlayType.Test ) {
						await UniTask.Yield();
					}
					EditorApplication.isPlaying = false;
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