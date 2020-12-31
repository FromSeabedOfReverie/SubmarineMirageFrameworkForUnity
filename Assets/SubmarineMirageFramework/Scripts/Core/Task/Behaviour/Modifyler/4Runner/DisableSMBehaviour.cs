//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class DisableSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public override async UniTask Run() {
			if ( !_owner._isOperable )	{ return; }
			if ( _owner._activeState == SMTaskActiveState.Disable )	{ return; }

			_owner._activeState = SMTaskActiveState.Disable;
			_owner.StopAsyncOnDisable();
			_owner._disableEvent.Run();

			await UTask.DontWait();
		}
	}
}