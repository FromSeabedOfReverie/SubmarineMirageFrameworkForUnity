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



	public class SelfInitializeSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;
		SMTaskRunAllType _runType	{ get; set; }


		public SelfInitializeSMGroupManager( SMTaskRunAllType runType )
			=> _runType = runType;


		public override async UniTask Run() {
			if ( _owner._ranState != SMTaskRunState.Create )	{ return; }


			await RunLower( _runType, () => new SelfInitializeSMGroup( _runType ) );

			if ( _runType == SMTaskRunAllType.Parallel )	{ _owner._ranState = SMTaskRunState.SelfInitialize; }
		}
	}
}