//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;



	// TODO : コメント追加、整頓



	public class RegisterSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;


		public RegisterSMObject( SMObject smObject ) : base( smObject ) {
			if ( !_object._isTop ) {
				throw new NotSupportedException( $"最上階の{nameof( SMObject )}で無い為、登録不可 :\n{_object}" );
			}
		}


		public override void Cancel() {
			_object.Dispose();
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Cancel )} : {this}" );
#endif
		}


		public override async UniTask Run() {
			if ( _object._lifeSpan == SMTaskLifeSpan.Forever && _object._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _object._owner, _object._scene._scene );
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{SMTaskLifeSpan.Forever}シーン移動 :",
					$"{_object?.ToLineString()}",
					$"{_object?._scene}"
				) );
			} else {
				Log.Debug( string.Join( "\n",
					$"シーン移動なし :",
					$"{_object?.ToLineString()}",
					$"{_object?._scene}"
				) );
#endif
			}
			RegisterObject();

			await SetRunObject();
		}


		async UniTask SetRunObject() {
			switch ( _object._type ) {
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
					RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Creating );
					break;

				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _object._objects._isEnter ) {
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
						RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Creating );
						RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Loading );
						RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Initializing );
						_object._top._modifyler.Register( new RunActiveSMObject( _object ) );
					}
					break;
			}
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( SetRunObject )} :",
				$"{nameof( _object._objects._isEnter )} : {_object._objects._isEnter}",
				$"{nameof( _object._top._modifyler )} : {_object._top._modifyler}"
			) );
#endif
		}
	}
}