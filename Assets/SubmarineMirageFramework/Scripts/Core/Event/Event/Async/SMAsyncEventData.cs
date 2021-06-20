//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using Utility;
	using Debug;



	public class SMAsyncEventData : BaseSMEventData {
		public Func<SMAsyncCanceler, UniTask> _event { get; private set; }
		public SMAsyncCanceler _canceler { private get; set; }



		public SMAsyncEventData( string key = "" ) : base( key ) {
		}

		public override void Dispose() {
			if ( _event == null )	{ return; }

			_event = null;
		}



		public override async UniTask Run() {
			await _event.Invoke( _canceler );
		}
	}
}