//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Task;
	using Debug;


	public class FinalDisableSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;


		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

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

			if ( lastActiveState != SMTaskActiveState.Disable ) {
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