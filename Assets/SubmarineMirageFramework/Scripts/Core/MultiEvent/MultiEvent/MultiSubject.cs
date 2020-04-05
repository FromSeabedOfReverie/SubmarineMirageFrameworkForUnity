//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using UniRx;


	// TODO : コメント追加、整頓


	public class MultiSubject : BaseMultiEvent< Subject<Unit> > {
		public override void OnRemove( Subject<Unit> function ) {
			function.OnCompleted();
			function.Dispose();
		}


		Subject<Unit> Insert( string findKey, EventAddType type, string key ) {
			var subject = new Subject<Unit>();
			RegisterEventModifyler( new InsertEventModifyData< Subject<Unit> >(
				findKey, type, key, subject
			) );
			return subject;
		}

		public Subject<Unit> InsertFirst( string findKey, string key ) {
			return Insert( findKey, EventAddType.First, key );
		}

		public Subject<Unit> InsertFirst( string findKey ) {
			return Insert( findKey, EventAddType.First, string.Empty );
		}

		public Subject<Unit> InsertLast( string findKey, string key ) {
			return Insert( findKey, EventAddType.Last, key );
		}

		public Subject<Unit> InsertLast( string findKey ) {
			return Insert( findKey, EventAddType.Last, string.Empty );
		}


		Subject<Unit> Add( EventAddType type, string key ) {
			var subject = new Subject<Unit>();
			RegisterEventModifyler( new AddEventModifyData< Subject<Unit> >(
				type, key, subject
			) );
			return subject;
		}

		public Subject<Unit> AddFirst( string key ) {
			return Add( EventAddType.First, key );
		}

		public Subject<Unit> AddFirst() {
			return Add( EventAddType.First, string.Empty );
		}

		public Subject<Unit> AddLast( string key ) {
			return Add( EventAddType.Last, key );
		}

		public Subject<Unit> AddLast() {
			return Add( EventAddType.Last, string.Empty );
		}


		public void Run() {
			CheckDisposeError();
			_isInvoking.Value = true;
			_events.ForEach( pair => pair.Value.OnNext( Unit.Default ) );
			_isInvoking.Value = false;
		}
	}
}