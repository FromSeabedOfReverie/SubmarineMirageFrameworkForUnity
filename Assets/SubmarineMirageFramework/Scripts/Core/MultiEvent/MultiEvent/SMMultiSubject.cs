//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using UniRx;
	using KoganeUnityLib;
	using Extension;



	// TODO : コメント追加、整頓



	public class SMMultiSubject : BaseSMMultiEvent< Subject<Unit> > {
		public override void OnRemove( Subject<Unit> function ) {
			function.OnCompleted();
			function.Dispose();
		}


		Subject<Unit> Insert( string findKey, SMEventAddType type, string key ) {
			var subject = new Subject<Unit>();
			Register( new InsertSMEvent< Subject<Unit> >( findKey, type, key, subject ) );
			return subject;
		}

		public Subject<Unit> InsertFirst( string findKey, string key )
			=> Insert( findKey, SMEventAddType.First, key );

		public Subject<Unit> InsertFirst( string findKey )
			=> Insert( findKey, SMEventAddType.First, string.Empty );

		public Subject<Unit> InsertLast( string findKey, string key )
			=> Insert( findKey, SMEventAddType.Last, key );

		public Subject<Unit> InsertLast( string findKey )
			=> Insert( findKey, SMEventAddType.Last, string.Empty );


		Subject<Unit> Add( SMEventAddType type, string key ) {
			var subject = new Subject<Unit>();
			Register( new AddSMEvent< Subject<Unit> >( type, key, subject ) );
			return subject;
		}

		public Subject<Unit> AddFirst( string key )
			=> Add( SMEventAddType.First, key );

		public Subject<Unit> AddFirst()
			=> Add( SMEventAddType.First, string.Empty );

		public Subject<Unit> AddLast( string key )
			=> Add( SMEventAddType.Last, key );

		public Subject<Unit> AddLast()
			=> Add( SMEventAddType.Last, string.Empty );


		public void Run() {
			CheckDisposeError();

			var temp = _events.Copy();
			temp.ForEach( pair => pair.Value.OnNext( Unit.Default ) );
		}
	}
}