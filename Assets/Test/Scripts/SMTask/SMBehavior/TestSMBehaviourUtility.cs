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
	using UTask;
	using SMTask;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestSMBehaviourUtility {
		public static ISMBehaviour CreateBehaviours( string behaviourData ) {
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
					var go = new GameObject( $"[t{a.i} i{i}]" );
					go.SetActive( a.isActive );
					go.SetParent( parents.GetOrDefault( a.i - 1 ) );
					parents[a.i] = go.transform;
					
					a.types.ForEach( t => {
						var b = go.AddComponent( Type.GetType( $"{typeof( BaseM ).Namespace}.{t}" ) );
						if ( top == null )	{ top = (ISMBehaviour)b; }
					} );
				} );

			return top;
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



		public static void SetEvent( ISMBehaviour behaviour ) {
			var name = behaviour.GetAboutName();
			var id = behaviour._id;

			behaviour._loadEvent.AddLast( async canceler => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._loadEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._loadEvent )}\n{behaviour}" );
			} );
			behaviour._initializeEvent.AddLast( async canceler => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._initializeEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._initializeEvent )}\n{behaviour}" );
			} );
			behaviour._enableEvent.AddLast( async canceler => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._enableEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._enableEvent )}\n{behaviour}" );
			} );
			behaviour._fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._fixedUpdateEvent )}\n{behaviour}" );
			} );
			behaviour._updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._updateEvent )}\n{behaviour}" );
			} );
			behaviour._lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._lateUpdateEvent )}\n{behaviour}" );
			} );
			behaviour._disableEvent.AddLast( async canceler => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._disableEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._disableEvent )}\n{behaviour}" );
			} );
			behaviour._finalizeEvent.AddLast( async canceler => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._finalizeEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._finalizeEvent )}\n{behaviour}" );
			} );
		}



		public static void LogBehaviour( string text, ISMBehaviour behaviour )
			=> Log.Debug( $"{text} : {behaviour?.ToLineString() ?? "null"}" );

		public static void LogBehaviours( string text, IEnumerable<ISMBehaviour> behaviours ) {
			var bs = behaviours.ToArray();
			Log.Debug( string.Join( "\n",
				$"{text} : {bs.Count()}",
				string.Join( "\n", bs.Select( b => b?.ToLineString() ?? "null" ) )
			) );
		}
	}



	public abstract class BaseB : SMBehaviour {
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