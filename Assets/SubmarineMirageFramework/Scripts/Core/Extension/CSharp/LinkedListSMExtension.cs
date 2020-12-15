//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestLinkedList
namespace SubmarineMirage.Extension {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Debug;



	// TODO : コメント追加、整頓



	public static class LinkedListSMExtension {
		public static void Enqueue<T>( this LinkedList<T> self, T data )
			=> self.AddLast( data );

		public static T Dequeue<T>( this LinkedList<T> self ) {
			var result = self.First.Value;
			self.RemoveFirst();
			return result;
		}


		public static void Push<T>( this LinkedList<T> self, T data )
			=> self.AddFirst( data );

		public static T Pop<T>( this LinkedList<T> self ) {
			var result = self.First.Value;
			self.RemoveFirst();
			return result;
		}


		public static IEnumerable< LinkedListNode<T> > GetNodes<T>( this LinkedList<T> self,
																	bool isReverse = false
		) {
			if ( isReverse ) {
				for ( var n = self.Last; n != null; n = n.Previous ) {
#if TestLinkedList
					SMLog.Debug( n.Value );
#endif
					yield return n;
				}
			} else {
				for ( var n = self.First; n != null; n = n.Next ) {
#if TestLinkedList
					SMLog.Debug( n.Value );
#endif
					yield return n;
				}
			}
		}


		public static LinkedListNode<T> FindNode<T>( this LinkedList<T> self, Predicate<T> findEvent,
														bool isFromLast = false, bool isFindLast = false
		) {
			var nodes = self.GetNodes( isFromLast );
			if ( isFindLast )	{ return nodes.LastOrDefault( n => findEvent( n.Value ) ); }
			else				{ return nodes.FirstOrDefault( n => findEvent( n.Value ) ); }
		}


		public static void AddBefore<T>( this LinkedList<T> self, T data, Predicate<T> findEvent,
											Action notFoundEvent = null, bool isFromLast = false,
											bool isFindLast = false
		) {
			var node = self.FindNode( findEvent, isFromLast, isFindLast );
			if ( node != null ) {
				self.AddBefore( node, data );
			} else {
				notFoundEvent?.Invoke();
			}
		}

		public static void AddAfter<T>( this LinkedList<T> self, T data, Predicate<T> findEvent,
											Action notFoundEvent = null, bool isFromLast = false,
											bool isFindLast = false
		) {
			var node = self.FindNode( findEvent, isFromLast, isFindLast );
			if ( node != null ) {
				self.AddAfter( node, data );
			} else {
				notFoundEvent?.Invoke();
			}
		}


		public static void RemoveAll<T>( this LinkedList<T> self, Predicate<T> findEvent,
											Action<T> removeEvent = null
		) {
			self
				.GetNodes()
				.ToArray()
				.Where( n => findEvent( n.Value ) )
				.ForEach( n => {
					removeEvent?.Invoke( n.Value );
					self.Remove( n );
				} );
		}
	}
}