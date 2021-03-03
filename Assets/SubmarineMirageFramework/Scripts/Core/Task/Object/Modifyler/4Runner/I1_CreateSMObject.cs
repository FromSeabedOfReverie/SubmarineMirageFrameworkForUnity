//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Modifyler.Base;
	using Debug;


	// TODO : コメント追加、整頓


	public class CreateSMObject : SMObjectModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public CreateSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.None )	{ return; }


			await RunLower( _runType, () => new CreateSMBehaviour() );

			if ( _runType == SMTaskRunAllType.DontRun )	{ _target._ranState = SMTaskRunState.Create; }
		}
	}
}