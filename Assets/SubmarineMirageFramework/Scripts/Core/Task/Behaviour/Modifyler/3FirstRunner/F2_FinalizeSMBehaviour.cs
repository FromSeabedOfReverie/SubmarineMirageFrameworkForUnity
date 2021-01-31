//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalizeSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public override async UniTask Run() {
			if ( _owner._ranState != SMTaskRunState.FinalDisable )	{ return; }


			if ( _owner._isRunFinalize ) {
				await _owner._finalizeEvent.Run( _owner._asyncCancelerOnDispose );
			}

			_owner._ranState = SMTaskRunState.Finalize;
			_owner._behaviour.Dispose();
		}
	}
}