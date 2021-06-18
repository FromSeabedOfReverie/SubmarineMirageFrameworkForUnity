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
	using Event;
	using Extension;
	using Debug;



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
			SMLog.Debug( $"Create :\n{this}" );
#endif
		}



		public void LinkLast( SMAsyncCanceler add ) {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( LinkLast )}", $"既に解放済\n{this}" );
			}
			base.LinkLast( add );
		}

		public new void Unlink() {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( Unlink )}", $"既に解放済\n{this}" );
			}
			base.Unlink();
		}

		public SMAsyncCanceler CreateChild() {
			if ( _isDispose ) {
				throw new ObjectDisposedException(
					$"{this.GetAboutName()}.{nameof( CreateChild )}", $"既に解放済\n{this}" );
			}

			var add = new SMAsyncCanceler();
			LinkLast( add );
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( CreateChild )} :\n{string.Join( "\n", GetAlls() )}" );
#endif
			return add;
		}



		public void Cancel( bool isRecreate = true ) {
			if ( _isDispose )	{ return; }

#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( Cancel )}( {isRecreate} ) : start\n{this}" );
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
			SMLog.Debug( $"{nameof( Cancel )}( {isRecreate} ) : end\n{this}" );
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