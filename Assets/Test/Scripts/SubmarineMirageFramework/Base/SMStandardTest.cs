//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using UnityEngine.TestTools;
	using Cysharp.Threading.Tasks;
	using UniRx;



	public abstract class SMStandardTest : BaseSMTest, ISMStandardBase {
		public SMDisposable _disposables	{ get; private set; } = new SMDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		public SMToStringer _toStringer	{ get; private set; }
		protected readonly SMAsyncEvent _createEvent = new SMAsyncEvent();
		protected readonly SMAsyncEvent _initializeEvent = new SMAsyncEvent();
		protected readonly SMSubject _finalizeEvent = new SMSubject();


		protected override void Awake() {
			base.Awake();

			_toStringer = new SMToStringer( this );
			SetToString();

			Observable.EveryUpdate()
				.Where( _ => !LogAssert.ignoreFailingMessages )
				.Subscribe( _ => LogAssert.ignoreFailingMessages = true )
				.AddFirst( this );

			_disposables.AddFirst( () => {
				_asyncCanceler.Dispose();
				_stopwatch.Dispose();
				_finalizeEvent.Run();
				_toStringer.Dispose();
				_createEvent.Dispose();
				_initializeEvent.Dispose();
				_finalizeEvent.Dispose();
			} );
		}

		public override async UniTask Initialize() {
			// LogAssert.ignoreFailingMessagesを有効にする為、1フレーム待機
			await UTask.NextFrame( _asyncCanceler );

			Create();
			await _createEvent.Run( _asyncCanceler );
			await _initializeEvent.Run( _asyncCanceler );
			_isInitialized = true;
		}

		public override void Dispose() => _disposables.Dispose();


		public virtual void SetToString() {}

		public override string ToString( int indent, bool isUseHeadIndent = true )
			=> _toStringer.Run( indent, isUseHeadIndent );

		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}