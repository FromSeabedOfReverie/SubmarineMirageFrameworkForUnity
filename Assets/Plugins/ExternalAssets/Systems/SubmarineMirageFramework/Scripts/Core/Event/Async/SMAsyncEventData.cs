//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestEventData
namespace SubmarineMirage {
	using System;
	using Cysharp.Threading.Tasks;



	public class SMAsyncEventData : BaseSMEventData {
		Func<SMAsyncCanceler, UniTask> _event	{ get; set; }



		public SMAsyncEventData( string key, Func<SMAsyncCanceler, UniTask> @event ) : base( key ) {
			_event = @event;
#if TestEventData
			SMLog.Debug( $"{nameof( SMAsyncEventData )}() : \n{this}" );
#endif
		}

		public override void Dispose() {
			if ( _event == null )	{ return; }

			_event = null;
#if TestEventData
			SMLog.Debug( $"{nameof( SMAsyncEventData )}.{nameof( Dispose )} : \n{this}" );
#endif
		}



		public override UniTask Run( SMAsyncCanceler canceler )
			=> _event.Invoke( canceler );
	}
}