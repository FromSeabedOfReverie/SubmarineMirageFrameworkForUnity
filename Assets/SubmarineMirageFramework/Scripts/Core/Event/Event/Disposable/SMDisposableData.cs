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



	public class SMDisposableData : BaseSMEventData {
		IDisposable _event	{ get; set; }



		public SMDisposableData( string key, object @event ) : base( key ) {
			switch ( @event ) {
				case IDisposable d:	_event = d;							return;
				case Action a:		_event = Disposable.Create( a );	return;
			}

			throw new InvalidOperationException( string.Join( "\n",
				$"{nameof( SMDisposableData )}() : 未対応イベントを指定",
				$"{nameof( @event )} : {@event.GetType()}",
				$"対応型 : {nameof( IDisposable )}, {nameof( Action )}"
			) );
		}

		public override void Dispose() {
			if ( _event == null )	{ return; }

			_event.Dispose();
			_event = null;
		}



		public override UniTask Run( SMAsyncCanceler canceler )
			=> throw new InvalidOperationException( $"{nameof( SMDisposableData )}.{nameof( Run )} : 未対応" );
	}
}