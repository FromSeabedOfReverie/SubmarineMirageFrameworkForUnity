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
	using UniRx;



	public class SMDisposableData : BaseSMEventData {
		IDisposable _event	{ get; set; }



		public SMDisposableData( string key, object @event ) : base( key ) {
#if TestEventData
			SMLog.Debug( $"{nameof( SMDisposableData )}() : \n{this}" );
#endif
			switch ( @event ) {
				case IDisposable d:	_event = d;							return;
				case Action a:		_event = Disposable.Create( a );	return;
			}

			throw new InvalidOperationException( string.Join( "\n",
				"未対応イベントを指定",
				$"{nameof( SMDisposableData )}()",
				$"{nameof( @event )} : {@event.GetType()}",
				$"対応型 : {nameof( IDisposable )}, {nameof( Action )}"
			) );
		}

		public override void Dispose() {
			if ( _event == null )	{ return; }

			try {
				_event.Dispose();

			} catch ( Exception e ) {
				// Dispose中のエラーは、無限循環防止の為、外部伝搬させない
				SMLog.Error( e );

			} finally {
				_event = null;
#if TestEventData
				SMLog.Debug( $"{nameof( SMDisposableData )}.{nameof( Dispose )} : \n{this}" );
#endif
			}
		}



		public override UniTask Run( SMAsyncCanceler canceler )
			=> throw new InvalidOperationException( string.Join( "\n",
				"未対応",
				$"{nameof( SMDisposableData )}.{nameof( Run )}"
			) );
	}
}