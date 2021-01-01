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
		SMTaskRunAllType _runType	{ get; set; }


		public CreateSMGroupManager( SMTaskRunAllType runType )
			=> _runType = runType;


		public override async UniTask Run() {
			if ( _owner._ranState != SMTaskRunState.None )	{ return; }


			await RunLower( _runType, () => new CreateSMGroup( _runType ) );

			if ( _runType == SMTaskRunAllType.DontRun )	{ _owner._ranState = SMTaskRunState.Create; }
		}
	}
}