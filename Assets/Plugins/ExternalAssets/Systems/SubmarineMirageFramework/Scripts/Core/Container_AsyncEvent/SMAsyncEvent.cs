//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using Cysharp.Threading.Tasks;



	public class SMAsyncEvent : BaseSMEvent {
		public new void Remove( string findKey )
			=> base.Remove( findKey );

		public new void Reverse()
			=> base.Reverse();



		public void InsertFirst( string findKey, string key, Func<SMAsyncCanceler, UniTask> @event )
			=> InsertFirst( findKey, new SMAsyncEventData( key, @event ) );

		public void InsertFirst( string findKey, Func<SMAsyncCanceler, UniTask> @event )
			=> InsertFirst( findKey, string.Empty, @event );


		public void InsertLast( string findKey, string key, Func<SMAsyncCanceler, UniTask> @event )
			=> InsertLast( findKey, new SMAsyncEventData( key, @event ) );

		public void InsertLast( string findKey, Func<SMAsyncCanceler, UniTask> @event )
			=> InsertLast( findKey, string.Empty, @event );



		public void AddFirst( string key, Func<SMAsyncCanceler, UniTask> @event )
			=> AddFirst( new SMAsyncEventData( key, @event ) );

		public void AddFirst( Func<SMAsyncCanceler, UniTask> @event )
			=> AddFirst( string.Empty, @event );


		public void AddLast( string key, Func<SMAsyncCanceler, UniTask> @event )
			=> AddLast( new SMAsyncEventData( key, @event ) );

		public void AddLast( Func<SMAsyncCanceler, UniTask> @event )
			=> AddLast( string.Empty, @event );



		public new UniTask Run( SMAsyncCanceler canceler )
			=> base.Run( canceler );
	}
}