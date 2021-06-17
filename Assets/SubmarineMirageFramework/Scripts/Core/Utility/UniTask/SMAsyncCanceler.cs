//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestAsyncCanceler
namespace SubmarineMirage.Utility {
	using System;
	using System.Threading;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Base;
	using Event;
	using Extension;
	using Debug;



	public class SMAsyncCanceler : SMStandardBase {
		static readonly CancellationToken DEFAULT_CANCELED_TOKEN;

		SMAsyncCanceler _previous	{ get; set; }
		SMAsyncCanceler _next		{ get; set; }

		CancellationTokenSource _canceler	{ get; set; } = new CancellationTokenSource();
		[SMShow] public readonly SMSubject _cancelEvent = new SMSubject();

		[SMShow] bool _isRawCancel		=> _canceler?.IsCancellationRequested ?? true;
		[SMShow] public bool _isCancel	=> _isDispose || _isRawCancel;
		[SMShow] new bool _isDispose	{ get; set; }



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.Add( nameof( _previous ), i => $"{ _previous?._id}" );
			_toStringer.Add( nameof( _next ), i => $"{_next?._id}" );
			_toStringer.Add( nameof( _canceler ), i => $"{_canceler?.GetHashCode()}" );
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
			SMLog.Debug( $"Create :\n{this}" );
#endif
		}



		public void Link( SMAsyncCanceler add ) {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( Link )}", $"既に解放済\n{this}" );
			}

#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Link )} : start\n{this}" );
#endif
			var last = _next;
			_next = add;
			add._previous = this;
			if ( last != null ) {
				add._next = last;
				last._previous = add;
			}
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Link )} : end\n{this}" );
#endif
		}

		public void Unlink() {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( Unlink )}", $"既に解放済\n{this}" );
			}

#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Unlink )} : start\n{this}" );
#endif
			if ( _previous != null )	{ _previous._next = _next; }
			if ( _next != null )		{ _next._previous = _previous; }
			_previous = null;
			_next = null;
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Unlink )} : end\n{this}" );
#endif
		}

		public SMAsyncCanceler CreateLinkCanceler() {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( CreateLinkCanceler )}", $"既に解放済\n{this}" );
			}

			var add = new SMAsyncCanceler();
			Link( add );
#if TestAsyncCanceler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( CreateLinkCanceler )}",
				$"this : {this}",
				$"{nameof( add )} : {add}"
			) );
#endif
			return add;
		}



		public void Cancel( bool isRecreate = true ) {
			if ( _isDispose )	{ return; }

#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Cancel )} : start\n{this}" );
#endif
			if ( _isRawCancel ) {
				if ( isRecreate )	{ Recreate(); }
			} else {
				_canceler.Cancel();
				if ( isRecreate )	{ Recreate(); }
				_cancelEvent.Run();
			}
			if ( _next != null ) {
				_next.Cancel( !_next._isRawCancel );
			}
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Cancel )} : end\n{this}" );
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



		public IEnumerable<SMAsyncCanceler> GetBrothers() {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( GetBrothers )}", $"既に解放済\n{this}" );
			}

			SMAsyncCanceler first = null;
			for ( first = this; first._previous != null; first = first._previous )	{}

			for ( var c = first; c != null; c = c._next ) {
				yield return c;
			}
		}



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CancellationToken ToToken() {
			if ( _isDispose )	{ return DEFAULT_CANCELED_TOKEN; }
			return _canceler.Token;
		}
	}
}