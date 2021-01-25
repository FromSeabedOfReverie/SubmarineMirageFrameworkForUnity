//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorExtension {
	using UnityEditor;
	using Task;
	using Utility;



	// TODO : コメント追加、整頓



	[InitializeOnLoad]
	public class PlayerEditorSMExtension : EditorWindowSMExtension {
		public static PlayerEditorSMExtension s_instance
			=> ScriptableSingleton<PlayerEditorSMExtension>.instance;


		protected override void Awake() {
			base.Awake();

			EditorApplication.playModeStateChanged += mode => {
				switch ( mode ) {
					case PlayModeStateChange.EnteredPlayMode:
						if ( SubmarineMirageFramework.s_playType == SubmarineMirageFramework.PlayType.Stop ) {
							SubmarineMirageFramework.s_playType = SubmarineMirageFramework.PlayType.Runtime;
						}
						return;
					case PlayModeStateChange.ExitingPlayMode:
						SubmarineMirageFramework.s_playType = SubmarineMirageFramework.PlayType.Stop;
						return;
				}
			};
		}

		public override void Dispose()	{}


		[MenuItem( "Edit/PlayExtension _F5", priority = 100000 )]
		static void PlayExtension() {
			if ( !EditorApplication.isPlaying ) {
				SubmarineMirageFramework.s_playType = SubmarineMirageFramework.PlayType.Editor;
				EditorApplication.isPlaying = true;

			} else {
				UTask.Void( async () => {
					var canceler = new SMTaskCanceler();
					SubmarineMirageFramework.Shutdown();
					if ( SubmarineMirageFramework.s_playType != SubmarineMirageFramework.PlayType.Test ) {
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