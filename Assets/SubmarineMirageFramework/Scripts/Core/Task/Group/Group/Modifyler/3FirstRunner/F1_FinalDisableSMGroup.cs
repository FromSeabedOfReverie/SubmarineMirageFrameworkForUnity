//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class FinalDisableSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;
		[SMShowLine] SMTaskRunAllType _runType	{ get; set; }


		public FinalDisableSMGroup( SMTaskRunAllType runType ) : base( null )
			=> _runType = runType;

		public override void Set( SMGroupBody owner ) {
			base.Set( owner );
			_owner._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _owner._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			_owner._activeState = SMTaskActiveState.Disable;

			await RunLower( _runType, () => new FinalDisableSMObject( _runType ) );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				_owner._ranState = SMTaskRunState.FinalDisable;
			}
		}
	}
}