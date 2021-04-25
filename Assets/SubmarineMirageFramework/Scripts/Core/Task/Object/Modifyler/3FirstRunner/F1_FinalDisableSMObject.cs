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


	// TODO : コメント追加、整頓


	public class FinalDisableSMObject : SMObjectModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public FinalDisableSMObject( SMTaskRunAllType runType ) : base( runType ) {}

		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			_target._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _target._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			_target._activeState = SMTaskActiveState.Disable;
			_target._isDisabling = true;

			var isActiveInHierarchy = _target.IsActiveInHierarchy();
			await RunLower( _runType, () => new FinalDisableSMBehaviour( isActiveInHierarchy ) );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				_target._isDisabling = false;
				_target._ranState = SMTaskRunState.FinalDisable;
			}
		}
	}
}