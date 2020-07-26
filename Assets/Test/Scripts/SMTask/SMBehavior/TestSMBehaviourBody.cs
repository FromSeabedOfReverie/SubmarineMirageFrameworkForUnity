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
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using MultiEvent;
	using SMTask;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class TestSMBehaviourBody : IDisposableExtension {
		public ISMBehaviour _behaviour		{ get; private set; }
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();



		public TestSMBehaviourBody( ISMBehaviour behaviour ) {
			_behaviour = behaviour;
			TestSMBehaviourUtility.SetEvent( _behaviour );
			_disposables.AddLast( _behaviour );
		}

		public void Dispose() => _disposables.Dispose();

		~TestSMBehaviourBody() => Dispose();


/*
		・作成テスト
		_id、設定された？
		文章表示、ちゃんとできてる？
*/
		public async UniTask TestCreate() {
			Log.Debug( $"Create : {_behaviour}" );
			await UTask.DontWait();
		}


/*
		・仕事実行時のエラーテスト
		RunStateEvent、違う型の場合、ちゃんとエラー出る？
*/
		public async UniTask TestRunErrorState() {
			var errorRuns = new SMTaskRanState[] {
				SMTaskRanState.None, SMTaskRanState.Created, SMTaskRanState.Loaded, SMTaskRanState.Initialized,
				SMTaskRanState.Finalized,
			};
			foreach ( var state in errorRuns ) {
				try						{ await _behaviour.RunStateEvent( state ); }
				catch ( Exception e )	{ Log.Error( e ); }
			}
		}


/*
		・イベントの実行テスト
		_loadEvent、_initializeEvent、_fixedUpdateEvent、_updateEvent、_lateUpdateEvent、_finalizeEvent、
		_enableEvent、_disableEvent、ちゃんと機能する？
		RunActiveEvent、初回時のみ、ちゃんと機能する？
*/
		public async UniTask TestEventRun( ISMBehaviour behaviour ) {
			Log.Debug( $"request : {SMTaskRanState.Creating}" );
			await behaviour.RunStateEvent( SMTaskRanState.Creating );

			Log.Debug( $"request : {SMTaskRanState.Loading}" );
			await behaviour.RunStateEvent( SMTaskRanState.Loading );

			Log.Debug( $"request : {SMTaskRanState.Initializing}" );
			await behaviour.RunStateEvent( SMTaskRanState.Initializing );

			Log.Debug( $"request : {nameof( behaviour.RunActiveEvent )}" );
			await behaviour.RunActiveEvent();

			Log.Debug( $"request : {SMTaskRanState.FixedUpdate}" );
			await behaviour.RunStateEvent( SMTaskRanState.FixedUpdate );

			Log.Debug( $"request : {SMTaskRanState.Update}" );
			await behaviour.RunStateEvent( SMTaskRanState.Update );

			Log.Debug( $"request : {nameof( behaviour.ChangeActive )}( {false} )" );
			await behaviour.ChangeActive( false );

			Log.Debug( $"request : {nameof( behaviour.ChangeActive )}( {true} )" );
			await behaviour.ChangeActive( true );

			Log.Debug( $"request : {SMTaskRanState.LateUpdate}" );
			await behaviour.RunStateEvent( SMTaskRanState.LateUpdate );

			Log.Debug( $"request : {SMTaskRanState.Finalizing}" );
			await behaviour.RunStateEvent( SMTaskRanState.Finalizing );
		}


/*
		・仕事型の実行テスト
		_type : DontWork、Work、FirstWork、設定通りに機能する？
*/
		public async UniTask TestTaskType<T1, T2, T3>() {
			var behaviourTypes = new Type[] { typeof( T1 ), typeof( T2 ), typeof( T3 ) };
			var behaviours = behaviourTypes.Select( t => {
				var b = t.Create<ISMBehaviour>();
				TestSMBehaviourUtility.SetEvent( b );
				return b;
			} );
			foreach( var b in behaviours ) {
				Log.Debug( $"・TestEventRun : {b._type}" );
				await TestEventRun( b );
			}
			behaviours.ForEach( b => b.Dispose() );
		}


/*
		・非同期処理の停止テスト
		StopActiveAsync、機能する？
*/
		public async UniTask TestStopActiveAsync() {
			Log.Debug( $"start : {nameof( TestStopActiveAsync )}" );

			var stopActiveAsync = new Action<int>( i => UTask.Void( async () => {
				await UTask.Delay( _behaviour._activeAsyncCanceler, i );
				Log.Debug( $"{nameof( _behaviour.StopActiveAsync )}" );
				_behaviour.StopActiveAsync();
			} ) );


			Log.Debug( $"・Run : {SMTaskRanState.Creating}" );
			stopActiveAsync( 0 );
			try { await _behaviour.RunStateEvent( SMTaskRanState.Creating ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunStateEvent( SMTaskRanState.Creating );

			Log.Debug( $"・Run : {SMTaskRanState.Loading}" );
			stopActiveAsync( 500 );
			try { await _behaviour.RunStateEvent( SMTaskRanState.Loading ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunStateEvent( SMTaskRanState.Loading );

			Log.Debug( $"・Run : {SMTaskRanState.Initializing}" );
			stopActiveAsync( 500 );
			try { await _behaviour.RunStateEvent( SMTaskRanState.Initializing ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunStateEvent( SMTaskRanState.Initializing );

			Log.Debug( $"・Run : {nameof( _behaviour.RunActiveEvent )}" );
			stopActiveAsync( 500 );
			try { await _behaviour.RunActiveEvent(); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunActiveEvent();

			Log.Debug( $"・Run : {SMTaskRanState.FixedUpdate}" );
			stopActiveAsync( 0 );
			try { await _behaviour.RunStateEvent( SMTaskRanState.FixedUpdate ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunStateEvent( SMTaskRanState.FixedUpdate );

			Log.Debug( $"・Run : {SMTaskRanState.Update}" );
			stopActiveAsync( 0 );
			try { await _behaviour.RunStateEvent( SMTaskRanState.Update ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunStateEvent( SMTaskRanState.Update );

			Log.Debug( $"・Run : {nameof( _behaviour.ChangeActive )}( {false} )" );
			stopActiveAsync( 500 );
			try { await _behaviour.ChangeActive( false ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.ChangeActive( false );

			Log.Debug( $"・Run : {nameof( _behaviour.ChangeActive )}( {true} )" );
			stopActiveAsync( 500 );
			try { await _behaviour.ChangeActive( true ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.ChangeActive( true );

			Log.Debug( $"・Run : {SMTaskRanState.LateUpdate}" );
			stopActiveAsync( 0 );
			try { await _behaviour.RunStateEvent( SMTaskRanState.LateUpdate ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunStateEvent( SMTaskRanState.LateUpdate );

			Log.Debug( $"・Run : {SMTaskRanState.Finalizing}" );
			stopActiveAsync( 1500 );
			try { await _behaviour.RunStateEvent( SMTaskRanState.Finalizing ); }
			catch ( OperationCanceledException ) {}
			await _behaviour.RunStateEvent( SMTaskRanState.Finalizing );


			Log.Debug( $"end : {nameof( TestStopActiveAsync )}" );
		}


/*
		・解放テスト
		解放される？
*/
		public async UniTask TestDispose<T>() {
			Log.Debug( $"start : {nameof( TestDispose )}" );

			for ( var waitSecond = 500; waitSecond < 7000; waitSecond += 1000 ) {
				var b = typeof( T ).Create<ISMBehaviour>();
				TestSMBehaviourUtility.SetEvent( b );

				UTask.Void( async () => {
					await UTask.Delay( b._activeAsyncCanceler, waitSecond );
					Log.Debug( $"{nameof( b.Dispose )}" );
					b.Dispose();
				} );

				try { await TestEventRun( b ); }
				catch ( OperationCanceledException ) {}

				b.Dispose();
			}

			Log.Debug( $"end : {nameof( TestDispose )}" );
		}


/*
		・削除テスト
		削除される？
*/
		public async UniTask TestDelete<T>() {
			{
				var b = typeof( T ).Create<ISMBehaviour>();
				TestSMBehaviourUtility.SetEvent( b );
				Log.Debug( $"Create : {b}" );

				await UTask.NextFrame( _behaviour._activeAsyncCanceler );
				b = null;
			}
			await UTask.Delay( _behaviour._activeAsyncCanceler, 2000 );

			Log.Debug( $"Delete" );
		}


/*
		・手動テスト
		イベント等の実行時の、複雑なタイミング等を、テストする
*/
		public IEnumerator TestManual() {
			_disposables.AddLast( TestSMBehaviourUtility.SetRunKey( _behaviour ) );
			while ( true )	{ yield return null; }
		}
	}
}