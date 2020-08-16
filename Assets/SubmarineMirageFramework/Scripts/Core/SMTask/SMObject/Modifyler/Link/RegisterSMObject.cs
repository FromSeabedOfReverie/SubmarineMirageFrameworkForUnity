//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class RegisterSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.LinkChanger;


		public RegisterSMObject( SMObject smObject ) : base( smObject ) {
			if ( !_object._isTop ) {
				throw new NotSupportedException( $"最上階の{nameof( SMObject )}で無い為、登録不可 :\n{_object}" );
			}
		}

		public override void Cancel() {
			_object.Dispose();
		}


		public override async UniTask Run() {
			if ( _object._lifeSpan == SMTaskLifeSpan.Forever && _object._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _object._owner, _object._scene._scene );
//				Log.Debug( $"MoveGameObjectToScene : {_object._owner}, {_object._scene._scene}" );
			} else {
//				Log.Debug( "Dont MoveGameObjectToScene" );
			}
			RegisterObject();

			await SetRunObject();
		}


		async UniTask SetRunObject() {
			switch ( _object._type ) {
				case SMTaskType.DontWork:
					// 生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
					await UTask.NextFrame( _object._asyncCanceler );
					RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Creating );

					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _object._objects._isEnter ) {
						// 生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
						await UTask.NextFrame( _object._asyncCanceler );
						RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Creating );
						RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Loading );
						RunStateSMObject.RunOrRegister( _object, SMTaskRanState.Initializing );
						_object._top._modifyler.Register( new RunActiveSMObject( _object ) );
					}
					return;
			}
		}
	}
}