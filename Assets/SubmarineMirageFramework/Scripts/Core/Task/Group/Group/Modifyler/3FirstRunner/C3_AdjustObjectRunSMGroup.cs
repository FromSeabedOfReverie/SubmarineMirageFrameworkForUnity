//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Modifyler {
	using Cysharp.Threading.Tasks;
	using Object;
	using Object.Modifyler;
	using Group.Manager.Modifyler;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class AdjustObjectRunSMGroup : SMGroupModifyData {
		
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public AdjustObjectRunSMGroup( SMObject target ) : base( target ) {}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }

			await RunEventToOwner();
		}



		async UniTask RunEventToOwner() {
// TODO : 動くように全体的に調整、実行待機させる、RunLower化


			if (	_owner._ranState >= SMTaskRunState.FinalDisable &&
					_target._ranState < SMTaskRunState.FinalDisable
			) {
				foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new FinalDisableSMObject( t ) );
				}
			}

			if (	_owner._ranState >= SMTaskRunState.Finalize &&
					_target._ranState < SMTaskRunState.Finalize
			) {
				foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new FinalizeSMObject( t ) );
				}
			}

			if ( _owner._isFinalizing )	{ return; }


			if (	_owner._ranState >= SMTaskRunState.Create &&
					_target._ranState < SMTaskRunState.Create
			) {
				// 非GameObjectの場合、生成直後だと、継承先コンストラクタ前に実行されてしまう為、1フレーム待機
				if ( !_target._isGameObject ) {
// TODO : 無くす
					await UTask.NextFrame( _target._asyncCanceler );
				}
				foreach ( var t in SMGroupManagerApplyer.ALL_RUN_TYPES ) {
					await RunLower( t, () => new CreateSMObject( t ) );
				}
			}

			if (	_owner._ranState >= SMTaskRunState.SelfInitialize &&
					_target._ranState < SMTaskRunState.SelfInitialize
			) {
				foreach ( var t in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new SelfInitializeSMObject( t ) );
				}
			}

			if (	_owner._ranState >= SMTaskRunState.Initialize &&
					_target._ranState < SMTaskRunState.Initialize
			) {
				foreach ( var t in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new InitializeSMObject( t ) );
				}
			}

			if (	_owner._ranState >= SMTaskRunState.InitialEnable &&
					_target._ranState < SMTaskRunState.InitialEnable
			) {
				foreach ( var t in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new InitialEnableSMObject( t ) );
				}
			}

			if ( !_owner._isInitialized )	{ return; }


			if ( SMObjectApplyer.IsActiveInHierarchy( _target ) ) {
				foreach ( var t in SMGroupManagerApplyer.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new EnableSMObject( t ) );
				}
				if ( _owner.IsTop( _target ) )	{ _owner._activeState = SMTaskActiveState.Enable; }

			} else {
				if ( _owner.IsTop( _target ) )	{ _owner._activeState = SMTaskActiveState.Disable; }
				foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new DisableSMObject( t ) );
				}
			}
		}
	}
}