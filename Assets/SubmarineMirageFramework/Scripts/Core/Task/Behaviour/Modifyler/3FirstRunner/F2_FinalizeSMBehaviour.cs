//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Cysharp.Threading.Tasks;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalizeSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public override async UniTask Run() {
			if ( _owner._owner._type == SMTaskType.DontWork )		{ return; }
			if ( _owner._ranState != SMTaskRunState.FinalDisable )	{ return; }

			_owner.StopAsyncOnDisable();	// 念の為、Disable未実行を考慮

			if ( _owner._isRunFinalize ) {
				await _owner._finalizeEvent.Run( _owner._asyncCancelerOnDispose );
			}
			_owner._ranState = SMTaskRunState.Finalize;
			_modifyler.UnregisterAll();
		}
	}
}