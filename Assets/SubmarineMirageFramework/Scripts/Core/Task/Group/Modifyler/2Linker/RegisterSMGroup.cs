//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class RegisterSMGroup : SMGroupModifyData {
		public RegisterSMGroup( SMGroup target ) : base( target )
			=> _type = SMTaskModifyType.Linker;


		protected override void Cancel() {
			_target.Dispose();
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( Cancel )} : {this}" );
#endif
		}


		public override async UniTask Run() {
			if ( _target._isGameObject && _target._lifeSpan == SMTaskLifeSpan.Forever ) {
				SceneManager.MoveGameObjectToScene( _target._gameObject, _owner._owner._scene );
#if TestGroupModifyler
				SMLog.Debug( string.Join( "\n",
					"シーン移動 :",
					_target._lifeSpan,
					_target.ToLineString(),
					_target._scene._scene
				) );
			} else {
				SMLog.Debug( string.Join( "\n",
					"シーン移動なし :",
					_target._lifeSpan,
					_target.ToLineString(),
					_target._scene._scene
				) );
#endif
			}
			SMGroupApplyer.Link( _owner, _target );

			await SetRunEvent();
		}


		async UniTask SetRunEvent() {
			switch ( _target._type ) {
				case SMTaskType.DontWork:
					// 非GameObjectの場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
					if ( !_target._isGameObject ) {
#if TestGroupModifyler
						SMLog.Debug( $"待機開始 : {this}" );
#endif
						await UTask.NextFrame( _target._asyncCanceler );
#if TestGroupModifyler
						SMLog.Debug( $"待機終了 : {this}" );
#endif
					}
					await _target.RunStateEvent( SMTaskRunState.Create, false );
					break;

				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _owner._isEnter ) {
						// 非GameObjectの場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
						if ( !_target._isGameObject ) {
#if TestGroupModifyler
							SMLog.Debug( $"待機開始 : {this}" );
#endif
							await UTask.NextFrame( _target._asyncCanceler );
#if TestGroupModifyler
							SMLog.Debug( $"待機終了 : {this}" );
#endif
						}
						await _target.RunStateEvent( SMTaskRunState.Create, false );
						await _target.RunStateEvent( SMTaskRunState.SelfInitialize, false );
						await _target.RunStateEvent( SMTaskRunState.Initialize, false );
						await _target.RunInitialActive( false );
					}
					break;
			}
#if TestGroupModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( SetRunEvent )} :",
				$"{nameof( _owner._isEnter )} : {_owner._isEnter}",
				$"{nameof( _modifyler )} : {_modifyler}"
			) );
#endif
		}
	}
}