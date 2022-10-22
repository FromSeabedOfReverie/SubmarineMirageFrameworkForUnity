//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestAsyncCanceler
namespace SubmarineMirageFramework {
	using System.Threading;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;



	public class SMAsyncCanceler : SMLinkNode {
		static readonly CancellationToken DEFAULT_CANCELED_TOKEN;

		[SMShowLine] new SMAsyncCanceler _previous {
			get => base._previous as SMAsyncCanceler;
			set => base._previous = value;
		}
		[SMShowLine] new SMAsyncCanceler _next {
			get => base._next as SMAsyncCanceler;
			set => base._next = value;
		}

		[SMShow] CancellationTokenSource _canceler	{ get; set; } = new CancellationTokenSource();
		[SMShow] public readonly SMSubject _cancelEvent = new SMSubject();

		[SMShow] bool _isRawCancel		=> _canceler?.IsCancellationRequested ?? true;
		[SMShow] public bool _isCancel	=> _isDispose || _isRawCancel;
		[SMShow] new bool _isDispose	{ get; set; }



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _canceler ), i => $"{_canceler?.GetHashCode()}" );
		}
#endregion



		static SMAsyncCanceler() {
			var c = new CancellationTokenSource();
			DEFAULT_CANCELED_TOKEN = c.Token;
			c.Cancel();
			c.Dispose();
		}

		public SMAsyncCanceler() {
			_disposables.AddFirst( () => {
#if TestAsyncCanceler
				SMLog.Debug( $"{nameof( Dispose )} : start\n{this}" );
#endif
				Cancel( false );
				_canceler.Dispose();
				_canceler = null;
				_cancelEvent.Dispose();
				Unlink();
				_isDispose = true;
#if TestAsyncCanceler
				SMLog.Debug( $"{nameof( Dispose )} : end\n{this}" );
#endif
			} );
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( SMAsyncCanceler )}() : \n{this}" );
#endif
		}



		public void LinkLast( SMAsyncCanceler add ) {
			if ( _isDispose )	{ CheckDisposeError( $"{nameof( LinkLast )}( {add._id} )" ); }

			base.LinkLast( add );
		}

		public new void Unlink() {
			if ( _isDispose )	{ CheckDisposeError( nameof( Unlink ) ); }

			base.Unlink();
		}

		public SMAsyncCanceler CreateChild() {
			if ( _isDispose )	{ CheckDisposeError( nameof( CreateChild ) ); }

			var add = new SMAsyncCanceler();
			LinkLast( add );
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( CreateChild )} : \n{string.Join( "\n", GetAlls() )}" );
#endif
			return add;
		}



		public void Cancel( bool isRecreateAfterCancel = true ) {
			if ( _isDispose )	{ return; }

#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Cancel )}( {isRecreateAfterCancel} ) : start\n{this}" );
#endif
			if ( !_isRawCancel ) {
				_canceler.Cancel();
				if ( isRecreateAfterCancel )	{ Recreate(); }
				if ( !_cancelEvent._isDispose )	{ _cancelEvent.Run(); }
			}
			_next?.Cancel();
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Cancel )}( {isRecreateAfterCancel} ) : end\n{this}" );
#endif
		}

		public void Recreate() {
			if ( _isDispose )		{ return; }
			if ( !_isRawCancel )	{ return; }

#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Recreate )} : start\n{this}" );
#endif
			_canceler.Dispose();
			_canceler = new CancellationTokenSource();
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Recreate )} : end\n{this}" );
#endif
		}



		public new IEnumerable<SMLinkNode> GetAlls()
			=> base.GetAlls();



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CancellationToken ToToken() {
			if ( _isDispose )	{ return DEFAULT_CANCELED_TOKEN; }
			return _canceler.Token;
		}
	}
}