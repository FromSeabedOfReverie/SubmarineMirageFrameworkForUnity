//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Task;
	using Debug;


	public class InitialEnableSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.Initialize )	{ return; }


			if (	_target.IsActiveInHierarchyAndComponent() &&
					_target._activeState != SMTaskActiveState.Enable
			) {
				_target._enableEvent.Run();
				_target._activeState = SMTaskActiveState.Enable;
			}

			_target._ranState = SMTaskRunState.InitialEnable;


			await InitialEnter();
		}


		async UniTask InitialEnter() {
			await _target._foreverFSM.InitialEnter( true );
			await _target._fsm.InitialEnter();
		}
	}
}