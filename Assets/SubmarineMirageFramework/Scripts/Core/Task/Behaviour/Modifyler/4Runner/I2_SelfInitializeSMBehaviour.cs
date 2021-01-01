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


	public class SelfInitializeSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public override async UniTask Run() {
			if ( _owner._ranState != SMTaskRunState.Create )	{ return; }

			await _owner._selfInitializeEvent.Run( _owner._asyncCancelerOnDispose );
			_owner._ranState = SMTaskRunState.SelfInitialize;
		}
	}
}