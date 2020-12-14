//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using Cysharp.Threading.Tasks;
	using Main;
	using Base;
	using MultiEvent;
	using UTask;
	using Debug.ToString;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMStandardTest : SMBaseTest, ISMStandardBase {
		[Hide] public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();
		public bool _isDispose => _disposables._isDispose;
		[Hide] public SMToStringer _toStringer	{ get; private set; }
		[Hide] protected readonly MultiAsyncEvent _createEvent = new MultiAsyncEvent();
		[Hide] protected readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		[Hide] protected readonly MultiSubject _finalizeEvent = new MultiSubject();


		public SMStandardTest() {
			_toStringer = new SMToStringer( this );
			SetToString();
		}

		protected override async UniTask AwakeSub() {
			_disposables.AddLast( () => {
				_asyncCanceler.Dispose();
				_finalizeEvent.Run();
				_createEvent.Dispose();
				_initializeEvent.Dispose();
				_finalizeEvent.Dispose();
			} );

			Create();
			await _createEvent.Run( _asyncCanceler );

			UTask.Void( async () => {
				await UTask.WaitWhile( _asyncCanceler, () => !SubmarineMirage.s_instance._isInitialized );
				await _initializeEvent.Run( _asyncCanceler );
				_isInitialized = true;
			} );
		}

		public override void Dispose() => _disposables.Dispose();

		protected void StopAsync() => _asyncCanceler.Cancel();

		public virtual void SetToString()	{}
		public override string ToString( int indent ) => _toStringer.Run( indent );
		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}