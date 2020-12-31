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


	public class InitializeSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public override async UniTask Run() {
			if ( _owner._owner._type == SMTaskType.DontWork )			{ return; }
			if ( _owner._ranState != SMTaskRunState.SelfInitialize )	{ return; }

			await _owner._initializeEvent.Run( _owner._asyncCancelerOnDispose );
			_owner._ranState = SMTaskRunState.Initialize;
		}
	}
}