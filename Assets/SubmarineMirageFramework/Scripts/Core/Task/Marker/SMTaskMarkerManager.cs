//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestTask
namespace SubmarineMirage.Task.Marker {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Base;
	using Service;
	using Extension;
	using Debug;



	public class SMTaskMarkerManager : SMStandardBase {
		[SMShowLine] public string _name	{ get; private set; }

		[SMShow] readonly Dictionary< SMTaskRunType, List<SMTask> > _markers =
			new Dictionary< SMTaskRunType, List<SMTask> >();

		[SMShow] public bool _isInitialized		{ get; private set; }
		[SMShow] bool _isRunning				{ get; set; }
		[SMShow] new bool _isDispose			{ get; set; }



		#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _markers ), i => _toStringer.DefaultValue( _markers, i, true ) );
		}
#endregion



		public SMTaskMarkerManager( string name ) {
			_name = name;

			var markerTypes = EnumUtils.GetValues<SMTaskMarkerType>();
			SMTaskManager.CREATE_TASK_TYPES.ForEach( type => {
				_markers[type] = new List<SMTask>();
				markerTypes.ForEach( mType => {
					_markers[type].Add( new SMTaskMarker( _name, type, mType ) );
				} );
			} );

			_disposables.AddFirst( () => {
#if TestTask
				SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( Dispose )} : start\n{this}" );
#endif
				SMTaskManager.DISPOSE_TASK_TYPES
					.SelectMany( t => GetAlls( t, true ).Reverse() )
					.ForEach( t => t.Dispose() );
				_markers.Clear();
				_isRunning = false;
				_isDispose = true;
#if TestTask
				SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( Dispose )} : end\n{this}" );
#endif
			} );
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}() : \n{this}" );
#endif
		}



		public void LinkLast( SMTask add ) {
			CheckDisposeError( $"{nameof( LinkLast )}( {add.GetAboutName()} )" );
			if ( add == null || add._isDispose ) {
				throw new ObjectDisposedException(
					$"{add}",
					string.Join( "\n",
						$"既に解放済",
						$"{this.GetAboutName()}.{nameof( LinkLast )}( {add.GetAboutName()} )"
					)
				);
			}

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( LinkLast )} : start\n{this}" );
#endif
			var last = GetLast( add._type, true )._previous;
			last.Link( add );
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( LinkLast )} : end\n{this}" );
#endif
		}



		public SMTask GetFirst( SMTaskRunType type, bool isRaw = false ) {
			if ( _isDispose ) {
				CheckDisposeError( $"{nameof( GetFirst )}( {type}, {isRaw} )" );
			}

			if ( isRaw ) {
				return _markers[type][( int )SMTaskMarkerType.First];
			}

			var first = GetFirst( type, true )._next;
			var last = GetLast( type, true );

			if ( first == last )	{ return null; }
			return first;
		}

		public SMTask GetLast( SMTaskRunType type, bool isRaw = false ) {
			if ( _isDispose ) {
				CheckDisposeError( $"{nameof( GetLast )}( {type}, {isRaw} )" );
			}

			if ( isRaw ) {
				return _markers[type][( int )SMTaskMarkerType.Last];
			}

			var first = GetFirst( type, true );
			var last = GetLast( type, true )._previous;

			if ( first == last )	{ return null; }
			return last;
		}



		public IEnumerable<SMTask> GetAlls( SMTaskRunType type, bool isRaw = false ) {
			if ( _isDispose ) {
				CheckDisposeError( $"{nameof( GetAlls )}( {type}, {isRaw} )" );
			}

			var first = GetFirst( type, isRaw );
			if ( first == null )	{ yield break; }
			var last = GetLast( type, isRaw )._next;

			for ( var t = first; t != last; t = t._next ) {
				yield return t;
			}
		}

		public IEnumerable<SMTask> GetAlls( bool isRaw = false ) {
			if ( _isDispose ) {
				CheckDisposeError( $"{nameof( GetAlls )}( {isRaw} )" );
			}

			foreach ( var type in SMTaskManager.CREATE_TASK_TYPES ) {
				foreach ( var task in GetAlls( type, isRaw ) ) {
					yield return task;
				}
			}
		}



		public async UniTask InitializeAll() {
			CheckDisposeError( nameof( InitializeAll ) );
			CheckDuplicateRunningError( nameof( InitializeAll ) );
			if ( _isInitialized ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"既に初期化済 : ",
					$"{this.GetAboutName()}.{nameof( InitializeAll )}",
					$"{this}"
				) );
			}

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( InitializeAll )} : start\n{this}" );
#endif
			_isRunning = true;
			var manager = SMServiceLocator.Resolve<SMTaskManager>();

			SMTaskManager.CREATE_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.CreateTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( InitializeAll )} : " +
				$"{nameof( manager.CreateTask )} end\n{this}" );
#endif

			SMTaskManager.RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.SelfInitializeTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( InitializeAll )} : " +
				$"{nameof( manager.SelfInitializeTask )} end\n{this}" );
#endif

			SMTaskManager.RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.InitializeTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( InitializeAll )} : " +
				$"{nameof( manager.InitializeTask )} end\n{this}" );
#endif

			SMTaskManager.RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ) )
				.ForEach( task => manager.InitialEnableTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( InitializeAll )} : " +
				$"{nameof( manager.InitialEnableTask )} end\n{this}" );
#endif

			_isInitialized = true;
			_isRunning = false;
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( InitializeAll )} : end\n{this}" );
#endif
		}

		public async UniTask FinalizeAll() {
			CheckDisposeError( nameof( FinalizeAll ) );
			CheckDuplicateRunningError( nameof( FinalizeAll ) );

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( FinalizeAll )} : start\n{this}" );
#endif
			_isRunning = true;
			var manager = SMServiceLocator.Resolve<SMTaskManager>();

			SMTaskManager.DISPOSE_RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ).Reverse() )
				.ForEach( task => manager.FinalDisableTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( FinalizeAll )} : " +
				$"{nameof( manager.FinalDisableTask )} end\n{this}" );
#endif

			SMTaskManager.DISPOSE_RUN_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ).Reverse() )
				.ForEach( task => manager.FinalizeTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( FinalizeAll )} : " +
				$"{nameof( manager.FinalizeTask )} end\n{this}" );
#endif

			SMTaskManager.DISPOSE_TASK_TYPES
				.SelectMany( type => GetAlls( type, true ).Reverse() )
				.ForEach( task => manager.DisposeTask( task ).Forget() );
			await manager._modifyler.WaitRunning();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( FinalizeAll )} : " +
				$"{nameof( manager.DisposeTask )} end\n{this}" );
#endif

			Dispose();
			_isRunning = false;
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskMarkerManager )}.{nameof( FinalizeAll )} : end\n{this}" );
#endif
		}



		void CheckDuplicateRunningError( string name ) {
			if ( !_isRunning )	{ return; }

			throw new InvalidOperationException( string.Join( "\n",
				$"初期化か、終了処理が、同時に実行された : ",
				$"{nameof( SMTaskMarkerManager )}.{name}",
				$"{this}"
			) );
		}
	}
}