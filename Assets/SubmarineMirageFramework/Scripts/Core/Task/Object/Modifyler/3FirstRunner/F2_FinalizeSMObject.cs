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
	using Task.Modifyler.Base;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalizeSMObject : SMObjectModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public FinalizeSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( _owner._ranState != SMTaskRunState.FinalDisable )	{ return; }


			await RunLower( _runType, () => new FinalizeSMBehaviour() );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				GetAllLowers()
					.Where( b => b._type == SMTaskType.DontWork )
					.Reverse()
					.ForEach( b => b._behaviour.Dispose() );

				_owner._ranState = SMTaskRunState.Finalize;
				_owner._object.Dispose();
			}
		}
	}
}