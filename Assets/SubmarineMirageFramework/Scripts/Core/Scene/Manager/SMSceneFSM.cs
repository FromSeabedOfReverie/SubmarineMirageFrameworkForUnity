//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using Task;
	using FSM;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneFSM : SMFSM<SMSceneManager, SMSceneFSM, SMScene> {
		[SMHide] public SMScene _scene => _state;



		public UniTask ChangeScene<T>() where T : SMScene
			=> ChangeState<T>();

		public UniTask ChangeScene( Type sceneType )
			=> ChangeState( sceneType );



		public IEnumerable<SMScene> GetScenes()
			=> GetStates();

		public SMScene GetScene( Type stateType )
			=> GetState( stateType );

		public T GetScene<T>() where T : SMScene
			=> GetState<T>();

		public SMScene GetScene( Scene rawScene )
			=> GetScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );



		public T GetBehaviour<T>() where T : SMBehaviour
			=> _scene?.GetBehaviour<T>();

		public SMBehaviour GetBehaviour( Type type )
			=> _scene?.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : SMBehaviour
			=> _scene?.GetBehaviours<T>() ?? Enumerable.Empty<T>();

		public IEnumerable<SMBehaviour> GetBehaviours( Type type )
			=> _scene?.GetBehaviours( type ) ?? Enumerable.Empty<SMBehaviour>();
	}
}