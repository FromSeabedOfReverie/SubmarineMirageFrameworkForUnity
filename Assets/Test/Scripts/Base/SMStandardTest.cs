//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestBase {
	using Cysharp.Threading.Tasks;
	using Base;
	using MultiEvent;
	using Debug;
	using Debug.ToString;


	// TODO : コメント追加、整頓


	public abstract class SMStandardTest : BaseSMTest, ISMStandardBase {
		[SMHide] public SMMultiDisposable _disposables	{ get; private set; } = new SMMultiDisposable();
		public bool _isDispose => _disposables._isDispose;
		[SMHide] public SMToStringer _toStringer	{ get; private set; }
		[SMHide] protected readonly SMMultiAsyncEvent _createEvent = new SMMultiAsyncEvent();
		[SMHide] protected readonly SMMultiAsyncEvent _initializeEvent = new SMMultiAsyncEvent();
		[SMHide] protected readonly SMMultiSubject _finalizeEvent = new SMMultiSubject();


		protected override void Awake() {
			base.Awake();

			_toStringer = new SMToStringer( this );
			SetToString();

			_disposables.AddLast( () => {
				_asyncCanceler.Dispose();
				_finalizeEvent.Run();
				_toStringer.Dispose();
				_createEvent.Dispose();
				_initializeEvent.Dispose();
				_finalizeEvent.Dispose();
			} );
		}

		public override async UniTask Initialize() {
			Create();
			await _createEvent.Run( _asyncCanceler );
			await _initializeEvent.Run( _asyncCanceler );
			_isInitialized = true;
		}

		public override void Dispose() => _disposables.Dispose();


		public virtual void SetToString()	{}
		public override string ToString( int indent ) => _toStringer.Run( indent );
		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}