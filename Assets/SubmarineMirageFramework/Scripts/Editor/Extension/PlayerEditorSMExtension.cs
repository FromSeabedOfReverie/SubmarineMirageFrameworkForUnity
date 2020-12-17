//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorExtension {
	using UnityEditor;
	using Main;
	using Task;
	using Utility;



	// TODO : コメント追加、整頓



	[InitializeOnLoad]
	public class PlayerEditorSMExtension : EditorWindowSMExtension {
		public enum PlayType {
			Stop,
			Runtime,
			Editor,
			Test,
		}
		public static PlayerEditorSMExtension s_instance
			=> ScriptableSingleton<PlayerEditorSMExtension>.instance;
		public PlayType _playType	{ get; set; }


		protected override void Awake() {
			base.Awake();

			EditorApplication.playModeStateChanged += mode => {
				switch ( mode ) {
					case PlayModeStateChange.EnteredPlayMode:
						if ( s_instance._playType == PlayType.Stop ) {
							s_instance._playType = PlayType.Runtime;
						}
						return;
					case PlayModeStateChange.ExitingPlayMode:
						s_instance._playType = PlayType.Stop;
						return;
				}
			};
		}

		public override void Dispose()	{}


		[MenuItem( "Edit/PlayExtension _F5", priority = 100000 )]
		static void PlayExtension() {
			if ( !EditorApplication.isPlaying ) {
				SubmarineMirage.DisposeInstance();
				s_instance._playType = PlayType.Editor;
				EditorApplication.isPlaying = true;

			} else {
				UTask.Void( async () => {
					var canceler = new SMTaskCanceler();
					SubmarineMirage.DisposeInstance();
					if ( s_instance._playType != PlayType.Test ) {
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