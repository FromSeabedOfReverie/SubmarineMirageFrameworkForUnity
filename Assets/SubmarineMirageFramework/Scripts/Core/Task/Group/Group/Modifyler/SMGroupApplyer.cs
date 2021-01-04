//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using System.Linq;
	using KoganeUnityLib;
	using Object;
	using Manager.Modifyler;
	using Scene;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMGroupApplyer {
		public static void SetTop( SMGroup group, SMObject smObject ) {
			group._topObject = smObject.GetTop();
			SetAllData( group );
		}



		public static void SetAllData( SMGroup group ) {
#if TestGroupModifyler
			SMLog.Debug( $"{nameof( SetAllData )} : start\n{group}" );
#endif
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
			group._scene = (
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				!SMSceneManager.s_isCreated						? null :
				group._lifeSpan == SMTaskLifeSpan.Forever		? SMSceneManager.s_instance._fsm._foreverScene :
				group._topObject._isGameObject
											? SMSceneManager.s_instance._fsm.Get( group._topObject._owner.scene ) :
				SMSceneManager.s_instance._fsm._scene != null	? SMSceneManager.s_instance._fsm._scene
																: SMSceneManager.s_instance._fsm._startScene
			);
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



		public static void Move( SMGroup newGroup, SMGroup remove ) {
			newGroup._modifyler.Move( remove._modifyler );
			remove._topObject.GetAllChildren().ForEach( o => o._group = newGroup );
			remove._topObject = null;
			remove.Dispose();
		}
	}
}