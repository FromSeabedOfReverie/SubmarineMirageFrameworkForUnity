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



	public class InitializeSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;
		SMTaskRunAllType _runType	{ get; set; }


		public InitializeSMGroup( SMTaskRunAllType runType ) : base( null )
			=> _runType = runType;


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }
			if ( _owner._ranState != SMTaskRunState.SelfInitialize )	{ return; }


			await RunLower( _runType, () => new InitializeSMObject( _runType ) );

			if ( _runType == SMTaskRunAllType.Parallel )	{ _owner._ranState = SMTaskRunState.Initialize; }
		}
	}
}