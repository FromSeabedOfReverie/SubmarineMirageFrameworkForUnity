//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestScene
namespace SubmarineMirage.Scene {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Service;
	using Task;
	using Task.Behaviour;
	using Task.Group.Modifyler;
	using FSM;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneManager : SMBehaviour, ISMFSMOwner<SMSceneFSM>, ISMService {
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;

		public SMSceneFSM _fsm	{ get; private set; }



		public SMSceneManager() {
			var setting = SMServiceLocator.Resolve<BaseSMSceneSetting>();
			_fsm = new SMSceneFSM( this, setting._scenes );
			SMServiceLocator.Unregister<BaseSMSceneSetting>();

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

		public void MoveForeverScene( GameObject rawObject )
			=> SceneManager.MoveGameObjectToScene( rawObject, _fsm._foreverScene._rawScene );



		public T GetBehaviour<T>( SMSceneType? sceneType = null ) where T : ISMBehaviour
			=> _fsm.GetBehaviour<T>( sceneType );

		public ISMBehaviour GetBehaviour( Type type, SMSceneType? sceneType = null )
			=> _fsm.GetBehaviour( type, sceneType );

		public IEnumerable<T> GetBehaviours<T>( SMSceneType? sceneType = null ) where T : ISMBehaviour
			=> _fsm.GetBehaviours<T>( sceneType );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type, SMSceneType? sceneType = null )
			=> _fsm.GetBehaviours( type, sceneType );
	}
}