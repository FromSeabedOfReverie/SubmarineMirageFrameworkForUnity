//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System;
	using System.Linq;
	using System.Collections;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using UTask;
	using MultiEvent;
	using SMTask;
	using SMTask.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class TestSMBehaviourBody : IDisposableExtension {
		ISMBehaviour _viewBehaviour;
		public string _viewText	{ get; private set; }
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();
		public bool _isDispose => _disposables._isDispose;


		public TestSMBehaviourBody() {
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _viewBehaviour == null ) {
					_viewText = string.Empty;
					return;
				}
				_viewText = string.Join( "\n",
					$"{_viewBehaviour.GetAboutName()}(",
					$"    {nameof( _viewBehaviour._id )} : {_viewBehaviour._id}",
					$"    {nameof( _viewBehaviour._type )} : {_viewBehaviour._type}",
					$"    {nameof( _viewBehaviour._object._owner )} : "
						+ $"{_viewBehaviour._object._owner}( {_viewBehaviour._object._id} )",
					$"    {nameof( _viewBehaviour._body._ranState )} : {_viewBehaviour._body._ranState}",
					$"    {nameof( _viewBehaviour._body._isActive )} : {_viewBehaviour._body._isActive}",
					$"    {nameof( _viewBehaviour._body._isInitialActive )} : "
						+ $"{_viewBehaviour._body._isInitialActive}",
					$"    {nameof( _viewBehaviour._isInitialized )} : {_viewBehaviour._isInitialized}",
					$"    {nameof( _viewBehaviour._isActive )} : {_viewBehaviour._isActive}"
				);
				if ( _viewBehaviour is MonoBehaviourExtension ) {
					var mb = (MonoBehaviourExtension)_viewBehaviour;
					_viewText += $"\n    {nameof( mb.isActiveAndEnabled )} : {mb.isActiveAndEnabled}";
				}
				_viewText += "\n)";
			} ) );
			_disposables.AddLast( () => _viewText = string.Empty );
		}

		public void Dispose() => _disposables.Dispose();

		~TestSMBehaviourBody() => Dispose();


		ISMBehaviour CreateBehaviour( Type type, bool isGameObjectActive = true, bool isComponentEnabled = true ) {
			if ( type.IsInheritance<SMMonoBehaviour>() ) {
				var go = new GameObject( $"{type.Name}" );
				go.SetActive( isGameObjectActive );
				var mb = (SMMonoBehaviour)go.AddComponent( type );
				mb.enabled = isComponentEnabled;
				new SMObject( go, new ISMBehaviour[] { mb }, null );
				return _viewBehaviour = mb;
			}
			return _viewBehaviour = type.Create<ISMBehaviour>();
		}

		ISMBehaviour CreateBehaviour<T>( bool isGameObjectActive = true, bool isComponentEnabled = true )
			=> CreateBehaviour( typeof( T ), isGameObjectActive, isComponentEnabled );


/*
		・作成テスト
		_id、設定された？
		文章表示、ちゃんとできてる？
*/
		public async UniTask TestCreate<T>() {
			var behaviour = CreateBehaviour<T>();
			Log.Debug( $"Create : {behaviour}" );
			behaviour.Dispose();
			await UTask.DontWait();
		}

/*
		・作成テスト
		_id、設定された？
		文章表示、ちゃんとできてる？
*/
		public async UniTask TestCreateMono<T>() {
			using ( var b = (SMMonoBehaviour)CreateBehaviour<T>( true, true ) ) {
				Log.Debug( $"Create : Active, Enable\n{b}" );
			}
			using ( var b = (SMMonoBehaviour)CreateBehaviour<T>( false, true ) ) {
				Log.Debug( $"Create : InActive, Enable\n{b}" );
			}
			using ( var b = (SMMonoBehaviour)CreateBehaviour<T>( true, false ) ) {
				Log.Debug( $"Create : Active, Disable\n{b}" );
			}
			using ( var b = (SMMonoBehaviour)CreateBehaviour<T>( false, false ) ) {
				Log.Debug( $"Create : InActive, Disable\n{b}" );
			}
			await UTask.DontWait();
		}


