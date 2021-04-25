//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class EnableSMBehaviour : SMBehaviourModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public override async UniTask Run() {
			if ( !_target.IsActiveInComponent() )	{ return; }
			if ( _target._isFinalizing )			{ return; }
			if ( !_target._isInitialized ) {
				_target._isRunInitialActive = _target._objectBody.IsActiveInHierarchy();
				return;
			}
			if ( _target._activeState == SMTaskActiveState.Enable )	{ return; }

			_target._enableEvent.Run();
			_target._activeState = SMTaskActiveState.Enable;

			await UTask.DontWait();
		}
	}
}