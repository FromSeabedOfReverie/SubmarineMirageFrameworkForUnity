//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.MultiEvent {
	using UniRx;


	// TODO : コメント追加、整頓


	public class MultiSubject : BaseMultiEvent< Subject<Unit> > {
		public MultiSubject() {
			_onRemoveEvent.Subscribe( e => {
				e?.OnCompleted();
				e?.Dispose();
			} );
		}


		Subject<Unit> Insert( string findKey, AddType type, string key ) {
			var subject = new Subject<Unit>();
			Insert( findKey, type, key, subject );
			return subject;
		}

		public Subject<Unit> InsertFirst( string findKey, string key ) {
			return Insert( findKey, AddType.First, key );
		}

		public Subject<Unit> InsertFirst( string findKey ) {
			return Insert( findKey, AddType.First, string.Empty );
		}

		public Subject<Unit> InsertLast( string findKey, string key ) {
			return Insert( findKey, AddType.Last, key );
		}

		public Subject<Unit> InsertLast( string findKey ) {
			return Insert( findKey, AddType.Last, string.Empty );
		}


		Subject<Unit> Add( AddType type, string key ) {
			var subject = new Subject<Unit>();
			Add( type, key, subject );
			return subject;
		}

		public Subject<Unit> AddFirst( string key ) {
			return Add( AddType.First, key );
		}

		public Subject<Unit> AddFirst() {
			return Add( AddType.First, string.Empty );
		}

		public Subject<Unit> AddLast( string key ) {
			return Add( AddType.Last, key );
		}

		public Subject<Unit> AddLast() {
			return Add( AddType.Last, string.Empty );
		}


		public void Invoke() {
			_isInvoking.Value = true;
			foreach ( var pair in _events ) {
				pair.Value?.OnNext( Unit.Default );
			}
			_isInvoking.Value = false;
		}
	}
}