//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestBase {
	using System;
	using UnityEngine.TestTools;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Base;
	using Extension;
	using Utility;
	using Debug;


	public abstract class SMRawTest : BaseSMTest, ISMRawBase {
		public CompositeDisposable _disposables	{ get; private set; } = new CompositeDisposable();
		[SMShowLine] public bool _isDispose => _disposables.IsDisposed;
		protected Func<SMAsyncCanceler, UniTask> _createEvent	{ get; set; }
		protected Func<SMAsyncCanceler, UniTask> _initializeEvent	{ get; set; }
		protected readonly Subject<Unit> _finalizeEvent = new Subject<Unit>();


		protected override void Awake() {
			base.Awake();

			_disposables.Add(
				Observable.EveryUpdate()
					.Where( _ => !LogAssert.ignoreFailingMessages )
					.Subscribe( _ => LogAssert.ignoreFailingMessages = true )
			);
			_disposables.Add( () => {
				_asyncCanceler.Dispose();
				_stopwatch.Dispose();
				_finalizeEvent.OnNext( Unit.Default );
				_createEvent = null;
				_initializeEvent = null;
				_finalizeEvent.OnCompleted();
				_finalizeEvent.Dispose();
			} );
		}

		public override async UniTask Initialize() {
			Create();
			if ( _createEvent != null ) {
				await _createEvent.Invoke( _asyncCanceler );
			}
			if ( _initializeEvent != null ) {
				await _initializeEvent.Invoke( _asyncCanceler );
			}
			_isInitialized = true;
		}

		public override void Dispose() => _disposables.Dispose();
	}
}