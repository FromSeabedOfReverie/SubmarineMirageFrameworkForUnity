//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTask
namespace SubmarineMirage.SMTask.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Scene;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMObjectModifyData {
		public SMObject _object;


		public SMObjectModifyData( SMObject smObject ) {
			_object = smObject;
#if TestSMTask
			Log.Debug( $"{this.GetAboutName()}() : {_object}" );
#endif
		}

		public abstract void Cancel();

		public abstract UniTask Run();


		protected void RegisterObject() {
			var o = _object._objects._objects[_object._type];
			if ( o != null )	{ AddObject( o, _object ); }
			else				{ _object._objects._objects[_object._type] = _object; }
		}

		protected void AddObject( SMObject brother, SMObject add ) {
			UnLinkObject( add );

			var last = brother.GetLast();
			add._parent = last._parent;
			add._previous = last;
			last._next = add;
		}

		public static void AddChildObject( SMObject parent, SMObject add ) {
			UnLinkObject( add );

			add._parent = parent;
			var last = parent.GetLastChild();
			if ( last != null ) {
				add._previous = last;
				last._next = add;
			} else {
				parent._child = add;
			}
		}

		public static void SetTopObject( SMObject smObject ) {
			SMObject top;
			for ( top = smObject; top._parent != null; top = top._parent ) {}
			SetAllObjectData( top );
		}

		public static void SetAllObjectData( SMObject top ) {
#if TestSMTask
			Log.Debug( $"{nameof( SetAllObjectData )} : start\n{top}" );
#endif
			var lastType = top._type;
			var lastScene = top._scene;
			var allObjects = top.GetAllChildren();
			var allBehaviours = allObjects.SelectMany( o => o.GetBehaviours() );
#if TestSMTask
			Log.Debug(
				$"{nameof( allBehaviours )} : \n"
					+ $"{string.Join( ", ", allBehaviours.Select( b => b.GetAboutName() ) )}"
			);
#endif
			top._type = (
				allBehaviours.Any( b => b._type == SMTaskType.FirstWork )	? SMTaskType.FirstWork :
				allBehaviours.Any( b => b._type == SMTaskType.Work )		? SMTaskType.Work
																			: SMTaskType.DontWork
			);
			top._lifeSpan = allBehaviours.Any( b => b._lifeSpan == SMTaskLifeSpan.Forever ) ?
				SMTaskLifeSpan.Forever : SMTaskLifeSpan.InScene;
			top._scene = (
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				!SceneManager.s_isCreated					? null :
				top._lifeSpan == SMTaskLifeSpan.Forever		? SceneManager.s_instance._fsm._foreverScene :
				top._owner != null							? SceneManager.s_instance._fsm.Get( top._owner.scene ) :
				SceneManager.s_instance._fsm._scene != null	? SceneManager.s_instance._fsm._scene
															: SceneManager.s_instance._fsm._startScene
			);

			allObjects.ForEach( o => {
				o._top = top;
				o._type = top._type;
				o._lifeSpan = top._lifeSpan;
				o._scene = top._scene;
			} );

			if ( lastScene == null ) {
#if TestSMTask
				Log.Debug( $"Register : {top}" );
#endif
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				if ( top._objects != null ) {
					top._modifyler.Register( new RegisterSMObject( top ) );
				}
			} else if ( top._type != lastType || top._scene != lastScene ) {
#if TestSMTask
				Log.Debug( $"ReRegister : {top}" );
#endif
				top._modifyler.Register( new ReRegisterSMObject( top, lastType, lastScene ) );
			} else {
#if TestSMTask
				Log.Debug( $"DontRegister : {top}" );
#endif
			}
#if TestSMTask
			Log.Debug( $"{nameof( SetAllObjectData )} : end\n{top}" );
#endif
		}

		public static void UnLinkObject( SMObject smObject ) {
			if ( smObject._objects?._objects[smObject._type] == smObject ) {
				smObject._objects._objects[smObject._type] = smObject._next;
			}

			if ( smObject._parent?._child == smObject ) {
				smObject._parent._child = smObject._next;
			}
			smObject._parent = null;

			if ( smObject._previous != null )	{ smObject._previous._next = smObject._next; }
			if ( smObject._next != null )		{ smObject._next._previous = smObject._previous; }
			smObject._previous = null;
			smObject._next = null;
		}


		public override string ToString()
			=> $"{this.GetAboutName()}( {_object._behaviour.GetAboutName()} )";
	}
}