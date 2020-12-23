//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using Task.Behaviour;
	using Task.Behaviour.Modifyler;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestSMBehaviourSMUtility {
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



		public static SMMultiDisposable SetRunKey( ISMBehaviour behaviour ) {
			var disposables = new SMMultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha1 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Create}" );
					RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Create ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha2 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.SelfInitialize}" );
					RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.SelfInitialize ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha3 ) ).Subscribe( (Action<long>)(_ => {
					SMLog.Warning( $"key down {SMTaskRunState.Initialize}" );
					RunStateSMBehaviour.RegisterAndRun( behaviour, (SMTaskRunState)SMTaskRunState.Initialize ).Forget();
				}) ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha4 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.FixedUpdate}" );
					RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha5 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.Update}" );
					RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha6 ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskRunState.LateUpdate}" );
					RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown(KeyCode.Alpha7 ) ).Subscribe( (Action<long>)(_ => {
					SMLog.Warning( $"key down {SMTaskRunState.Finalize}" );
					RunStateSMBehaviour.RegisterAndRun( behaviour, (SMTaskRunState)SMTaskRunState.Finalize ).Forget();
				}) )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Enable}" );
					ChangeActiveSMBehaviour.RegisterAndRun( behaviour, true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {SMTaskActiveState.Disable}" );
					ChangeActiveSMBehaviour.RegisterAndRun( behaviour, false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( ChangeActiveSMBehaviour.RegisterAndRunInitial )}" );
					ChangeActiveSMBehaviour.RegisterAndRunInitial( behaviour ).Forget();
				} )
			);
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					SMLog.Warning( $"key down {nameof( behaviour.Dispose )}" );
					behaviour.Dispose();
					behaviour = null;
				} )
			);

			return disposables;
		}



		public static void SetEvent( ISMBehaviour behaviour ) {
			var name = behaviour.GetAboutName();
			var id = behaviour._id;

			behaviour._selfInitializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {name}( {id} ).{nameof( behaviour._selfInitializeEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {name}( {id} ).{nameof( behaviour._selfInitializeEvent )}\n{behaviour}" );
			} );
			behaviour._initializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {name}( {id} ).{nameof( behaviour._initializeEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {name}( {id} ).{nameof( behaviour._initializeEvent )}\n{behaviour}" );
			} );
			behaviour._enableEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {name}( {id} ).{nameof( behaviour._enableEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {name}( {id} ).{nameof( behaviour._enableEvent )}\n{behaviour}" );
			} );
			behaviour._fixedUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{name}( {id} ).{nameof( behaviour._fixedUpdateEvent )}\n{behaviour}" );
			} );
			behaviour._updateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{name}( {id} ).{nameof( behaviour._updateEvent )}\n{behaviour}" );
			} );
			behaviour._lateUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{name}( {id} ).{nameof( behaviour._lateUpdateEvent )}\n{behaviour}" );
			} );
			behaviour._disableEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {name}( {id} ).{nameof( behaviour._disableEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {name}( {id} ).{nameof( behaviour._disableEvent )}\n{behaviour}" );
			} );
			behaviour._finalizeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {name}( {id} ).{nameof( behaviour._finalizeEvent )}\n{behaviour}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {name}( {id} ).{nameof( behaviour._finalizeEvent )}\n{behaviour}" );
			} );
		}



		public static void LogBehaviour( string text, ISMBehaviour behaviour )
			=> SMLog.Debug( $"{text} : {behaviour?.ToLineString() ?? "null"}" );

		public static void LogBehaviours( string text, IEnumerable<ISMBehaviour> behaviours ) {
			var bs = behaviours.ToArray();
			SMLog.Debug( string.Join( "\n",
				$"{text} : {bs.Count()}",
				string.Join( "\n", bs.Select( b => b?.ToLineString() ?? "null" ) )
			) );
		}
	}



	public abstract class BaseB : SMBehaviour {
		public BaseB( bool isDebug = false ) : base( isDebug ) {
			SMLog.Debug( $"{nameof( BaseB )}() : {this}" );
		}
		public override void Create() {
			SMLog.Debug( $"{nameof( Create )} : {this}" );
		}
	}
	public class B1 : BaseB {
		public B1( bool isDebug = false ) : base( isDebug ) {}
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class B2 : BaseB {
		public B2( bool isDebug = false ) : base( isDebug ) {}
		public override SMTaskType _type => SMTaskType.Work;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class B3 : BaseB {
		public B3( bool isDebug = false ) : base( isDebug ) {}
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;
	}
	public class B4 : BaseB {
		public B4( bool isDebug = false ) : base( isDebug ) {}
		public override SMTaskType _type => SMTaskType.DontWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}
	public class B5 : BaseB {
		public B5( bool isDebug = false ) : base( isDebug ) {}
		public override SMTaskType _type => SMTaskType.Work;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}
	public class B6 : BaseB {
		public B6( bool isDebug = false ) : base( isDebug ) {}
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
	}



	public abstract class BaseM : SMMonoBehaviour {
		public BaseM() {
			SMLog.Debug( $"{nameof( BaseM )}() : {this}" );
		}
		public override void Create() {
			SMLog.Debug( $"{nameof( Create )} : {this}" );
		}
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