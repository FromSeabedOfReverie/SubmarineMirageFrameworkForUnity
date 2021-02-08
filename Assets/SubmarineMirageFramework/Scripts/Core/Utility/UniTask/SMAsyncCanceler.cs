//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestAsyncCanceler
namespace SubmarineMirage.Utility {
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using KoganeUnityLib;
	using SubmarineMirage.Base;
	using Event;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMAsyncCanceler : SMStandardBase {
		static readonly CancellationToken s_defaultCanceledToken;
		CancellationTokenSource _canceler	{ get; set; } = new CancellationTokenSource();
		public CancellationToken _lastCanceledToken	{ get; private set; }
		[SMShow] public SMSubject _cancelEvent	{ get; private set; } = new SMSubject();
		[SMShow] public bool _isCancel	=> _isDispose || _canceler.IsCancellationRequested;

		SMAsyncCanceler _parent	{ get; set; }
		[SMShow] public readonly LinkedList<SMAsyncCanceler> _children = new LinkedList<SMAsyncCanceler>();



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.Add( nameof( _parent ), i => $"{( _parent != null ? 1 : 0 )}" );
			_toStringer.SetValue( nameof( _children ), i => $"{_children.Count}" );
			_toStringer.Add( nameof( _canceler ), i => $"{_canceler?.GetHashCode()}" );
			_toStringer.Add( nameof( _lastCanceledToken ), i => $"{_lastCanceledToken.GetHashCode()}" );
		}
#endregion


		static SMAsyncCanceler() {
			var c = new CancellationTokenSource();
			s_defaultCanceledToken = c.Token;
			c.Cancel();
			c.Dispose();
		}

		public SMAsyncCanceler() {
			_disposables.AddLast( () => {
				CancelBody( false );
				_canceler.Dispose();
				_canceler = null;
				_cancelEvent.Dispose();
				_cancelEvent = null;
				Unlink();
				if ( !_children.IsEmpty() ) {
					_children.ForEach( c => c._parent = null );
					_children.Clear();
				}
			} );
#if TestAsyncCanceler
			_disposables.AddLast( () => SMLog.Debug( $"{nameof( SMAsyncCanceler )}.{nameof( Dispose )}\n{this}" ) );
#endif
		}

		void Unlink() {
			if ( _parent != null ) {
				_parent._children.Remove( this );
				_parent = null;
			}
		}


		public void SetParent( SMAsyncCanceler parent ) {
			Unlink();
			_parent = parent;
			_parent._children.AddLast( this );
		}

		public SMAsyncCanceler CreateChild() {
			var child = new SMAsyncCanceler();
			child._parent = this;
			_children.AddLast( child );
#if TestAsyncCanceler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( SMAsyncCanceler )}.{nameof( CreateChild )}",
				$"this : {this}",
				$"{nameof( child )} : {child}"
			) );
#endif
			return child;
		}


		void CancelBody( bool isReCreate = true ) {
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( SMAsyncCanceler )}.{nameof( CancelBody )}\n{this}" );
#endif
			if ( _canceler == null )	{ return; }
			_canceler.Cancel();
			_lastCanceledToken = _canceler.Token;
			if ( isReCreate ) {
				_canceler.Dispose();
				_canceler = new CancellationTokenSource();
			}
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( SMAsyncCanceler )}.{nameof( _cancelEvent.Run )}\n{this}" );
#endif
			_cancelEvent.Run();
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( SMAsyncCanceler )}.{nameof( _children )}.{nameof( CancelBody )}\n{this}" );
#endif
			_children.ForEach( c => c.CancelBody() );
		}

		public void Cancel() => CancelBody();


		IEnumerable<SMAsyncCanceler> GetChildren() {
			var currents = new Queue<SMAsyncCanceler>( new [] {this} );
			while ( !currents.IsEmpty() ) {
				var canceler = currents.Dequeue();
				yield return canceler;
				canceler._children.ForEach( c => currents.Enqueue( c ) );
			}
		}

		public bool IsCanceledBy( CancellationToken token ) {
#if TestAsyncCanceler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( token )} : {token.GetHashCode()}",
				$"{nameof( GetChildren )} last : "
					+ $"{string.Join( ",", GetChildren().Select( c => c._lastCanceledToken.GetHashCode() ) )}",
				$"== : {GetChildren().Any( c => c._lastCanceledToken == token )}"
			) );
#endif
			return GetChildren().Any( c => c._lastCanceledToken == token );
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CancellationToken ToToken() {
			var token = _canceler?.Token ?? s_defaultCanceledToken;
#if TestAsyncCanceler
			SMLog.Debug( $"{nameof( token )} : {token.GetHashCode()}" );
#endif
			return token;
		}
	}
}