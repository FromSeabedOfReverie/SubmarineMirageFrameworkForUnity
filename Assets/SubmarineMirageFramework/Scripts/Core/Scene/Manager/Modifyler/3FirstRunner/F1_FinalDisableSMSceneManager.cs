//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using Task;
	using Task.Modifyler.Base;
	using Scene.Base;
	using Scene.Modifyler.Base;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalDisableSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public override void Set( SMSceneManagerBody owner ) {
			base.Set( owner );
			_owner._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _owner._ranState >= SMTaskRunState.FinalDisable )	{ return; }
			if ( _owner._isFinalDisabling )	{ return; }
			_owner._isFinalDisabling = true;


			await FinalExit();


			var lastActiveState = _owner._activeState;
			_owner._activeState = SMTaskActiveState.Disable;
			_owner.StopAsyncOnDisable();

			if (	_owner.IsActiveInHierarchyAndComponent() &&
					lastActiveState != SMTaskActiveState.Disable
			) {
				_owner._disableEvent.Run();
			}

			_owner._ranState = SMTaskRunState.FinalDisable;
			_owner._isFinalDisabling = false;
		}


		async UniTask FinalExit() {
			await _owner._fsm.GetFSMs()
				.Where( fsm => fsm != _owner._foreverFSM )
				.Select( fsm => fsm.FinalExit( true ) );
			await _owner._foreverFSM.FinalExit( true );
		}
	}
}