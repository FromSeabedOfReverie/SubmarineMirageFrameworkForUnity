//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Scene;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMObjectModifyData {
		public enum ModifyType {
			Linker,
			Runner,
		}

		public abstract ModifyType _type	{ get; }
		static uint s_idCount;
		public uint _id					{ get; private set; }
		public SMObject _object;


		public SMObjectModifyData( SMObject smObject ) {
			_id = ++s_idCount;
			_object = smObject;
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( SMObjectModifyData )}() : {this}" );
#endif
		}

		public abstract void Cancel();

		public abstract UniTask Run();


		protected void RegisterObject() {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( RegisterObject )} : start\n{this}" );
			if ( _object._objects != null ) {
				Log.Debug( string.Join( "\n",
					_object._objects._objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
#endif
			var o = _object._objects._objects[_object._type];
			if ( o != null ) {
				AddObject( o, _object );
			} else {
				_object._objects._objects[_object._type] = _object;
			}
#if TestSMTaskModifyler
			if ( _object._objects != null ) {
				Log.Debug( string.Join( "\n",
					_object._objects._objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
			Log.Debug( $"{nameof( RegisterObject )} : end\n{this}" );
#endif
		}

		protected void AddObject( SMObject brother, SMObject add ) {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( AddObject )} : start\n{this}" );
#endif
			var last = brother.GetLast();
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( last )} : {last?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}"
			) );
#endif
			add._parent = last._parent;
			add._previous = last;
			last._next = add;
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( last )} : {last?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}"
			) );
			Log.Debug( $"{nameof( AddObject )} : end\n{this}" );
#endif
		}

		public static void AddChildObject( SMObject parent, SMObject add ) {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( AddChildObject )} : start" );
			Log.Debug( string.Join( "\n",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}"
			) );
#endif
			add._parent = parent;
			var last = parent.GetLastChild();
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( last )} : {last?.ToLineString()}" );
#endif
			if ( last != null ) {
				add._previous = last;
				last._next = add;
			} else {
				parent._child = add;
			}
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}",
				$"{nameof( last )} : {last?.ToLineString()}"
			) );
			Log.Debug( $"{nameof( AddChildObject )} : end" );
#endif
		}

		public static void SetTopObject( SMObject smObject ) {
			SMObject top;
			for ( top = smObject; top._parent != null; top = top._parent ) {}
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( smObject )} : {smObject?.ToLineString()}",
				$"{nameof( top )} : {top?.ToLineString()}"
			) );
#endif
			SetAllObjectData( top );
		}

		public static void SetAllObjectData( SMObject top ) {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( SetAllObjectData )} : start\n{top}" );
#endif
			var lastType = top._type;
			var lastScene = top._scene;
			var allObjects = top.GetAllChildren();
			var allBehaviours = allObjects.SelectMany( o => o.GetBehaviours() );
#if TestSMTaskModifyler
			Log.Debug(
				$"{nameof( allObjects )} :\n"
					+ $"{string.Join( "\n", allObjects.Select( o => o?.ToLineString() ) )}"
			);
			Log.Debug(
				$"{nameof( allBehaviours )} :\n"
					+ $"{string.Join( "\n", allBehaviours.Select( b => b?.ToLineString() ) )}"
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
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( lastType )} : {lastType}",
				$"{nameof( lastScene )} : {lastScene}",
				$"{nameof( top._type )} : {top._type}",
				$"{nameof( top._lifeSpan )} : {top._lifeSpan}",
				$"{nameof( top._scene )} : {top._scene}"
			) );
#endif
			allObjects.ForEach( o => {
				o._top = top;
				o._type = top._type;
				o._lifeSpan = top._lifeSpan;
				o._scene = top._scene;
			} );

			if ( lastScene == null ) {
#if TestSMTaskModifyler
				Log.Debug( $"Register : {top}" );
#endif
				// SceneManager作成時の場合、循環参照になる為、設定出来ない
				if ( top._objects != null ) {
					top._modifyler.Register( new RegisterSMObject( top ) );
				}
			} else if ( top._type != lastType || top._scene != lastScene ) {
#if TestSMTaskModifyler
				Log.Debug( $"ReRegister : {top}" );
#endif
				top._modifyler.Register( new ReRegisterSMObject( top, lastType, lastScene ) );
			} else {
#if TestSMTaskModifyler
				Log.Debug( $"DontRegister : {top}" );
#endif
			}
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( SetAllObjectData )} : end\n{top}" );
#endif
		}


		public static void UnLinkObject( SMObject smObject ) {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( UnLinkObject )} : start" );
			var objects = smObject?._objects?._objects;
			var parent = smObject?._parent;
			var previous = smObject?._previous;
			var next = smObject?._next;
			if ( objects != null ) {
				Log.Debug( string.Join( "\n",
					objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
			Log.Debug( string.Join( "\n",
				$"{nameof( smObject )} : {smObject?.ToLineString()}",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( previous )} : {previous?.ToLineString()}",
				$"{nameof( next )} : {next?.ToLineString()}"
			) );
#endif
			if ( smObject._objects?._objects.GetOrDefault( smObject._type ) == smObject ) {
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
#if TestSMTaskModifyler
			if ( objects != null ) {
				Log.Debug( string.Join( "\n",
					objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
			Log.Debug( string.Join( "\n",
				$"{nameof( smObject )} : {smObject?.ToLineString()}",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( previous )} : {previous?.ToLineString()}",
				$"{nameof( next )} : {next?.ToLineString()}"
			) );
			Log.Debug( $"{nameof( UnLinkObject )} : end" );
#endif
		}


		public override string ToString() => string.Join( " ",
			$"{this.GetAboutName()}(",
			$"    {_id}",
			$"    {_type}",
			$"    {_object?.ToLineString()}",
			")"
		);
	}
}