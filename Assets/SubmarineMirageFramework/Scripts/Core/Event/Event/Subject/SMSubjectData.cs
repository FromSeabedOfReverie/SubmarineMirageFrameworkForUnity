//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using Cysharp.Threading.Tasks;
	using UniRx;
	using Utility;
	using Debug;



	public class SMSubjectData : BaseSMEventData {
		public Subject<Unit> _event { get; private set; }



		public SMSubjectData( string key = "" ) : base( key ) {
			_event = new Subject<Unit>();
		}

		public override void Dispose() {
			if ( _event == null )	{ return; }

			_event.OnCompleted();
			_event.Dispose();
			_event = null;
		}



		public override async UniTask Run( SMAsyncCanceler canceler ) {
			_event.OnNext( Unit.Default );

			await UTask.DontWait();
		}
	}
}