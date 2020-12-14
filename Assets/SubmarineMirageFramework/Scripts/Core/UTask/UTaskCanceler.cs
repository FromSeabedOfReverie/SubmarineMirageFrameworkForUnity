//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestUTask
namespace SubmarineMirage.UTask {
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using KoganeUnityLib;
	using Base;
	using MultiEvent;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class UTaskCanceler : SMStandardBase {
		CancellationTokenSource _canceler = new CancellationTokenSource();
		public CancellationToken _lastCanceledToken	{ get; private set; }
		public MultiSubject _cancelEvent	{ get; private set; } = new MultiSubject();
		public bool _isCancel	=> _isDispose || _canceler.IsCancellationRequested;

		UTaskCanceler _parent;
		public readonly List<UTaskCanceler> _children = new List<UTaskCanceler>();


		public UTaskCanceler() {
			_disposables.AddLast( () => {
				CancelBody( false );
				_canceler.Dispose();
				_canceler = null;
				_cancelEvent.Dispose();
				_cancelEvent = null;
				UnLink();
				if ( !_children.IsEmpty() ) {
					_children.ForEach( c => c._parent = null );
					_children.Clear();
				}
			} );
#if TestUTask
			_disposables.AddLast( () => Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( Dispose )}\n{this}" ) );
#endif
		}

		void UnLink() {
			if ( _parent != null ) {
				_parent._children.Remove( c => c == this );
				_parent = null;
			}
		}


		public void SetParent( UTaskCanceler parent ) {
			UnLink();
			_parent = parent;
			_parent._children.Add( this );
		}

		public UTaskCanceler CreateChild() {
			var child = new UTaskCanceler();
			child._parent = this;
			_children.Add( child );
#if TestUTask
			Log.Debug( string.Join( "\n",
				$"{nameof( UTaskCanceler )}.{nameof( CreateChild )}",
				$"this : {this}",
				$"{nameof( child )} : {child}"
			) );
#endif
			return child;
		}


		void CancelBody( bool isReCreate = true ) {
#if TestUTask
			Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( CancelBody )}\n{this}" );
#endif
			if ( _canceler == null )	{ return; }
			_canceler.Cancel();
			_lastCanceledToken = _canceler.Token;
			if ( isReCreate ) {
				_canceler.Dispose();
				_canceler = new CancellationTokenSource();
			}
#if TestUTask
			Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( _cancelEvent.Run )}\n{this}" );
#endif
			_cancelEvent.Run();
#if TestUTask
			Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( _children )}.{nameof( CancelBody )}\n{this}" );
#endif
			_children.ForEach( c => c.CancelBody() );
		}

		public void Cancel() => CancelBody();


		IEnumerable<UTaskCanceler> GetChildren() {
			var currents = new Queue<UTaskCanceler>( new [] {this} );
			while ( !currents.IsEmpty() ) {
				var canceler = currents.Dequeue();
				yield return canceler;
				canceler._children.ForEach( c => currents.Enqueue( c ) );
			}
		}

		public bool IsCanceledBy( CancellationToken token ) {
#if TestUTask
			Log.Debug( string.Join( "\n",
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
#if TestUTask
			Log.Debug( $"{nameof( token )} : {token.GetHashCode()}" );
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