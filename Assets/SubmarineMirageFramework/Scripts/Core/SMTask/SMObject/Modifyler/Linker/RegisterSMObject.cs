//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;



	// TODO : コメント追加、整頓



	public class RegisterSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;


		public RegisterSMObject() : base( null )	{}


		public override void Cancel() {
			_group.Dispose();
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Cancel )} : {this}" );
#endif
		}


		public override async UniTask Run() {
			 // コンストラクタで設定出来ない為、ここで設定
			_object = _group._topObject;

			if ( _group._lifeSpan == SMTaskLifeSpan.Forever && _object._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _object._owner, _group._scene._scene );
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{SMTaskLifeSpan.Forever}シーン移動 :",
					$"{_object?.ToLineString()}",
					$"{_group?._scene}"
				) );
			} else {
				Log.Debug( string.Join( "\n",
					$"シーン移動なし :",
					$"{_object?.ToLineString()}",
					$"{_group?._scene}"
				) );
#endif
			}
			_group._objects.Register( _group );

			await SetRunObject();
		}


		async UniTask SetRunObject() {
			switch ( _group._type ) {
				case SMTaskType.DontWork:
					// Mono未使用の場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
					if ( _object._owner == null ) {
#if TestSMTaskModifyler
						Log.Debug( $"待機開始 : {this}" );
#endif
						await UTask.NextFrame( _object._asyncCanceler );
#if TestSMTaskModifyler
						Log.Debug( $"待機終了 : {this}" );
#endif
					}
					RunStateSMObject.RunOrRegister( _object, SMTaskRunState.Create );
					break;

				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _group._objects._isEnter ) {
						// Mono未使用の場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
						if ( _object._owner == null ) {
#if TestSMTaskModifyler
							Log.Debug( $"待機開始 : {this}" );
#endif
							await UTask.NextFrame( _object._asyncCanceler );
#if TestSMTaskModifyler
							Log.Debug( $"待機終了 : {this}" );
#endif
						}
						RunStateSMObject.RunOrRegister( _object, SMTaskRunState.Create );
						RunStateSMObject.RunOrRegister( _object, SMTaskRunState.SelfInitializing );
						RunStateSMObject.RunOrRegister( _object, SMTaskRunState.Initializing );
						_owner.Register( new RunActiveSMObject( _object ) );
					}
					break;
			}
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( SetRunObject )} :",
				$"{nameof( _group._objects._isEnter )} : {_group._objects._isEnter}",
				$"{nameof( _owner )} : {_owner}"
			) );
#endif
		}
	}
}