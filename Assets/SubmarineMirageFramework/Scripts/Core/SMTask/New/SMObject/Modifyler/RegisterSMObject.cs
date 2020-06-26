//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UniRx.Async;
	using Utility;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class RegisterSMObject : SMObjectModifyData {
		public RegisterSMObject( SMObject smObject ) : base( smObject ) {}


		public override async UniTask Run() {
			if ( _object._lifeSpan == SMTaskLifeSpan.Forever && _object._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _object._owner, _object._scene._scene );
//				Log.Debug( $"MoveGameObjectToScene : {_object._owner}, {_object._scene._scene}" );
			} else {
//				Log.Debug( "Dont MoveGameObjectToScene" );
			}

			_object._objects.Add( _object );

			await SetRunObject();
		}


		async UniTask SetRunObject() {
			switch ( _object._type ) {
				case SMTaskType.DontWork:
					// 生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
					await UniTaskUtility.Yield( _object._asyncCancel );
					_object._top._modifyler.Register(
						new RunStateSMObject( _object, SMTaskRanState.Creating ) );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _object._objects._isEnter ) {
						// 生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
						await UniTaskUtility.Yield( _object._asyncCancel );
						_object._top._modifyler.Register(
							new RunStateSMObject( _object, SMTaskRanState.Creating ) );
						_object._top._modifyler.Register(
							new RunStateSMObject( _object, SMTaskRanState.Loading ) );
						_object._top._modifyler.Register(
							new RunStateSMObject( _object, SMTaskRanState.Initializing ) );
						_object._top._modifyler.Register(
							new RunActiveSMObject( _object ) );
					}
					return;
			}
		}
	}
}