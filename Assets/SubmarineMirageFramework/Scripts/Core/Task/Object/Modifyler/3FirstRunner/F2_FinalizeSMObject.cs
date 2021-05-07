//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage.Modifyler;
	using Debug;


	public class FinalizeSMObject : SMObjectModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public FinalizeSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( _target._ranState != SMTaskRunState.FinalDisable )	{ return; }


			await RunLower( _runType, () => new FinalizeSMBehaviour() );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				GetAllLowers()
					.Select( t => ( SMBehaviourBody )t )
					.Where( b => b._type == SMTaskType.DontWork )
					.Reverse()
					.ForEach( b => b._behaviour.Dispose() );

				_target._ranState = SMTaskRunState.Finalize;
				_target._object.Dispose();
			}
		}
	}
}