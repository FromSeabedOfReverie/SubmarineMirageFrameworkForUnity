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
	using Service;
	using Task;
	using Task.Modifyler.Base;
	using Scene.Base;
	using Scene.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


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
			_target._fsm = SMSceneFSM.Generate( _target._sceneManager, setting._sceneFSMList );
			SMServiceLocator.Unregister<ISMSceneSetting>();

			_target._foreverFSM = _target._fsm.GetFSM<ForeverSMScene>();
			_target._mainFSM = _target._fsm.GetFSM<MainSMScene>();
			_target._uiFSM = _target._fsm.GetFSM<UISMScene>();
			_target._debugFSM = _target._fsm.GetFSM<DebugSMScene>();
			_target._foreverScene = _target._foreverFSM.GetScenes().FirstOrDefault();

			_target._firstLoadedRawScenes = _target.GetLoadedRawScenes().ToList();

			_target._fsm.GetFSMs().ForEach( fsm => {
				fsm._body._startStateType = fsm.GetScenes()
					.FirstOrDefault( s => _target.IsFirstLoaded( s ) )
					?.GetType();
//				SMLog.Debug( fsm._body._startStateType?.GetAboutName() );
			} );
			// 不明なシーンを設定
			var scene = _target._mainFSM.GetScene<UnknownSMScene>();
			scene.Setup();
		}
	}
}