//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Modifyler.Base;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class DisableSMBehaviour : SMBehaviourModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public override async UniTask Run() {
			if ( !_target.IsActiveInComponent() )	{ return; }
			if ( _target._isFinalizing )	{ return; }
			if ( !_target._isInitialized ) {
				_target._isRunInitialActive = false;
				return;
			}
			if ( _target._activeState == SMTaskActiveState.Disable )	{ return; }

			_target._activeState = SMTaskActiveState.Disable;
			_target.StopAsyncOnDisable();
			_target._disableEvent.Run();

			await UTask.DontWait();
		}
	}
}