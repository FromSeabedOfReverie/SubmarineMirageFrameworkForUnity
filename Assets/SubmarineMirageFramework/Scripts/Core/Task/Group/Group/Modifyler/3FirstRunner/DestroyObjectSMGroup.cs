//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Object;
	using Object.Modifyler;
	using Group.Manager.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class DestroyObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;
		[SMShowLine] bool _isLastFinalizing	{ get; set; }


		public DestroyObjectSMGroup( SMObject target ) : base( target ) {}


		public override void Set( SMGroup owner ) {
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
			foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalDisableSMObject( t ) );
			}
			_owner._ranState = SMTaskRunState.FinalDisable;
			foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalizeSMObject( t ) );
			}
			_owner._ranState = SMTaskRunState.Finalize;
			SMGroupApplyer.DisposeAll( _owner );
			if ( _target._isGameObject )	{ _target._gameObject.Destroy(); }

			_owner._groups._modifyler.Register( new UnregisterGroupSMGroupManager( _owner ) );
		}


		async UniTask DestroyObject() {
			foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalDisableSMObject( t ) );
			}
			foreach ( var t in SMGroupManagerApplyer.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalizeSMObject( t ) );
			}
			SMObjectBody.DisposeAll( _target );
			if ( _target._isGameObject )	{ _target._gameObject.Destroy(); }

			GetAllLowers().ForEach( o => _modifyler.Unregister( o ) );
			SMObjectBody.Unlink( _target );
			SMGroupApplyer.SetAllData( _owner );
		}
	}
}