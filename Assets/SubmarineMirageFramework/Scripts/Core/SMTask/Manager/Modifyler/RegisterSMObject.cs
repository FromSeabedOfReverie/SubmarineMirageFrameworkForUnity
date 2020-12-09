//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
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
			_object = _group._topObject;	// コンストラクタで設定出来ない為、ここで設定

			if ( _group._isGameObject && _group._lifeSpan == SMTaskLifeSpan.Forever ) {
				UnitySceneManager.MoveGameObjectToScene( _group._gameObject, _group._scene._scene );
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{_group._lifeSpan}シーン移動 :",
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

			await SetRunEvent();
		}


		async UniTask SetRunEvent() {
			switch ( _group._type ) {
				case SMTaskType.DontWork:
					// 非GameObjectの場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
					if ( !_group._isGameObject ) {
#if TestSMTaskModifyler
						Log.Debug( $"待機開始 : {this}" );
#endif
						await UTask.NextFrame( _group._asyncCanceler );
#if TestSMTaskModifyler
						Log.Debug( $"待機終了 : {this}" );
#endif
					}
					RunStateSMObject.RegisterAndRun( _group, SMTaskRunState.Create );
					break;

				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _group._objects._isEnter ) {
						// 非GameObjectの場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
						if ( !_group._isGameObject ) {
#if TestSMTaskModifyler
							Log.Debug( $"待機開始 : {this}" );
#endif
							await UTask.NextFrame( _group._asyncCanceler );
#if TestSMTaskModifyler
							Log.Debug( $"待機終了 : {this}" );
#endif
						}
						RunStateSMObject.RegisterAndRun( _group, SMTaskRunState.Create );
						RunStateSMObject.RegisterAndRun( _group, SMTaskRunState.SelfInitializing );
						RunStateSMObject.RegisterAndRun( _group, SMTaskRunState.Initializing );
						_owner.Register( new RunInitialActiveSMObject( _object ) );
					}
					break;
			}
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( SetRunEvent )} :",
				$"{nameof( _group._objects._isEnter )} : {_group._objects._isEnter}",
				$"{nameof( _owner )} : {_owner}"
			) );
#endif
		}
	}
}