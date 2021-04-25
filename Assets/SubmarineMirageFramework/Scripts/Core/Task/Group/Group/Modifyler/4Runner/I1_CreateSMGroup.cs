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



	public class CreateSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		[SMShowLine] SMTaskRunAllType _runType	{ get; set; }


		public CreateSMGroup( SMTaskRunAllType runType ) : base( null )
			=> _runType = runType;


		public override async UniTask Run() {
			if ( _target._isFinalizing )					{ return; }
			if ( _target._ranState != SMTaskRunState.None )	{ return; }


			await RunLower( _runType, () => new CreateSMObject( _runType ) );

// TODO : DontRunは、呼ばれない為、別の更新方法を考える
			if ( _runType == SMTaskRunAllType.DontRun )	{ _target._ranState = SMTaskRunState.Create; }
		}
	}
}