//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class AdjustObjectRunSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public AdjustObjectRunSMGroup( SMObjectBody target ) : base( target ) {
			if ( !_target._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}未所持の為、親合致不可 :\n{_target}" );
			}
		}


		public override async UniTask Run() {
			// 全体が「終了中」の場合
			if ( _owner._isFinalizing )	{
				await RunFinalizeToOwner();
				return;
			}

			await RunInitializeToOwner();

			// 全体が「初期化済」でない場合、未処理
			if ( !_owner._isInitialized )	{ return; }

			await RunActiveToOwner();
		}


		async UniTask RunFinalizeToOwner() {
			// 全体が「終了無効」済で、未実行の場合、実行
			if (	_owner._ranState >= SMTaskRunState.FinalDisable &&
					_target._ranState < SMTaskRunState.FinalDisable
			) {
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new FinalDisableSMObject( t ) );
				}
			}
			// 全体が「終了」済で、未実行の場合、実行
			if (	_owner._ranState >= SMTaskRunState.Finalize &&
					_target._ranState < SMTaskRunState.Finalize
			) {
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new FinalizeSMObject( t ) );
				}
				_target.DisposeAllChildren();
			}
		}


		async UniTask RunInitializeToOwner() {
			// 全体が「作成」済で、未実行の場合、実行
			if (	_owner._ranState >= SMTaskRunState.Create &&
					_target._ranState < SMTaskRunState.Create
			) {
				foreach ( var t in SMGroupManagerBody.ALL_RUN_TYPES ) {
					await RunLower( t, () => new CreateSMObject( t ) );
				}
			}
			// 全体が「自身初期化」済で、未実行の場合、実行
			if (	_owner._ranState >= SMTaskRunState.SelfInitialize &&
					_target._ranState < SMTaskRunState.SelfInitialize
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new SelfInitializeSMObject( t ) );
				}
			}
			// 全体が「初期化」済で、未実行の場合、実行
			if (	_owner._ranState >= SMTaskRunState.Initialize &&
					_target._ranState < SMTaskRunState.Initialize
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new InitializeSMObject( t ) );
				}
			}
			// 全体が「初期有効」済で、未実行の場合、実行
			if (	_owner._ranState >= SMTaskRunState.InitialEnable &&
					_target._ranState < SMTaskRunState.InitialEnable
			) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new InitialEnableSMObject( t ) );
				}
			}
		}


		async UniTask RunActiveToOwner() {
			// 全階層で「有効」の場合、実行
			if ( _target.IsActiveInHierarchy() ) {
				foreach ( var t in SMGroupManagerBody.SEQUENTIAL_RUN_TYPES ) {
					await RunLower( t, () => new EnableSMObject( t ) );
				}
				if ( _owner.IsTop( _target ) )	{ _owner._activeState = SMTaskActiveState.Enable; }

			// 全階層で「無効」の場合、実行
			} else {
				if ( _owner.IsTop( _target ) )	{ _owner._activeState = SMTaskActiveState.Disable; }
				foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
					// 親に合わせる為、階層が無効でも実行するフラグを設定
					// 親変更後だと、親は無効、新子は有効の場合がある
					await RunLower( t, () => new DisableSMObject( t, true ) );
				}
			}
		}
	}
}