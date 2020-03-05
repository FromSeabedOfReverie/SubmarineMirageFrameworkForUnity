//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process {
	using System;
	using System.Threading;
	using UniRx;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class TestNewProcess : IDisposable {
		public virtual TestNewProcessManager.ProcessType _type => TestNewProcessManager.ProcessType.Work;
		public virtual TestNewProcessManager.ProcessLifeSpan _lifeSpan
			=> TestNewProcessManager.ProcessLifeSpan.InScene;

		public bool _isInitialized	{ get; private set; }
		public bool _isActive		{ get; private set; }
		CancellationTokenSource _activeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _activeAsyncCancel => _activeAsyncCanceler.Token;
		CancellationTokenSource _finalizeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _finalizeAsyncCancel => _finalizeAsyncCanceler.Token;

		public MultiAsyncEvent _loadEvent		{ get; protected set; }
		public MultiAsyncEvent _initializeEvent	{ get; protected set; }
		public MultiAsyncEvent _enableEvent		{ get; protected set; }
		public MultiSubject _updateEvent		{ get; protected set; }
		public MultiAsyncEvent _disableEvent	{ get; protected set; }
		public MultiAsyncEvent _finalizeEvent	{ get; protected set; }

		protected TestNewProcess() {
			_loadEvent = new MultiAsyncEvent();

			_initializeEvent = new MultiAsyncEvent();
			_initializeEvent.AddLast( async ( cancel ) => {
				await UniTaskUtility.DontWait( cancel );
				_isInitialized = true;
			} );

			_enableEvent = new MultiAsyncEvent();
			_enableEvent.AddLast( async ( cancel ) => {
				if ( !_isInitialized ) {
					await _loadEvent.Invoke( _activeAsyncCancel );
					await _initializeEvent.Invoke( _activeAsyncCancel );
					if ( !_isInitialized )	{ StopActiveAsync(); }
				}
				// TODO : 実際は、ここで利用者呼戻しを使用したい為、上記初期化ミス処理は、管理クラスから呼ぶ
				//			と言うか、ほぼ全て管理クラスから呼び、ステート変更等で、上手くカプセル化する
				_isActive = true;
			} );

			_updateEvent = new MultiSubject();

			_disableEvent = new MultiAsyncEvent();
			_disableEvent.AddFirst( async ( cancel ) => {
				_isActive = false;
				StopActiveAsync();
				await UniTaskUtility.DontWait( cancel );
			} );

			_finalizeEvent = new MultiAsyncEvent();
			_finalizeEvent.AddLast( async ( cancel ) => {
				await UniTaskUtility.DontWait( cancel );
				Dispose();
			} );

			TestNewProcessManager.s_instance.Register( this );
		}
		public abstract void Create();
		public void StopActiveAsync() {
			_activeAsyncCanceler.Cancel();
			_activeAsyncCanceler.Dispose();
			_activeAsyncCanceler = new CancellationTokenSource();
		}
		public virtual void Dispose() {
			_activeAsyncCanceler.Cancel();
			_finalizeAsyncCanceler.Cancel();
			_activeAsyncCanceler.Dispose();
			_finalizeAsyncCanceler.Dispose();

			_loadEvent.Dispose();
			_initializeEvent.Dispose();
			_enableEvent.Dispose();
			_updateEvent.Dispose();
			_disableEvent.Dispose();
			_finalizeEvent.Dispose();
		}
		public override string ToString() {
			return this.ToDeepString();
		}
	}

	public class TestHogeProcess : TestNewProcess {
		public override void Create() {
			_loadEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( "1" ).Subscribe( _ => {
				Log.Debug( "update 1" );
			} );
			_finalizeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
	}
	public class TestHogeProcess2 : TestHogeProcess {
		public override void Create() {
			base.Create();
			_loadEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( "2" ).Subscribe( _ => {
				Log.Debug( "update 2" );
			} );
			_updateEvent.AddFirst( "0" ).Subscribe( _ => {
				Log.Debug( "update 0" );
			} );
			_updateEvent.InsertFirst( "1", "0.5" ).Subscribe( _ => {
				Log.Debug( "update 0.5" );
			} );
			_updateEvent.InsertLast( "1", "1.5" ).Subscribe( _ => {
				Log.Debug( "update 1.5" );
			} );
			_finalizeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
		~TestHogeProcess2() {
			Log.Debug( "Delete TestHogeProcess2" );
		}
	}
}