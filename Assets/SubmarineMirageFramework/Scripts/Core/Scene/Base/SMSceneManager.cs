//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestScene
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using KoganeUnityLib;
	using Task;
	using Task.Behaviour;
	using Task.Object;
	using Task.Group.Modifyler;
	using FSM;
	using Singleton;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneManager : SMSingleton<SMSceneManager>, ISMFSMOwner<SMSceneFSM> {
		public SMSceneFSM _fsm	{ get; private set; }



		public SMSceneManager() {
			_fsm = new SMSceneFSM( this );
// TODO : ここで、SetAllDataする意味が分からない・・・
			SMGroupApplyer.SetAllData( _object._group );

			_disposables.AddLast( () => {
				_fsm.Dispose();
			} );

#if TestScene
			SMLog.Debug( $"{nameof( SceneManager )}() : {this}" );
#endif
		}

		public override void Create() {
#if TestScene
			SMLog.Debug( $"{nameof( Create )} : {this}" );
#endif
		}



		public T GetBehaviour<T>( SMTaskType? taskType = null )
			where T : ISMBehaviour
			=> GetBehaviours<T>( taskType )
				.FirstOrDefault();

		public TBehaviour GetBehaviour<TBehaviour, TScene>( SMTaskType? taskType = null )
			where TBehaviour : ISMBehaviour
			where TScene : SMScene
			=> GetBehaviours<TBehaviour, TScene>( taskType )
				.FirstOrDefault();

		public ISMBehaviour GetBehaviour( Type type, Type sceneType = null, SMTaskType? taskType = null )
			=> GetBehaviours( type, sceneType, taskType )
				.FirstOrDefault();

		public IEnumerable<T> GetBehaviours<T>( SMTaskType? taskType = null )
			where T : ISMBehaviour
			=> GetBehaviours( typeof( T ), null, taskType )
				.Select( b => (T)b );

		public IEnumerable<TBehaviour> GetBehaviours<TBehaviour, TScene>( SMTaskType? taskType = null )
			where TBehaviour : ISMBehaviour
			where TScene : SMScene
			=> GetBehaviours( typeof( TBehaviour ), typeof( TScene ), taskType )
				.Select( b => (TBehaviour)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type, Type sceneType = null,
														SMTaskType? taskType = null
		) {
			var scenes = Enumerable.Empty<SMScene>();
			if ( sceneType != null ) {
				var scene = _fsm.GetAllScenes().FirstOrDefault( s => s.GetType() == sceneType );
				if ( scene != null )	{ scenes = new [] { scene }; }
			} else {
				scenes = _fsm.GetAllScenes();
			}
			var currents = new Queue<SMObject>( scenes.SelectMany( s => {
				return s._groups.GetAllTops();
			} ) );
			while ( !currents.IsEmpty() ) {
				var o = currents.Dequeue();
				foreach ( var b in o.GetBehaviours( type ).Where( bb => bb._type == taskType ) ) {
					yield return b;
				}
				o.GetChildren().ForEach( c => currents.Enqueue( c ) );
			}
		}
	}
}