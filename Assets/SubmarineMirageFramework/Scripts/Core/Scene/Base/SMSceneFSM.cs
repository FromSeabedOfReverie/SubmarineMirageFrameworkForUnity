//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using KoganeUnityLib;
	using Task.Behaviour;
	using FSM;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneFSM : SMParallelFSM<SMSceneManager, SMSceneInternalFSM, SMSceneType> {
		public SMSceneInternalFSM _foreverFSM	{ get; private set; }
		public SMSceneInternalFSM _mainFSM		{ get; private set; }
		public SMScene _foreverScene	{ get; private set; }
		readonly List<Scene> _firstLoadedRawScenes = new List<Scene>();


		public SMSceneFSM( SMSceneManager owner,　Dictionary<SMSceneType, SMSceneInternalFSM> fsms )
			: base( owner, fsms )
		{
			_foreverFSM = GetFSM( SMSceneType.Forever );
			_mainFSM = GetFSM( SMSceneType.Main );
			_foreverScene = _foreverFSM.GetScenes().FirstOrDefault();

			_firstLoadedRawScenes = GetLoadedRawScenes().ToList();
		}


		public bool RemoveFirstLoaded( SMScene scene ) {
			var count = _firstLoadedRawScenes
				.RemoveAll( s => s.name == scene._name );
			return count > 0;
		}

		public bool IsFirstLoaded( SMScene scene )
			=> _firstLoadedRawScenes
				.Any( s => s.name == scene._name );


		public IEnumerable<Scene> GetLoadedRawScenes()
			=> Enumerable.Range( 0, SceneManager.sceneCount - 1 )
				.Select( i => SceneManager.GetSceneAt( i ) );



		public IEnumerable<SMScene> GetScenes()
			=> GetFSMs()
				.SelectMany( fsm => fsm.GetScenes() )
				.Distinct();

		public SMScene GetScene( Scene rawScene )
			=> GetScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );



		public T GetBehaviour<T>( SMSceneType? sceneType = null ) where T : ISMBehaviour
			=> GetBehaviours<T>( sceneType )
				.FirstOrDefault();

		public ISMBehaviour GetBehaviour( Type type, SMSceneType? sceneType = null )
			=> GetBehaviours( type, sceneType )
				.FirstOrDefault();

		public IEnumerable<T> GetBehaviours<T>( SMSceneType? sceneType = null ) where T : ISMBehaviour
			=> GetBehaviours( typeof( T ), sceneType )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type, SMSceneType? sceneType = null ) {
			if ( sceneType.HasValue ) {
				return GetFSM( sceneType.Value )?.GetBehaviours( type ) ?? Enumerable.Empty<ISMBehaviour>();
			}
			return GetFSMs()
				.SelectMany( fsm => fsm.GetBehaviours( type ) );
		}
	}
}