//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestUTask
namespace SubmarineMirage.UTask {
	using System.Threading;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class UTaskCanceler : IDisposableExtension {
		CancellationTokenSource _canceler = new CancellationTokenSource();
		public readonly MultiSubject _cancelEvent = new MultiSubject();

		UTaskCanceler _parent;
		readonly List<UTaskCanceler> _children = new List<UTaskCanceler>();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public UTaskCanceler() {
			_disposables.AddLast( () => Cancel( false ) );
			_disposables.AddLast( _cancelEvent );
			_disposables.AddLast( () => {
				if ( _parent != null ) {
					_parent._children.Remove( c => c == this );
					_parent = null;
				}
				if ( !_children.IsEmpty() ) {
					_children.ForEach( c => c._parent = null );
					_children.Clear();
				}
			} );
#if TestUTask
			_disposables.AddLast( () => Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( Dispose )}\n{this}" ) );
#endif
		}

		~UTaskCanceler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Cancel( bool isReCreate = true ) {
#if TestUTask
			Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( Cancel )}\n{this}" );
#endif
			_canceler.Cancel();
			_canceler.Dispose();
			if ( isReCreate )	{ _canceler = new CancellationTokenSource(); }
#if TestUTask
			Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( _cancelEvent.Run )}\n{this}" );
#endif
			_cancelEvent.Run();
#if TestUTask
			Log.Debug( $"{nameof( UTaskCanceler )}.{nameof( _children )}.{nameof( Cancel )}\n{this}" );
#endif
			_children.ForEach( c => c.Cancel() );
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


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CancellationToken ToToken() => _canceler.Token;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ( UniTask, CancellationTokenRegistration ) ToUniTask() => _canceler.Token.ToUniTask();

		public override string ToString() => string.Join( "\n",
			$"{nameof( UTaskCanceler )}(",
			$"    {nameof( _cancelEvent )} : {_cancelEvent}",
			$"    {nameof( _parent )} : {( _parent != null ? 1 : 0 )}",
			$"    {nameof( _children )} : {_children.Count}",
			$")"
		);
	}
}