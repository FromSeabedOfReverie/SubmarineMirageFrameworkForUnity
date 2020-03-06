//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System;
	using System.Threading;
	using MultiEvent;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public abstract class BaseProcess : IDisposable {
		public virtual CoreProcessManager.ProcessType _type => CoreProcessManager.ProcessType.Work;
		public virtual CoreProcessManager.ProcessLifeSpan _lifeSpan
			=> CoreProcessManager.ProcessLifeSpan.InScene;

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

		protected BaseProcess() {
			_loadEvent = new MultiAsyncEvent();

			_initializeEvent = new MultiAsyncEvent();
			_initializeEvent.AddLast( async cancel => {
				await UniTaskUtility.DontWait( cancel );
				_isInitialized = true;
			} );

			_enableEvent = new MultiAsyncEvent();
			_enableEvent.AddLast( async cancel => {
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
			_disableEvent.AddFirst( async cancel => {
				_isActive = false;
				StopActiveAsync();
				await UniTaskUtility.DontWait( cancel );
			} );

			_finalizeEvent = new MultiAsyncEvent();
			_finalizeEvent.AddLast( async cancel => {
				await UniTaskUtility.DontWait( cancel );
//				Dispose();	// エラーが出るので、コメント
			} );

			CoreProcessManager.s_instance.Register( this );
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
}