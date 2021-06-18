//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestLinkNode
namespace SubmarineMirage.Utility {
	using System.Collections.Generic;
	using Base;
	using Debug;



	public abstract class SMLinkNode : SMStandardBase {
		[SMShowLine] protected SMLinkNode _previous	{ get; set; }
		[SMShowLine] protected SMLinkNode _next		{ get; set; }



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _previous ),	i => _toStringer.DefaultLineValue( _previous ) );
			_toStringer.SetValue( nameof( _next ),		i => _toStringer.DefaultLineValue( _next ) );

			_toStringer.SetLineValue( nameof( _previous ),	() => $"↑{_previous?._id}" );
			_toStringer.SetLineValue( nameof( _next ),		() => $"↓{_next?._id}" );
		}
#endregion



		protected void Link( SMLinkNode add ) {
#if TestLinkNode
			SMLog.Debug( $"{nameof( Link )} : start\n{string.Join( "\n", GetAlls() )}" );
#endif
			var last = _next;
			_next = add;
			add._previous = this;
			if ( last != null ) {
				add._next = last;
				last._previous = add;
			}
#if TestLinkNode
			SMLog.Debug( $"{nameof( Link )} : end\n{string.Join( "\n", GetAlls() )}" );
#endif
		}

		protected void LinkLast( SMLinkNode add ) {
#if TestLinkNode
			SMLog.Debug( $"{nameof( LinkLast )} : start\n{string.Join( "\n", GetAlls() )}" );
#endif
			var last = GetLast();
			last._next = add;
			add._previous = last;
#if TestLinkNode
			SMLog.Debug( $"{nameof( LinkLast )} : end\n{string.Join( "\n", GetAlls() )}" );
#endif
		}

		protected void Unlink() {
#if TestLinkNode
			SMLog.Debug( $"{nameof( Unlink )} : start\n{this}" );
#endif
			if ( _previous != null ) { _previous._next = _next; }
			if ( _next != null ) { _next._previous = _previous; }
			_previous = null;
			_next = null;
#if TestLinkNode
			SMLog.Debug( $"{nameof( Unlink )} : end\n{this}" );
#endif
		}



		protected SMLinkNode GetFirst() {
			SMLinkNode first = null;
			for ( first = this; first._previous != null; first = first._previous ) { }
#if TestLinkNode
			SMLog.Debug( $"{nameof( GetFirst )} :\n{first}" );
#endif
			return first;
		}

		protected SMLinkNode GetLast() {
			SMLinkNode last = null;
			for ( last = this; last._next != null; last = last._next ) { }
#if TestLinkNode
			SMLog.Debug( $"{nameof( GetLast )} :\n{last}" );
#endif
			return last;
		}

		protected IEnumerable<SMLinkNode> GetAlls() {
			for ( var c = GetFirst(); c != null; c = c._next ) {
				yield return c;
			}
		}
	}
}