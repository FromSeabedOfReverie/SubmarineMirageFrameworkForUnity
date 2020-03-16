//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using System;
	using UniRx;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class MultiDisposable : BaseMultiEvent<IDisposable>, IDisposable {
		public new CompositeDisposable _disposables	{ get; private set; }
		protected override bool _isDispose => _disposables.IsDisposed;


		protected override void SetDisposables() {
			_disposables = new CompositeDisposable();
			_disposables.Add( _isInvoking );
			_disposables.Add(
				Disposable.Create( () => {
					foreach ( var pair in _events ) {
						OnRemove( pair.Value );
					}
					_events.Clear();
					_removeKeys.Clear();
					Log.Debug( $"Dispose {this.GetAboutName()} {_ownerName}" );
				} )
			);
		}

		public override void Dispose() => _disposables.Dispose();

		protected override void OnRemove( IDisposable function ) => function.Dispose();


		public void InsertFirst( string findKey, string key, Action function ) {
			Insert( findKey, AddType.First, key, Disposable.Create( function ) );
		}

		public void InsertFirst( string findKey, Action function ) {
			Insert( findKey, AddType.First, string.Empty, Disposable.Create( function ) );
		}

		public void InsertLast( string findKey, string key, Action function ) {
			Insert( findKey, AddType.Last, key, Disposable.Create( function ) );
		}

		public void InsertLast( string findKey, Action function ) {
			Insert( findKey, AddType.Last, string.Empty, Disposable.Create( function ) );
		}


		public void AddFirst( string key, Action function ) {
			Add( AddType.First, key, Disposable.Create( function ) );
		}

		public void AddFirst( Action function ) {
			Add( AddType.First, string.Empty, Disposable.Create( function ) );
		}

		public void AddLast( string key, Action function ) {
			Add( AddType.Last, key, Disposable.Create( function ) );
		}

		public void AddLast( Action function ) {
			Add( AddType.Last, string.Empty, Disposable.Create( function ) );
		}
	}
}