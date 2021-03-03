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
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public override void Set( SMSceneManagerBody owner ) {
			base.Set( owner );
			_target._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _target._ranState >= SMTaskRunState.FinalDisable )	{ return; }
			if ( _target._isFinalDisabling )	{ return; }
			_target._isFinalDisabling = true;


			await FinalExit();


			var lastActiveState = _target._activeState;
			_target._activeState = SMTaskActiveState.Disable;
			_target.StopAsyncOnDisable();

			if (	_target.IsActiveInHierarchyAndComponent() &&
					lastActiveState != SMTaskActiveState.Disable
			) {
				_target._disableEvent.Run();
			}

			_target._ranState = SMTaskRunState.FinalDisable;
			_target._isFinalDisabling = false;
		}


		async UniTask FinalExit() {
			await _target._fsm.GetFSMs()
				.Where( fsm => fsm != _target._foreverFSM )
				.Select( fsm => fsm.FinalExit( true ) );
			await _target._foreverFSM.FinalExit( true );
		}
	}
}