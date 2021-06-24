//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;



	public class SMEventData : BaseSMEventData {
		Action _event	{ get; set; }



		public SMEventData( string key, Action @event ) : base( key ) {
			_event = @event;
		}

		public override void Dispose() {
			if ( _event == null ) { return; }

			_event = null;
		}



		public override async UniTask Run( SMAsyncCanceler canceler ) {
			_event.Invoke();

			await UTask.DontWait();
		}
	}
}