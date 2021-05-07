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


	public class SelfInitializeSMObject : SMObjectModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public SelfInitializeSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.Create )	{ return; }


			await RunLower( _runType, () => new SelfInitializeSMBehaviour() );

			if ( _runType == SMTaskRunAllType.Parallel )	{ _target._ranState = SMTaskRunState.SelfInitialize; }
		}
	}
}