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



	public class CreateSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public CreateSMGroupManager() : base( null ) {}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }
			if ( _owner._ranState != SMTaskRunState.None )	{ return; }


			foreach ( var t in SMGroupManagerBody.ALL_RUN_TYPES ) {
				await RunLower( t, () => new CreateSMGroup( t ) );
			}
			_owner._ranState = SMTaskRunState.Create;
		}
	}
}