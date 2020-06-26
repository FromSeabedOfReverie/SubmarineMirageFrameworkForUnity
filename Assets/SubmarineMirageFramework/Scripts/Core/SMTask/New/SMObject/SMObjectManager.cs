//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Modifyler;
	using Scene;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMObjectManager : IDisposableExtension {
		public BaseScene _owner	{ get; private set; }
		public readonly Dictionary<SMTaskType, SMObject> _objects = new Dictionary<SMTaskType, SMObject>();
		public bool _isEnter	{ get; private set; }

		public CancellationToken _activeAsyncCancel => _owner._activeAsyncCancel;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObjectManager( BaseScene owner ) {
			_owner = owner;

			EnumUtils.GetValues<SMTaskType>().ForEach( t => _objects[t] = null );
			_disposables.AddLast( () => {
				_objects
					.SelectMany( pair => pair.Value?.GetBrothers() )
					.ForEach( o => o?.Dispose() );
				_objects.Clear();
			} );
		}

		public void Dispose() => _disposables.Dispose();

		~SMObjectManager() => Dispose();



		public IEnumerable<SMObject> Get( SMTaskType? type = null, bool isReverse = false ) {
			var result = (
				type.HasValue	? _objects[type.Value]?.GetBrothers() ?? Enumerable.Empty<SMObject>()
								: _objects
									.Select( pair => pair.Value )
									.Where( o => o != null )
									.SelectMany( o => o.GetBrothers() )
			);

			if ( isReverse ) {
/*
				var befor = string.Join(
					", ",
					(	type.HasValue	? _objects[type.Value].GetBrothers()
										: _objects
											.Where( pair => pair.Value != null )
											.SelectMany( pair => pair.Value.GetBrothers() )
					).Select( o => o._behavior.GetAboutName() )
				);
				Log.Debug( $"befor reverse : {befor}" );
*/
				result = result.Reverse();
/*
				Log.Debug( $"new : {string.Join( ", ", result.Select( o => o._behavior.GetAboutName() ) )}" );
				var after = string.Join(
					", ",
					(	type.HasValue	? _objects[type.Value].GetBrothers()
										: _objects
											.Where( pair => pair.Value != null )
											.SelectMany( pair => pair.Value.GetBrothers() )
					).Select( o => o._behavior.GetAboutName() )
				);
				Log.Debug( $"after reverse : {after}" );
*/
			}
			return result;
		}

		public SMObject GetLast( SMTaskType type )
			=> _objects.GetOrDefault( type )?.GetLast() ?? null;


		public T GetBehaviour<T>( SMTaskType? type = null ) where T : ISMBehavior
			=> Get( type )
				.Select( o => o.GetBehaviourInChildren<T>() )
				.FirstOrDefault( b => b != null );

		public ISMBehavior GetBehaviour( Type systemType, SMTaskType? type = null )
			=> Get( type )
				.Select( o => o.GetBehaviourInChildren( systemType ) )
				.FirstOrDefault( b => b != null );

		public IEnumerable<T> GetBehaviours<T>( SMTaskType? type = null ) where T : ISMBehavior
			=> Get( type )
				.SelectMany( o => o.GetBehavioursInChildren<T>() );

		public IEnumerable<ISMBehavior> GetBehaviours( Type systemType, SMTaskType? type = null )
			=> Get( type )
				.SelectMany( o => o.GetBehavioursInChildren( systemType ) );



		public void Add( SMObject smObject ) {
			var last = GetLast( smObject._type );
			if ( last != null )	{ last.Add( smObject ); }
			else				{ _objects[smObject._type] = smObject; }
		}



		public async UniTask RunAllStateEvents( SMTaskType type, SMTaskRanState state ) {
			var os = Get( type, type == SMTaskType.FirstWork && state == SMTaskRanState.Finalizing );
			switch ( type ) {
				case SMTaskType.FirstWork:
					os.ForEach( o => o._modifyler.Register( new RunStateSMObject( o, state ) ) );
					foreach ( var o in os ) {
						await o._modifyler.WaitRunning();
					}
					break;

				case SMTaskType.Work:
					os.ForEach( o => o._modifyler.Register( new RunStateSMObject( o, state ) ) );
					await UniTask.WhenAll(
						os.Select( o => o._modifyler.WaitRunning() )
					);
					break;
			}
		}

		public async UniTask ChangeAllActives( SMTaskType type, bool isActive ) {
			var os = Get( type, type == SMTaskType.FirstWork && !isActive );
			switch ( type ) {
				case SMTaskType.FirstWork:
					os.ForEach( o => o._modifyler.Register( new ChangeActiveSMObject( o, isActive, true ) ) );
					foreach ( var o in os ) {
						await o._modifyler.WaitRunning();
					}
					break;

				case SMTaskType.Work:
					os.ForEach( o => o._modifyler.Register( new ChangeActiveSMObject( o, isActive, true ) ) );
					await UniTask.WhenAll(
						os.Select( o => o._modifyler.WaitRunning() )
					);
					break;
			}
		}

		public async UniTask RunAllActiveEvents( SMTaskType type ) {
			var os = Get( type );
			switch ( type ) {
				case SMTaskType.FirstWork:
					os.ForEach( o => o._modifyler.Register( new RunActiveSMObject( o ) ) );
					foreach ( var o in os ) {
						await o._modifyler.WaitRunning();
					}
					break;

				case SMTaskType.Work:
					os.ForEach( o => o._modifyler.Register( new RunActiveSMObject( o ) ) );
					await UniTask.WhenAll(
						os.Select( o => o._modifyler.WaitRunning() )
					);
					break;
			}
		}


		public async UniTask Enter() {
			await Load();
//			Log.Debug( _modifyler );
//			await UniTaskUtility.Yield( _activeAsyncCancel );
//			await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => !Input.GetKeyDown( KeyCode.Return ) );
			_isEnter = true;
			return;
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Creating );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Loading );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Initializing );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Creating );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Loading );
			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Initializing );

			await RunAllActiveEvents( SMTaskType.FirstWork );
			await RunAllActiveEvents( SMTaskType.Work );
		}

		public async UniTask Exit() {
			await ChangeAllActives( SMTaskType.Work, false );
			await ChangeAllActives( SMTaskType.FirstWork, false );

			await RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Finalizing );
			await RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Finalizing );

			Get( SMTaskType.Work ).ForEach( o => o.Dispose() );
			Get( SMTaskType.FirstWork ).ForEach( o => o.Dispose() );
			Get( SMTaskType.DontWork ).ForEach( o => o.Dispose() );

			_objects.ForEach( pair => _objects[pair.Key] = null );

			_isEnter = false;
		}


		async UniTask Load() {
			if ( _owner == _owner._fsm._foreverScene )	{ return; }
			TimeManager.s_instance.StartMeasure();

			var currents = _owner._scene.GetRootGameObjects().Select( go => go.transform );
			while ( !currents.IsEmpty() ) {
				var children = Enumerable.Empty<Transform>();
				foreach ( var t in currents ) {
					var bs = t.GetComponents<SMMonoBehaviour>();
					if ( !bs.IsEmpty() ) {
						new SMObject( t.gameObject, bs, null );
						await UniTaskUtility.Yield( _activeAsyncCancel );
					} else {
						foreach ( Transform child in t ) {
							children.Concat( child );
						}
					}
				}
				currents = children;
				await UniTaskUtility.Yield( _activeAsyncCancel );
			}

			Log.Debug( $"Load {_owner.GetAboutName()} : {TimeManager.s_instance.StopMeasure()}秒" );
		}
	}
}