//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using KoganeUnityLib;
	using FSM;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneFSM : SMParallelFSM<SMSceneManager, SMSceneInternalFSM, SMSceneType> {
		public readonly List<Scene> _firstLoadedRawScenes = new List<Scene>();


		public SMSceneFSM( SMSceneManager owner ) : base(
			owner,
			new Dictionary<SMSceneType, SMSceneInternalFSM> {
				{
					SMSceneType.Forever,
					new SMSceneInternalFSM(
						new SMScene[] {
							new ForeverSMScene()
						},
						typeof( ForeverSMScene )
					)
				}, {
					SMSceneType.Main,
					new SMSceneInternalFSM(
						new SMScene[] {
							new NoneMainSMScene(),
						},
						typeof( MainSMScene )
					)
				}, {
					SMSceneType.UI,
					new SMSceneInternalFSM(
						new SMScene[] {
							new NoneUISMScene(),
						},
						typeof( UISMScene )
					)
				}, {
					SMSceneType.FieldChunk1,
					new SMSceneInternalFSM(
						new SMScene[] {
							new NoneFieldChunkSMScene(),
						},
						typeof( FieldChunkSMScene )
					)
				}, {
					SMSceneType.FieldChunk2,
					new SMSceneInternalFSM(
						new SMScene[] {
							new NoneFieldChunkSMScene(),
						},
						typeof( FieldChunkSMScene )
					)
				}, {
					SMSceneType.FieldChunk3,
					new SMSceneInternalFSM(
						new SMScene[] {
							new NoneFieldChunkSMScene(),
						},
						typeof( FieldChunkSMScene )
					)
				}, {
					SMSceneType.FieldChunk4,
					new SMSceneInternalFSM(
						new SMScene[] {
							new NoneFieldChunkSMScene(),
						},
						typeof( FieldChunkSMScene )
					)
				},
			}
		) {
			_firstLoadedRawScenes = GetLoadedRawScenes().ToList();
		}


		public IEnumerable<SMScene> GetAllScenes()
			=> _fsms.SelectMany( pair => pair.Value.GetAllScenes() );

		public SMScene GetScene( Scene rawScene )
			=> GetAllScenes()
				.FirstOrDefault( s => s._rawScene == rawScene )
				?? _fsms.First().Value.GetState( typeof( NoneMainSMScene ) );


		public IEnumerable<Scene> GetLoadedRawScenes()
			=> Enumerable.Range( 0, SceneManager.sceneCount - 1 )
				.Select( i => SceneManager.GetSceneAt( i ) );


		public bool RemoveFirstLoaded( SMScene scene ) {
			var count = _firstLoadedRawScenes
				.RemoveAll( s => s.name == scene._name );
			return count > 0;
		}

		public bool IsFirstLoaded( SMScene scene )
			=> _firstLoadedRawScenes
				.Any( s => s.name == scene._name );
	}
}