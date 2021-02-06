//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task.Base;
	using Task.Modifyler.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class DestroyObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;
		[SMShowLine] bool _isLastFinalizing	{ get; set; }


		public DestroyObjectSMGroup( SMObjectBody target ) : base( target ) {}


		public override void Set( SMGroupBody owner ) {
			base.Set( owner );

			_isLastFinalizing = _owner._isFinalizing;
			if ( _owner.IsTop( _target ) )	{ _owner._isFinalizing = true; }
		}


		public override async UniTask Run() {
			if ( _isLastFinalizing )	{ return; }

			// 設定時から割込みが入り、トップになる場合がある
			if ( _owner.IsTop( _target ) ) {
				await DestroyGroup();
			} else {
				await DestroyObject();
			}
		}


		async UniTask DestroyGroup() {
			_owner._isFinalizing = true;
			_owner._activeState = SMTaskActiveState.Disable;
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalDisableSMObject( t ) );
			}
			_owner._ranState = SMTaskRunState.FinalDisable;
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalizeSMObject( t ) );
			}
			_owner._ranState = SMTaskRunState.Finalize;
			_owner.DisposeAllObjects();
			_owner._gameObject.Destroy();

			_owner._managerBody._modifyler.Register( new UnregisterGroupSMGroupManager( _owner ) );
		}


		async UniTask DestroyObject() {
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalDisableSMObject( t ) );
			}
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalizeSMObject( t ) );
			}
			_target.DisposeAllChildren();
			_target._gameObject.Destroy();

			GetAllLowers().ForEach( o => _modifyler.Unregister( o ) );
			_target.Unlink();
		}
	}
}