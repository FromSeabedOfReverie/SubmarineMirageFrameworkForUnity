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



	public class FinalDisableSMGroupManager : SMGroupManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public FinalDisableSMGroupManager() : base( null ) {}

		public override void Set( SMGroupManagerBody owner ) {
			base.Set( owner );
			_target._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _target._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			_target._activeState = SMTaskActiveState.Disable;
			foreach ( var t in SMGroupManagerBody.REVERSE_SEQUENTIAL_RUN_TYPES ) {
				await RunLower( t, () => new FinalDisableSMGroup( t ) );
			}
			_target._ranState = SMTaskRunState.FinalDisable;
		}
	}
}