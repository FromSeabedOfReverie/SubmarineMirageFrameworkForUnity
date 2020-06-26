//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestProcess {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using SMTask;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestProcessUtility {
		public static ISMBehavior CreateMonoBehaviourProcess( string processData ) {
			ISMBehavior top = null;

			var parents = new Dictionary<int, Transform>();
			processData
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
						var c = go.AddComponent( Type.GetType( $"{typeof(BaseM).Namespace}.{t}" ) );
						if ( top == null )	{ top = (ISMBehavior)c; }
					} );
				} );

			return top;
		}


		public static MultiDisposable SetRunKey( SMObject hierarchy ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					hierarchy.RunStateEvent( SMTaskRanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					hierarchy.RunStateEvent( SMTaskRanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					hierarchy.RunStateEvent( SMTaskRanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					hierarchy.RunStateEvent( SMTaskRanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					hierarchy.RunStateEvent( SMTaskRanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					hierarchy.RunStateEvent( SMTaskRanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					hierarchy.RunStateEvent( SMTaskRanState.Finalizing ).Forget();
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling Owner" );
					hierarchy.ChangeActive( true, true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling Owner" );
					hierarchy.ChangeActive( false, true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					hierarchy.ChangeActive( true, false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					hierarchy.ChangeActive( false, false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down RunActiveEvent" );
					hierarchy.RunActiveEvent().Forget();
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					hierarchy.Dispose();
					hierarchy = null;
				} )
			);

			return disposables;
		}


		public static MultiDisposable SetChangeActiveKey( SMObject hierarchy ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling Child Owner" );
					hierarchy.ChangeActive( true, true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.F ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling Child Owner" );
					hierarchy.ChangeActive( false, true ).Forget();
				} )
			);

			return disposables;
		}


		public static void SetEvent( ISMBehavior process ) {
			var name = process.GetAboutName();
			var id = (
				process is BaseM	? ( (BaseM)process )._id :
				process is BaseB	? ( (BaseB)process )._id
									: (int?)null
			);

			process._loadEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._loadEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._loadEvent : end" );
			} );
			process._initializeEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._initializeEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._initializeEvent : end" );
			} );
			process._enableEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._enableEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._enableEvent : end" );
			} );
			process._fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} )._fixedUpdateEvent" );
			} );
			process._updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} )._updateEvent" );
			} );
			process._lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} )._lateUpdateEvent" );
			} );
			process._disableEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._disableEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._disableEvent : end" );
			} );
			process._finalizeEvent.AddLast( async cancel => {
				Log.Debug( $"{name}( {id} )._finalizeEvent : start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( $"{name}( {id} )._finalizeEvent : end" );
			} );
		}


		public static void LogHierarchy( string text, SMObject hierarchy ) {
			if ( hierarchy == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = hierarchy._owner != null ? hierarchy._owner.name : null;
			Log.Debug( $"{text} : " + string.Join( ", ",
				hierarchy._processes.Select( p => p.GetAboutName() )
			) + $" : {name}" );
		}

		public static void LogHierarchies( string text, IEnumerable<SMObject> hierarchies ) {
			Log.Debug( $"{text} :\n" + string.Join( "\n",
				hierarchies.Select( h => {
					var name = h._owner != null ? h._owner.name : null;
					return string.Join( ", ",
						h._processes.Select( p => p.GetAboutName() )
					) + $" : {name}";
				} )
			) );
		}


		public static void LogProcess( string text, ISMBehavior process ) {
			if ( process == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = process._object._owner != null ? process._object._owner.name : null;
			var id = ( process as BaseM )?._id ?? ( process as BaseB )?._id;
			Log.Debug( $"{text} : {process.GetAboutName()}, {name}, processID : {id}" );
		}

		public static void LogProcesses( string text, IEnumerable<ISMBehavior> processes ) {
			Log.Debug( $"{text} :\n" + string.Join( "\n",
				processes.Select( p => {
					var name = p._object._owner != null ? p._object._owner.name : null;
					var id = ( p as BaseM )?._id ?? ( p as BaseB )?._id;
					return $"{p.GetAboutName()}, {name}, processID : {id}";
				} )
			) );
		}
	}



	public abstract class BaseB : SMBehavior {
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