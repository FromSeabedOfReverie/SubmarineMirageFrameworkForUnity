//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestTaskCanceler
namespace SubmarineMirage.Task {
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using KoganeUnityLib;
	using Base;
	using MultiEvent;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMTaskCanceler : SMStandardBase {
		CancellationTokenSource _canceler = new CancellationTokenSource();
		public CancellationToken _lastCanceledToken	{ get; private set; }
		public SMMultiSubject _cancelEvent	{ get; private set; } = new SMMultiSubject();
		public bool _isCancel	=> _isDispose || _canceler.IsCancellationRequested;

		SMTaskCanceler _parent;
		public readonly List<SMTaskCanceler> _children = new List<SMTaskCanceler>();


		public SMTaskCanceler() {
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
#if TestTaskCanceler
			_disposables.AddLast( () => SMLog.Debug( $"{nameof( SMTaskCanceler )}.{nameof( Dispose )}\n{this}" ) );
#endif
		}

		void Unlink() {
			if ( _parent != null ) {
				_parent._children.Remove( c => c == this );
				_parent = null;
			}
		}


		public void SetParent( SMTaskCanceler parent ) {
			Unlink();
			_parent = parent;
			_parent._children.Add( this );
		}

		public SMTaskCanceler CreateChild() {
			var child = new SMTaskCanceler();
			child._parent = this;
			_children.Add( child );
#if TestTaskCanceler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( SMTaskCanceler )}.{nameof( CreateChild )}",
				$"this : {this}",
				$"{nameof( child )} : {child}"
			) );
#endif
			return child;
		}


		void CancelBody( bool isReCreate = true ) {
#if TestTaskCanceler
			SMLog.Debug( $"{nameof( SMTaskCanceler )}.{nameof( CancelBody )}\n{this}" );
#endif
			if ( _canceler == null )	{ return; }
			_canceler.Cancel();
			_lastCanceledToken = _canceler.Token;
			if ( isReCreate ) {
				_canceler.Dispose();
				_canceler = new CancellationTokenSource();
			}
#if TestTaskCanceler
			SMLog.Debug( $"{nameof( SMTaskCanceler )}.{nameof( _cancelEvent.Run )}\n{this}" );
#endif
			_cancelEvent.Run();
#if TestTaskCanceler
			SMLog.Debug( $"{nameof( SMTaskCanceler )}.{nameof( _children )}.{nameof( CancelBody )}\n{this}" );
#endif
			_children.ForEach( c => c.CancelBody() );
		}

		public void Cancel() => CancelBody();


		IEnumerable<SMTaskCanceler> GetChildren() {
			var currents = new Queue<SMTaskCanceler>( new [] {this} );
			while ( !currents.IsEmpty() ) {
				var canceler = currents.Dequeue();
				yield return canceler;
				canceler._children.ForEach( c => currents.Enqueue( c ) );
			}
		}

		public bool IsCanceledBy( CancellationToken token ) {
#if TestTaskCanceler
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
			var token = _canceler.Token;
#if TestTaskCanceler
			SMLog.Debug( $"{nameof( token )} : {token.GetHashCode()}" );
#endif
			return token;
		}


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _parent ), i => $"{( _parent != null ? 1 : 0 )}" );
			_toStringer.SetValue( nameof( _children ), i => $"{_children.Count}" );
			_toStringer.SetValue( nameof( _canceler ), i => $"{_canceler?.GetHashCode()}" );
			_toStringer.SetValue( nameof( _lastCanceledToken ), i => $"{_lastCanceledToken.GetHashCode()}" );
		}
	}
}