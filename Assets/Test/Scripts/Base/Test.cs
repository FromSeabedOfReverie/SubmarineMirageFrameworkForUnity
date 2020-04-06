//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Threading;
	using MultiEvent;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class Test : BaseTest {
		protected readonly MultiSubject _initializeEvent = new MultiSubject();
		protected readonly MultiSubject _finalizeEvent = new MultiSubject();
		CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		protected CancellationToken _asyncCancel => _asyncCanceler.Token;
		protected readonly MultiDisposable _disposables	= new MultiDisposable();


		protected override void Awake() {
			try {
				base.Awake();
				SetAsyncCancelerDisposable();
				_disposables.AddLast( _initializeEvent, _finalizeEvent );
				Create();
				_initializeEvent.Run();
			} catch ( Exception e ) {
				Log.Error( e );
			}
		}

		void SetAsyncCancelerDisposable() {
			_disposables.AddFirst( "_asyncCanceler", () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} );
		}


		protected override void OnDestroy() {
			try {
				_finalizeEvent.Run();
				base.OnDestroy();
			} catch ( Exception e ) {
				Log.Error( e );
			}
		}

		public override void Dispose() => _disposables.Dispose();

		~Test() => Dispose();


		protected void StopAsync() {
			_disposables.Remove( "_asyncCanceler" );
			_asyncCanceler = new CancellationTokenSource();
			SetAsyncCancelerDisposable();
		}
	}
}