/*
		・仕事実行時のエラーテスト
		RunStateEvent、違う型の場合、ちゃんとエラー出る？
*/
		public async UniTask TestRunErrorState<T>() {
			var behaviour = CreateBehaviour<T>();
			var errorRunStates = new SMTaskRunState[] {
				SMTaskRunState.None, SMTaskRunState.SelfInitialized, SMTaskRunState.Initialized,
				SMTaskRunState.Finalized,
			};
			foreach ( var state in errorRunStates ) {
				try						{ await RunStateSMBehaviour.RegisterAndRun( behaviour, state ); }
				catch ( Exception e )	{ Log.Error( e ); }
			}
			behaviour.Dispose();
		}


/*
		・イベントの実行テスト
		_loadEvent、_initializeEvent、_fixedUpdateEvent、_updateEvent、_lateUpdateEvent、_finalizeEvent、
		_enableEvent、_disableEvent、ちゃんと機能する？
		RunActiveEvent、初回時のみ、ちゃんと機能する？
*/
		async UniTask TestEventRun( ISMBehaviour behaviour ) {
			Log.Debug( $"request : {SMTaskRunState.Create}" );
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Create );

			Log.Debug( $"request : {SMTaskRunState.SelfInitializing}" );
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.SelfInitializing );

			Log.Debug( $"request : {SMTaskRunState.Initializing}" );
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Initializing );

			Log.Debug( $"request : {nameof( ChangeActiveSMBehaviour.RegisterAndRunInitial )}" );
			await ChangeActiveSMBehaviour.RegisterAndRunInitial( behaviour );

			Log.Debug( $"request : {SMTaskRunState.FixedUpdate}" );
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.FixedUpdate );

			Log.Debug( $"request : {SMTaskRunState.Update}" );
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Update );

			Log.Debug( $"request : {SMTaskActiveState.Disable}" );
			await ChangeActiveSMBehaviour.RegisterAndRun( behaviour, false );

			Log.Debug( $"request : {SMTaskActiveState.Enable}" );
			await ChangeActiveSMBehaviour.RegisterAndRun( behaviour, true );

			Log.Debug( $"request : {SMTaskRunState.LateUpdate}" );
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.LateUpdate );

			Log.Debug( $"request : {SMTaskRunState.Finalizing}" );
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Finalizing );
		}


/*
		・仕事型の実行テスト
		_type : DontWork、Work、FirstWork、設定通りに機能する？
*/
		public async UniTask TestTaskType<T1, T2, T3>() {
			var behaviourTypes = new Type[] { typeof( T1 ), typeof( T2 ), typeof( T3 ) };
			var behaviours = behaviourTypes.Select( t => {
				var b = CreateBehaviour( t );
				TestSMBehaviourUtility.SetEvent( b );
				return b;
			} );

			foreach( var b in behaviours ) {
				Log.Debug( $"・TestEventRun : {b._type}" );
				await TestEventRun( b );
				b.Dispose();
			}
		}


