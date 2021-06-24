//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using KoganeUnityLib;
	using Debug;



	public class SMDisposable : BaseSMEvent {
		public void InsertFirst( string findKey, string key, params object[] events )
			=> events.ForEach( e =>
				base.InsertFirst( findKey, new SMDisposableData( key, e ) )
			);

		public void InsertFirst( string findKey, params object[] events )
			=> InsertFirst( findKey, string.Empty, events );

		public void InsertFirst( string findKey, string key, Action @event )
			=> InsertFirst( findKey, key, @event as object );

		public void InsertFirst( string findKey, Action @event )
			=> InsertFirst( findKey, string.Empty, @event as object );



		public void InsertLast( string findKey, string key, params object[] events )
			=> events.ForEach( e =>
				base.InsertLast( findKey, new SMDisposableData( key, e ) )
			);

		public void InsertLast( string findKey, params object[] events )
			=> InsertLast( findKey, string.Empty, events );

		public void InsertLast( string findKey, string key, Action @event )
			=> InsertLast( findKey, key, @event as object );

		public void InsertLast( string findKey, Action @event )
			=> InsertLast( findKey, string.Empty, @event as object );



		public void AddFirst( string key, params object[] events )
			=> events.ForEach( e =>
				base.AddFirst( new SMDisposableData( key, e ) )
			);

		public void AddFirst( params object[] events )
			=> AddFirst( string.Empty, events );

		public void AddFirst( string key, Action @event )
			=> AddFirst( key, @event as object );

		public void AddFirst( Action @event )
			=> AddFirst( string.Empty, @event as object );



		public void AddLast( string key, params object[] events )
			=> events.ForEach( e =>
				base.AddLast( new SMDisposableData( key, e ) )
			);

		public void AddLast( params object[] events )
			=> AddLast( string.Empty, events );

		public void AddLast( string key, Action @event )
			=> AddLast( key, @event as object );

		public void AddLast( Action @event )
			=> AddLast( string.Empty, @event as object );
	}
}