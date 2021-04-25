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



	public class FinalizeSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;
		[SMShowLine] SMTaskRunAllType _runType	{ get; set; }


		public FinalizeSMGroup( SMTaskRunAllType runType ) : base( null )
			=> _runType = runType;


		public override async UniTask Run() {
			if ( _target._ranState != SMTaskRunState.FinalDisable )	{ return; }


			await RunLower( _runType, () => new FinalizeSMObject( _runType ) );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				_target._ranState = SMTaskRunState.Finalize;
				_target._group.Dispose();
			}
		}
	}
}