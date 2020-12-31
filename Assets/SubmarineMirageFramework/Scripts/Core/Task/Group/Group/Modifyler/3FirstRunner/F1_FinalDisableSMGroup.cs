//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Group.Modifyler {
	using Cysharp.Threading.Tasks;
	using Object.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class FinalDisableSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;
		SMTaskRunAllType _runType	{ get; set; }


		public FinalDisableSMGroup( SMTaskRunAllType runType ) : base( null )
			=> _runType = runType;

		public override void Set( SMGroup owner ) {
			base.Set( owner );
			_owner._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _owner._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			_owner._activeState = SMTaskActiveState.Disable;

			await RunLower( _runType, o => new FinalDisableSMObject( _runType ) );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				_owner._ranState = SMTaskRunState.FinalDisable;
			}
		}
	}
}