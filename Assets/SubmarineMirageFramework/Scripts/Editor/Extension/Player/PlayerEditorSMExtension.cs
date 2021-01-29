//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorExtension {
	using UnityEngine;
	using UnityEditor;
	using Task;
	using Utility;



	// TODO : コメント追加、整頓



	public class PlayerEditorSMExtension : ScriptableSingletonSMExtension<PlayerEditorSMExtension> {
		public enum Type {
			Stop,
			Runtime,
			Editor,
			Test,
		}

		// 何度再生されても、値を保持する
		// 他のstaticクラスだと、実行の度に作り直される為、値を保持できない
		// その為、ここで実行タイプを設定し、参照する
		Type _typeBody;
		public Type _type	{
			get => _typeBody;
			set {
				_typeBody = value;
				// SMLogだと有効設定前後のタイミングで呼ばれる為、駄目
				Debug.Log( $"{nameof( PlayerEditorSMExtension )}.{nameof( _type )} : {_typeBody}" );
			}
		}


		protected override void Awake() {
			base.Awake();

			EditorApplication.playModeStateChanged += mode => {
				switch ( mode ) {
					case PlayModeStateChange.EnteredPlayMode:
						if ( _type == Type.Stop )	{ _type = Type.Runtime; }
						return;
					case PlayModeStateChange.ExitingPlayMode:
						_type = Type.Stop;
						return;
				}
			};
		}

		public override void Dispose() {}



		public void PlayExtension() {
			if ( !EditorApplication.isPlaying ) {
				_type = Type.Editor;
				EditorApplication.isPlaying = true;

			} else {
				UTask.Void( async () => {
					SubmarineMirageFramework.Shutdown();
					using ( var canceler = new SMTaskCanceler() ) {
						await UTask.DelayFrame( canceler, 2 );
					}
					EditorApplication.isPlaying = false;
				} );
			}
		}

		public void PauseExtension() {
			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		public void StepExtension() {
			EditorApplication.Step();
		}
	}
}