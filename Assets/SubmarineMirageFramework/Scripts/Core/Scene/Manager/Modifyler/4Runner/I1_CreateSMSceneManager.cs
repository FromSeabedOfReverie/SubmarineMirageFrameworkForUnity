//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage.Modifyler;
	using Service;
	using Task;
	using FSM;
	using Extension;
	using Utility;
	using Debug;


	public class CreateSMSceneManager : SMSceneManagerModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.None )	{ return; }

			Create();
			_target._ranState = SMTaskRunState.Create;

			await UTask.DontWait();
		}


		// SMScene内部で、SMServiceLocatorから自身を参照する為、Body生成後に、Sceneを遅延生成する
		void Create() {
			var setting = SMServiceLocator.Resolve<ISMSceneSetting>();
			_target._fsm = SMFSM.Generate( _target, setting._sceneFSMList );
			SMServiceLocator.Unregister<ISMSceneSetting>();

			_target._foreverFSM = _target._fsm.GetFSM<ForeverSMScene>();
			_target._mainFSM = _target._fsm.GetFSM<MainSMScene>();
			_target._uiFSM = _target._fsm.GetFSM<UISMScene>();
			_target._debugFSM = _target._fsm.GetFSM<DebugSMScene>();

			_target._foreverScene = _target._foreverFSM.GetStates()
				.Select( s => ( SMScene )s )
				.FirstOrDefault();

			_target._firstLoadedRawScenes = _target.GetLoadedRawScenes().ToList();

			_target._fsm.GetFSMs().ForEach( fsm => {
				fsm._body._startStateType = fsm.GetStates()
					.Select( s => ( SMScene )s )
					.FirstOrDefault( s => _target.IsFirstLoaded( s ) )
					?.GetType();
//				SMLog.Debug( fsm._body._startStateType?.GetAboutName() );
			} );
			// 不明なシーンを設定
			var scene = _target._mainFSM.GetState<UnknownSMScene>();
			scene.Setup();
		}
	}
}