/*
		・非同期処理の停止テスト
		StopActiveAsync、機能する？
*/
		public async UniTask TestStopActiveAsync<T>() {
			var behaviour = CreateBehaviour<T>();
			TestSMBehaviourUtility.SetEvent( behaviour );
			var canceler = new UTaskCanceler();

			var stopActiveAsync = new Action<int>( waitSecond => UTask.Void( async () => {
				await UTask.Delay( canceler, waitSecond );
				Log.Debug( $"{nameof( behaviour.StopAsyncOnDisable )}" );
				behaviour.StopAsyncOnDisable();
			} ) );


			Log.Debug( $"・Run : {SMTaskRunState.Create}" );
			stopActiveAsync( 0 );
			try { await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Create ); }
			catch ( OperationCanceledException ) {}
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Create );

			Log.Debug( $"・Run : {SMTaskRunState.SelfInitializing}" );
			stopActiveAsync( 500 );
			try { await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.SelfInitializing ); }
			catch ( OperationCanceledException ) {}
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.SelfInitializing );

			Log.Debug( $"・Run : {SMTaskRunState.Initializing}" );
			stopActiveAsync( 500 );
			try { await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Initializing ); }
			catch ( OperationCanceledException ) {}
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Initializing );

			Log.Debug( $"・Run : {nameof( ChangeActiveSMBehaviour.RegisterAndRunInitial )}" );
			stopActiveAsync( 500 );
			try { await ChangeActiveSMBehaviour.RegisterAndRunInitial( behaviour ); }
			catch ( OperationCanceledException ) {}
			await ChangeActiveSMBehaviour.RegisterAndRunInitial( behaviour );

			Log.Debug( $"・Run : {SMTaskRunState.FixedUpdate}" );
			stopActiveAsync( 0 );
			try { await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.FixedUpdate ); }
			catch ( OperationCanceledException ) {}
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.FixedUpdate );

			Log.Debug( $"・Run : {SMTaskRunState.Update}" );
			stopActiveAsync( 0 );
			try { await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Update ); }
			catch ( OperationCanceledException ) {}
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Update );

			Log.Debug( $"・Run : {SMTaskActiveState.Disable}" );
			stopActiveAsync( 500 );
			try { await ChangeActiveSMBehaviour.RegisterAndRun( behaviour, false ); }
			catch ( OperationCanceledException ) {}
			await ChangeActiveSMBehaviour.RegisterAndRun( behaviour, false );

			Log.Debug( $"・Run : {SMTaskActiveState.Enable}" );
			stopActiveAsync( 500 );
			try { await ChangeActiveSMBehaviour.RegisterAndRun( behaviour, true ); }
			catch ( OperationCanceledException ) {}
			await ChangeActiveSMBehaviour.RegisterAndRun( behaviour, true );

			Log.Debug( $"・Run : {SMTaskRunState.LateUpdate}" );
			stopActiveAsync( 0 );
			try { await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.LateUpdate ); }
			catch ( OperationCanceledException ) {}
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.LateUpdate );

			Log.Debug( $"・Run : {SMTaskRunState.Finalizing}" );
			stopActiveAsync( 1500 );
			try { await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Finalizing ); }
			catch ( OperationCanceledException ) {}
			await RunStateSMBehaviour.RegisterAndRun( behaviour, SMTaskRunState.Finalizing );

			behaviour.Dispose();
			canceler.Dispose();
		}


/*
		・解放テスト
		解放される？
*/
		public async UniTask TestDispose<T>() {
			var canceler = new UTaskCanceler();

			for ( var waitSecond = 500; waitSecond < 7000; waitSecond += 1000 ) {
				var b = CreateBehaviour<T>();
				TestSMBehaviourUtility.SetEvent( b );

				UTask.Void( async () => {
					await UTask.Delay( canceler, waitSecond );
					Log.Debug( $"{nameof( b.Dispose )}" );
					b.Dispose();
				} );

				try { await TestEventRun( b ); }
				catch ( OperationCanceledException ) {}

				b.Dispose();
				canceler.Cancel();
			}

			canceler.Dispose();
		}


/*
		・削除テスト
		削除される？
*/
		public async UniTask TestDelete<T>() {
			var canceler = new UTaskCanceler();
			{
				var b = CreateBehaviour<T>();
				TestSMBehaviourUtility.SetEvent( b );
				Log.Debug( $"Create : {b}" );

				await UTask.NextFrame( canceler );
				b = null;
			}
			await UTask.Delay( canceler, 2000 );

			Log.Debug( $"Delete" );
			canceler.Dispose();
		}


/*
		・手動テスト
		イベント等の実行時の、複雑なタイミング等を、テストする
*/
		public IEnumerator TestManual<T>( bool isActive = true ) {
			var behaviour = CreateBehaviour<T>( isActive );
			TestSMBehaviourUtility.SetEvent( behaviour );
			_disposables.AddLast( TestSMBehaviourUtility.SetRunKey( behaviour ) );
			_disposables.AddLast( behaviour );
			while ( true )	{ yield return null; }
		}
	}
}