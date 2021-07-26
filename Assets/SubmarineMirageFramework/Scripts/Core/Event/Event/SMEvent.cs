//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using Cysharp.Threading.Tasks;
	using Debug;



	public class SMEvent : BaseSMEvent {
		public new void Remove( string findKey )
			=> base.Remove( findKey );

		public new void Reverse()
			=> base.Reverse();



		public void InsertFirst( string findKey, string key, Action @event )
			=> InsertFirst( findKey, new SMEventData( key, @event ) );

		public void InsertFirst( string findKey, Action @event )
			=> InsertFirst( findKey, string.Empty, @event );


		public void InsertLast( string findKey, string key, Action @event )
			=> InsertLast( findKey, new SMEventData( key, @event ) );

		public void InsertLast( string findKey, Action @event )
			=> InsertLast( findKey, string.Empty, @event );



		public void AddFirst( string key, Action @event )
			=> AddFirst( new SMEventData( key, @event ) );

		public void AddFirst( Action @event )
			=> AddFirst( string.Empty, @event );


		public void AddLast( string key, Action @event )
			=> AddLast( new SMEventData( key, @event ) );

		public void AddLast( Action @event )
			=> AddLast( string.Empty, @event );



		public void Run()
			=> Run( null ).Forget();
	}
}