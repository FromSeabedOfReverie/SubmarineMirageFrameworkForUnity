//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.UTask {
	using System.Linq;
	using System.Threading;
	using System.Collections;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public class UTaskCanceler : IDisposableExtension {
		CancellationTokenSource _canceler = new CancellationTokenSource();
		public readonly MultiSubject _cancelEvent = new MultiSubject();

		UTaskCanceler _parent;
		readonly List<UTaskCanceler> _children = new List<UTaskCanceler>();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public UTaskCanceler() {
			_disposables.AddLast( () => {
				_canceler.Cancel();
				_canceler.Dispose();
			} );
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
		}

		~UTaskCanceler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Cancel() {
			_canceler.Cancel();
			_canceler.Dispose();
			_canceler = new CancellationTokenSource();

			_cancelEvent.Run();

			_children.ForEach( c => c.Cancel() );
		}


		public UTaskCanceler CreateChild() {
			var child = new UTaskCanceler();
			child._parent = this;
			_children.Add( child );
			return child;
		}


		public CancellationToken ToToken() => _canceler.Token;

		public ( UniTask, CancellationTokenRegistration ) ToUniTask() => _canceler.Token.ToUniTask();

		public override string ToString() => string.Join( "\n",
			$"{nameof( UTaskCanceler )}(",
			$"    {nameof( _cancelEvent )} : {_cancelEvent}",
			$"    {nameof( _parent )} : {_parent != null}",
			$"    {nameof( _children )} : {_children.Count}",
			$")"
		);
	}
}