//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using Cysharp.Threading.Tasks;
	using UniRx;
	using Debug;



	public class SMSubject : BaseSMEvent {
		public new void Remove( string findKey )
			=> base.Remove( findKey );

		public new void Reverse()
			=> base.Reverse();



		public Subject<Unit> InsertFirst( string findKey, string key = "" ) {
			var data = new SMSubjectData( key );
			InsertFirst( findKey, data );
			return data._event;
		}

		public Subject<Unit> InsertLast( string findKey, string key = "" ) {
			var data = new SMSubjectData( key );
			InsertLast( findKey, data );
			return data._event;
		}



		public Subject<Unit> AddFirst( string key = "" ) {
			var data = new SMSubjectData( key );
			AddFirst( data );
			return data._event;
		}

		public Subject<Unit> AddLast( string key = "" ) {
			var data = new SMSubjectData( key );
			AddLast( data );
			return data._event;
		}



		public void Run()
			=> Run( null ).Forget();
	}
}