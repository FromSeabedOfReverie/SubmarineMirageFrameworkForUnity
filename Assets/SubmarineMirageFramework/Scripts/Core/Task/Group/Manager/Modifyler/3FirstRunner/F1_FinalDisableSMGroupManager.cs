//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Manager.Modifyler {
	using Cysharp.Threading.Tasks;
	using Group.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class FinalDisableSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;
		SMTaskRunAllType _runType	{ get; set; }


		public FinalDisableSMGroupManager( SMTaskRunAllType runType )
			=> _runType = runType;

		public override void Set( SMGroupManager owner ) {
			base.Set( owner );
			_owner._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _owner._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			_owner._activeState = SMTaskActiveState.Disable;

			await RunLower( _runType, () => new FinalDisableSMGroup( _runType ) );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				_owner._ranState = SMTaskRunState.FinalDisable;
			}
		}
	}
}