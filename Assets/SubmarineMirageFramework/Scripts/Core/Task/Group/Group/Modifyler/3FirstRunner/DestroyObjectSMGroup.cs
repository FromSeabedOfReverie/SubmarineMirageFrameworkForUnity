//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage.Modifyler;
	using Extension;
	using Debug;



	public class DestroyObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;
		[SMShowLine] bool _isLastFinalizing	{ get; set; }


		public DestroyObjectSMGroup( SMObjectBody smObject ) : base( smObject ) {}


		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			_isLastFinalizing = _target._isFinalizing;
			if ( _target.IsTop( _object ) )	{ _target._isFinalizing = true; }
		}


		public override async UniTask Run() {
			if ( _isLastFinalizing )	{ return; }

			// 設定時から割込みが入り、トップになる場合がある
			if ( _target.IsTop( _object ) ) {
				await DestroyGroup();
			} else {
				await DestroyObject();
			}
		}


		async UniTask DestroyGroup() {
			_target._isFinalizing = true;
			_target._activeState = SMTaskActiveState.Disable;
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalDisableSMObject( t ) );
			}
			_target._ranState = SMTaskRunState.FinalDisable;
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalizeSMObject( t ) );
			}
			_target._ranState = SMTaskRunState.Finalize;
			_target.DisposeAllObjects();
			_target._gameObject.Destroy();

			_target._managerBody._modifyler.Register( new UnregisterGroupSMGroupManager( _target ) );
		}


		async UniTask DestroyObject() {
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalDisableSMObject( t ) );
			}
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalizeSMObject( t ) );
			}
			_object.DisposeAllChildren();
			_object._gameObject.Destroy();

			GetAllLowers()
				.Select( t => ( SMObjectBody )t )
				.ForEach( o => _target.UnregisterModifyler( o ) );
			_object.Unlink();
		}
	}
}