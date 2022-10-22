//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using KoganeUnityLib;



	public class SMDisposable : BaseSMEvent {
		public void InsertFirst( string findKey, string key, params object[] events )
			=> events.ForEach( e =>
				base.InsertFirst( findKey, new SMDisposableData( key, e ) )
			);

		public void InsertFirst( string findKey, params object[] events )
			=> InsertFirst( findKey, string.Empty, events );

		public void InsertFirst( string findKey, string key, params Action[] events )
			=> InsertFirst( findKey, key, events as object[] );

		public void InsertFirst( string findKey, params Action[] events )
			=> InsertFirst( findKey, string.Empty, events as object[] );



		public void InsertLast( string findKey, string key, params object[] events )
			=> events.ForEach( e =>
				base.InsertLast( findKey, new SMDisposableData( key, e ) )
			);

		public void InsertLast( string findKey, params object[] events )
			=> InsertLast( findKey, string.Empty, events );

		public void InsertLast( string findKey, string key, params Action[] events )
			=> InsertLast( findKey, key, events as object[] );

		public void InsertLast( string findKey, params Action[] events )
			=> InsertLast( findKey, string.Empty, events as object[] );



		public void AddFirst( string key, params object[] events )
			=> events.ForEach( e =>
				base.AddFirst( new SMDisposableData( key, e ) )
			);

		public void AddFirst( params object[] events )
			=> AddFirst( string.Empty, events );

		public void AddFirst( string key, params Action[] events )
			=> AddFirst( key, events as object[] );

		public void AddFirst( params Action[] events )
			=> AddFirst( string.Empty, events as object[] );



		public void AddLast( string key, params object[] events )
			=> events.ForEach( e =>
				base.AddLast( new SMDisposableData( key, e ) )
			);

		public void AddLast( params object[] events )
			=> AddLast( string.Empty, events );

		public void AddLast( string key, params Action[] events )
			=> AddLast( key, events as object[] );

		public void AddLast( params Action[] events )
			=> AddLast( string.Empty, events as object[] );
	}
}