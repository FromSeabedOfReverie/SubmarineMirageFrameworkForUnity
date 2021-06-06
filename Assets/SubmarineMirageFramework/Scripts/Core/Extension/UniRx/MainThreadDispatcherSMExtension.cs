//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UniRx;
	using Event;
	using Service;


	public class MainThreadDispatcherSMExtension : MonoBehaviourSMExtension, ISMService {
		readonly SMDisposable _disposables = new SMDisposable();
		public readonly Subject<Unit> _onGUIEvent = new Subject<Unit>();


		protected override void Awake() {
			base.Awake();

			_disposables.AddLast( () => {
				_onGUIEvent.OnCompleted();
				_onGUIEvent.Dispose();
				gameObject.Destroy();
			} );
		}

		public override void Dispose() => _disposables.Dispose();


		void OnGUI() {
			if ( _disposables._isDispose )	{ return; }
			_onGUIEvent.OnNext( Unit.Default );
		}
	}
}