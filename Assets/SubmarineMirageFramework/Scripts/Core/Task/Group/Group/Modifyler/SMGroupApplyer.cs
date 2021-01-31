//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Service;
	using Object;
	using Object.Modifyler;
	using Manager.Modifyler;
	using Scene;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMGroupApplyer {
		public static void DisposeAll( SMGroup group ) {
			SMObjectBody.DisposeAll( group._topObject );
			group.Dispose();
		}



		public static void SetTop( SMGroup group, SMObject smObject ) {
			group._topObject = smObject.GetTop();
			SetAllData( group );
		}



		public static void SetAllData( SMGroup group ) {
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( SetAllData )} : start\n{group}" );
#endif
			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();

			var lastScene = group._scene;
			var allObjects = group._topObject.GetAllChildren();
			var allBehaviours = allObjects.SelectMany( o => o.GetBehaviours() );
#if TestGroupModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( allObjects )} :",
				$"{string.Join( "\n", allObjects.Select( o => o?.ToLineString() ) )}"
			) );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( allBehaviours )} :",
				$"{string.Join( "\n", allBehaviours.Select( b => b?.ToLineString() ) )}"
			) );
#endif
			group._lifeSpan = allBehaviours.Any( b => b._lifeSpan == SMTaskLifeSpan.Forever ) ?
				SMTaskLifeSpan.Forever : SMTaskLifeSpan.InScene;

			// SceneManager作成時の場合、循環参照になる為、設定出来ない
			if ( sceneManager == null ) {
				group._scene = null;
			} else if ( group._lifeSpan == SMTaskLifeSpan.Forever ) {
				group._scene = sceneManager._fsm._foreverScene;
			} else if ( group._isGameObject ) {
				group._scene = sceneManager._fsm.GetScene( group._gameObject.scene );
			} else if ( sceneManager._fsm._mainFSM._scene != null ) {
				group._scene = sceneManager._fsm._mainFSM._scene;
			} else {
				group._scene = sceneManager._fsm._mainFSM.GetState( sceneManager._fsm._mainFSM._startStateType );
			}

			// 親子変更等で、無理矢理、削除不能化した場合を考慮
			if ( group._scene != null || group._scene == sceneManager._fsm._foreverScene ) {
				group._lifeSpan = SMTaskLifeSpan.Forever;
			}
#if TestGroupModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( lastScene )} : {lastScene}",
				$"{nameof( group._lifeSpan )} : {group._lifeSpan}",
				$"{nameof( group._scene )} : {group._scene}"
			) );
#endif
			allObjects.ForEach( o => o._group = group );


			if ( lastScene == null ) {
#if TestGroupModifyler
				SMLog.Debug( $"Register : {group}" );
#endif
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				if ( group._groups != null ) {
					group._groups._modifyler.Register( new RegisterGroupSMGroupManager( group ) );
				}


			} else if ( group._scene != lastScene ) {
#if TestGroupModifyler
				SMLog.Debug( $"Reregister : {group}" );
#endif
				lastScene._groups._modifyler.Register( new SendReregisterGroupSMGroupManager( group ) );


			} else {
#if TestGroupModifyler
				SMLog.Debug( $"DontRegister : {group}" );
#endif
			}
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( SetAllData )} : end\n{group}" );
#endif
		}



		public static void FixedUpdate( SMGroup group, SMTaskType type ) {
			if ( !group._isFinalizing )	{ return; }
			if ( !group._isActive )		{ return; }
			if ( group._ranState < SMTaskRunState.InitialEnable )	{ return; }

			group._topObject.GetAllChildren()
				.ForEach( o => SMObjectBody.FixedUpdate( o, type ) );

			if ( type == SMTaskType.Work && group._ranState == SMTaskRunState.InitialEnable ) {
				group._ranState = SMTaskRunState.FixedUpdate;
			}
		}


		public static void Update( SMGroup group, SMTaskType type ) {
			if ( !group._isFinalizing )	{ return; }
			if ( !group._isActive )		{ return; }
			if ( group._ranState < SMTaskRunState.FixedUpdate )	{ return; }

			group._topObject.GetAllChildren()
				.ForEach( o => SMObjectBody.Update( o, type ) );

			if ( type == SMTaskType.Work && group._ranState == SMTaskRunState.FixedUpdate ) {
				group._ranState = SMTaskRunState.Update;
			}
		}


		public static void LateUpdate( SMGroup group, SMTaskType type ) {
			if ( !group._isFinalizing )	{ return; }
			if ( !group._isActive )		{ return; }
			if ( group._ranState < SMTaskRunState.Update )	{ return; }
			
			group._topObject.GetAllChildren()
				.ForEach( o => SMObjectBody.LateUpdate( o, type ) );

			if ( type == SMTaskType.Work && group._ranState == SMTaskRunState.Update ) {
				group._ranState = SMTaskRunState.LateUpdate;
			}
		}
	}
}