//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using SMTask;
	using SMTask.Modifyler;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestSMTaskUtility {
		public static ISMBehaviour CreateSMMonoBehaviour( string behaviourData ) {
			ISMBehaviour top = null;

			var parents = new Dictionary<int, Transform>();
			behaviourData
				.UnifyNewLine()
				.Split( '\n' )
				.Select( s => new {
					i = s.CountString( "\t" ),
					s = s
						.Replace( "\t", "" )
						.Replace( " ", "" ),
				} )
				.Where( a => !a.s.IsNullOrEmpty() )
				.Select( a => new {
					a.i,
					types = a.s
						.Split( "," )
						.Where( s => !s.IsNullOrEmpty() )
						.Where( s => s != "null" )
						.ToList(),
				} )
				.Select( a => {
					var isActiveNullable = a.types.FirstOrDefault().ToBooleanOrNull();
					if ( isActiveNullable.HasValue )	{ a.types.RemoveAt( 0 ); }
					else								{ isActiveNullable = true; }
					var isActive = isActiveNullable.Value;
					return new { a.i, isActive, a.types };
				} )
				.ForEach( ( a, i ) => {
					var go = new GameObject( $"indent : {a.i}, id : {i}" );
					go.SetActive( a.isActive );
					go.SetParent( parents.GetOrDefault( a.i - 1 ) );
					parents[a.i] = go.transform;
					
					a.types.ForEach( t => {
						var b = go.AddComponent( Type.GetType( $"{typeof(BaseM).Namespace}.{t}" ) );
						if ( top == null )	{ top = (ISMBehaviour)b; }
					} );
				} );

			return top;
		}


		public static MultiDisposable SetRunKey( SMObject smObject ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Creating ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Loading ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Initializing ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.FixedUpdate ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Update ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.LateUpdate ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					smObject._top._modifyler.Register( new RunStateSMObject( smObject, SMTaskRanState.Finalizing ) );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, false ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, false ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down RunActiveEvent" );
					smObject._top._modifyler.Register( new RunActiveSMObject( smObject ) );
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					smObject.Dispose();
					smObject = null;
				} )
			);

			return disposables;
		}

		public static MultiDisposable SetRunKey( ISMBehaviour behaviour ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					behaviour.RunStateEvent( SMTaskRanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					behaviour.RunStateEvent( SMTaskRanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					behaviour.RunStateEvent( SMTaskRanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					behaviour.RunStateEvent( SMTaskRanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					behaviour.RunStateEvent( SMTaskRanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					behaviour.RunStateEvent( SMTaskRanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					behaviour.RunStateEvent( SMTaskRanState.Finalizing ).Forget();
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					behaviour.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					behaviour.ChangeActive( false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down RunActiveEvent" );
					behaviour.RunActiveEvent().Forget();
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					behaviour.Dispose();
					behaviour = null;
				} )
			);

			return disposables;
		}


		public static MultiDisposable SetChangeActiveKey( SMObject smObject ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling Child Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, true, true ) );
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.F ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling Child Owner" );
					smObject._top._modifyler.Register( new ChangeActiveSMObject( smObject, false, true ) );
				} )
			);

			return disposables;
		}


		public static void SetEvent( ISMBehaviour behaviour ) {
			var name = behaviour.GetAboutName();
			var id = (
				behaviour is BaseM	? ( (BaseM)behaviour )._id :
				behaviour is BaseB	? ( (BaseB)behaviour )._id
									: (int?)null
			);

			behaviour._loadEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._loadEvent )}" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._loadEvent )}" );
			} );
			behaviour._initializeEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._initializeEvent )}" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._initializeEvent )}" );
			} );
			behaviour._enableEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._enableEvent )}" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._enableEvent )}" );
			} );
			behaviour._fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._fixedUpdateEvent )}" );
			} );
			behaviour._updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._updateEvent )}" );
			} );
			behaviour._lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._lateUpdateEvent )}" );
			} );
			behaviour._disableEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._disableEvent )}" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._disableEvent )}" );
			} );
			behaviour._finalizeEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._finalizeEvent )}" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._finalizeEvent )}" );
			} );
		}


		public static void LogSMObject( string text, SMObject smObject ) {
			if ( smObject == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = smObject._owner != null ? smObject._owner.name : null;
			Log.Debug(
				$"{text} : "
					+ string.Join( ", ", smObject.GetBehaviours().Select( b => b.GetAboutName() ) )
					+ $" : {name}"
			);
		}

		public static void LogSMObjects( string text, IEnumerable<SMObject> smObjects ) {
			Log.Debug(
				$"{text} :\n"
					+ string.Join( "\n",
						smObjects.Select( o => {
							var name = o._owner != null ? o._owner.name : null;
							return string.Join( ", ", o.GetBehaviours().Select( b => b.GetAboutName() ) )
								+ $" : {name}";
						} )
					)
			);
		}


		public static void LogBehaviour( string text, ISMBehaviour behaviour ) {
			if ( behaviour == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = behaviour._object._owner != null ? behaviour._object._owner.name : null;
			var id = ( behaviour as BaseM )?._id ?? ( behaviour as BaseB )?._id;
			Log.Debug( $"{text} : {behaviour.GetAboutName()}, {name}, behaviourID : {id}" );
		}

		public static void LogBehaviours( string text, IEnumerable<ISMBehaviour> behaviours ) {
			Log.Debug(
				$"{text} :\n"
					+ string.Join( "\n",
						behaviours.Select( b => {
							var name = b._object._owner != null ? b._object._owner.name : null;
							var id = ( b as BaseM )?._id ?? ( b as BaseB )?._id;
							return $"{b.GetAboutName()}, {name}, behaviourID : {id}";
						} )
					)
			);
		}
	}



	public abstract class BaseB : SMBehaviour {
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
		static int s_count = 0;
		public int _id;
		public BaseB() => _id = s_count++;
		public override void Create() {}
	}
	public class B1 : BaseB {
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class B2 : BaseB {
		public override SMTaskType _type => SMTaskType.Work;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class B3 : BaseB {
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class B4 : BaseB {
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}
	public class B5 : BaseB {
		public override SMTaskType _type => SMTaskType.Work;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}
	public class B6 : BaseB {
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}



	public abstract class BaseM : SMMonoBehaviour {
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
		static int s_count = 0;
		public int _id;
		public BaseM() => _id = s_count++;
		public override void Create() {}
	}
	public class M1 : BaseM {
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class M2 : BaseM {
		public override SMTaskType _type => SMTaskType.Work;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class M3 : BaseM {
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class M4 : BaseM {
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}
	public class M5 : BaseM {
		public override SMTaskType _type => SMTaskType.Work;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}
	public class M6 : BaseM {
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}
}