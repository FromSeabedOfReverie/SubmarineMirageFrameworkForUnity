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


	public class FinalizeSMBehaviour : SMBehaviourModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public override async UniTask Run() {
			if ( _target._ranState != SMTaskRunState.FinalDisable )	{ return; }


			if ( _target._isRunFinalize ) {
				await _target._finalizeEvent.Run( _target._asyncCancelerOnDispose );
			}

			_target._ranState = SMTaskRunState.Finalize;
			_target._behaviour.Dispose();
		}
	}
}