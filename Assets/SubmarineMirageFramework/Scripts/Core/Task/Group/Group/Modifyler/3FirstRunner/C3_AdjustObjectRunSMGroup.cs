//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Debug;



	public class AdjustObjectRunSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public AdjustObjectRunSMGroup( SMObjectBody smObject ) : base( smObject ) {}


		public override async UniTask Run() {
			// 全体が「終了中」の場合
			if ( _target._isFinalizing )	{
				await RunFinalizeToOwner();
				return;
			}

			await RunInitializeToOwner();

			// 全体が「初期化済」でない場合、未処理
			if ( !_target._isInitialized )	{ return; }

			await RunActiveToOwner();
		}


		async UniTask RunFinalizeToOwner() {
			// 全体が「終了無効」済で、未実行の場合、実行
			if (	_target._ranState >= SMTaskRunState.FinalDisable &&
					_object._ranState < SMTaskRunState.FinalDisable
			) {
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new FinalDisableSMObject( t ) );
				}
			}
			// 全体が「終了」済で、未実行の場合、実行
			if (	_target._ranState >= SMTaskRunState.Finalize &&
					_object._ranState < SMTaskRunState.Finalize
			) {
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new FinalizeSMObject( t ) );
				}
				_object.DisposeAllChildren();
			}
		}


		async UniTask RunInitializeToOwner() {
			// 全体が「作成」済で、未実行の場合、実行
			if (	_target._ranState >= SMTaskRunState.Create &&
					_object._ranState < SMTaskRunState.Create
			) {
				foreach ( var t in SMGroupManagerBody.ALL_RUN_TYPES ) {
					await RunLower( t, () => new CreateSMObject( t ) );
				}
			}
			// 全体が「自身初期化」済で、未実行の場合、実行
			if (	_target._ranState >= SMTaskRunState.SelfInitialize &&
					_object._ranState < SMTaskRunState.SelfInitialize
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new SelfInitializeSMObject( t ) );
				}
			}
			// 全体が「初期化」済で、未実行の場合、実行
			if (	_target._ranState >= SMTaskRunState.Initialize &&
					_object._ranState < SMTaskRunState.Initialize
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new InitializeSMObject( t ) );
				}
			}
			// 全体が「初期有効」済で、未実行の場合、実行
			if (	_target._ranState >= SMTaskRunState.InitialEnable &&
					_object._ranState < SMTaskRunState.InitialEnable
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new InitialEnableSMObject( t ) );
				}
			}
		}


		async UniTask RunActiveToOwner() {
			// 全階層で「有効」の場合、実行
			if ( _object.IsActiveInHierarchy() ) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new EnableSMObject( t ) );
				}
				if ( _target.IsTop( _object ) )	{ _target._activeState = SMTaskActiveState.Enable; }

			// 全階層で「無効」の場合、実行
			} else {
				if ( _target.IsTop( _object ) )	{ _target._activeState = SMTaskActiveState.Disable; }
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					// 親に合わせる為、階層が無効でも実行するフラグを設定
					// 親変更後だと、親は無効、新子は有効の場合がある
					await RunLower( t, () => new DisableSMObject( t, true ) );
				}
			}
		}
	}
}