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
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }
			if ( _owner._ranState != SMTaskRunState.None )	{ return; }

			Create();
			_owner._ranState = SMTaskRunState.Create;

			await UTask.DontWait();
		}


		// SMScene内部で、SMServiceLocatorから自身を参照する為、Body生成後に、Sceneを遅延生成する
		void Create() {
			var setting = SMServiceLocator.Resolve<ISMSceneSetting>();
			_owner._fsm = SMSceneFSM.Generate( _owner._sceneManager, setting._sceneFSMList );
			SMServiceLocator.Unregister<ISMSceneSetting>();

			_owner._foreverFSM = _owner._fsm.GetFSM<ForeverSMScene>();
			_owner._mainFSM = _owner._fsm.GetFSM<MainSMScene>();
			_owner._uiFSM = _owner._fsm.GetFSM<UISMScene>();
			_owner._debugFSM = _owner._fsm.GetFSM<DebugSMScene>();
			_owner._foreverScene = _owner._foreverFSM.GetScenes().FirstOrDefault();

			_owner._firstLoadedRawScenes = _owner.GetLoadedRawScenes().ToList();

			_owner._fsm.GetFSMs().ForEach( fsm => {
				fsm._body._startStateType = fsm.GetScenes()
					.FirstOrDefault( s => _owner.IsFirstLoaded( s ) )
					?.GetType();
//				SMLog.Debug( fsm._body._startStateType?.GetAboutName() );
			} );
			// 不明なシーンを設定
			var scene = _owner._mainFSM.GetScene<UnknownMainSMScene>();
			scene.Setup();
		}
	}
}