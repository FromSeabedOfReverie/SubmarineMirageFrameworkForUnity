//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMObjectModifyData {
		public enum ModifyType {
			Interrupter,
			Linker,
			Runner,
		}

		static uint s_idCount;
		public uint _id			{ get; private set; }
		public ModifyType _type	{ get; protected set; }
		public SMObject _object	{ get; protected set; }
		public SMObjectModifyler _owner;
		protected SMObjectGroup _group => _owner?._owner;



		public SMObjectModifyData( SMObject smObject ) {
			_id = ++s_idCount;
			_object = smObject;
			
			if ( _object != null && _object._isDispose ) {
				throw new ObjectDisposedException( $"{nameof( _object )}", $"既に解放、削除済\n{_object}" );
			}

#if TestSMTaskModifyler
			Log.Debug( $"{nameof( SMObjectModifyData )}() : {this}" );
#endif
		}

		public abstract void Cancel();

		public abstract UniTask Run();


		public static void AddChildObject( SMObject parent, SMObject add ) {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( AddChildObject )} : start" );
			Log.Debug( string.Join( "\n",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}"
			) );
#endif
			add._parent = parent;
			var last = parent.GetLastChild();
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( last )} : {last?.ToLineString()}" );
#endif
			if ( last != null ) {
				add._previous = last;
				last._next = add;
			} else {
				parent._child = add;
			}
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( add )} : {add?.ToLineString()}",
				$"{nameof( last )} : {last?.ToLineString()}"
			) );
			Log.Debug( $"{nameof( AddChildObject )} : end" );
#endif
		}


		public static void UnLinkObject( SMObject smObject ) {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( UnLinkObject )} : start" );
			var parent = smObject?._parent;
			var previous = smObject?._previous;
			var next = smObject?._next;
			Log.Debug( string.Join( "\n",
				$"{nameof( smObject )} : {smObject?.ToLineString()}",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( previous )} : {previous?.ToLineString()}",
				$"{nameof( next )} : {next?.ToLineString()}"
			) );
#endif
			if ( smObject._parent?._child == smObject ) {
				smObject._parent._child = smObject._next;
			}
			smObject._parent = null;

			if ( smObject._previous != null )	{ smObject._previous._next = smObject._next; }
			if ( smObject._next != null )		{ smObject._next._previous = smObject._previous; }
			smObject._previous = null;
			smObject._next = null;
#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( smObject )} : {smObject?.ToLineString()}",
				$"{nameof( parent )} : {parent?.ToLineString()}",
				$"{nameof( previous )} : {previous?.ToLineString()}",
				$"{nameof( next )} : {next?.ToLineString()}"
			) );
			Log.Debug( $"{nameof( UnLinkObject )} : end" );
#endif
		}

		protected bool IsCanChangeActive( SMObject smObject, bool isChangeOwner ) {
			if ( !smObject._isGameObject )													{ return true; }
			if ( smObject._parent != null && !smObject._parent._owner.activeInHierarchy )	{ return false; }
			if ( !isChangeOwner && !smObject._owner.activeSelf )							{ return false; }
			return true;
		}



		public override string ToString() => string.Join( " ",
			$"{this.GetAboutName()}(",
			string.Join( ", ",
				_id,
				_type,
				_object?.ToLineString()
			),
			")"
		);
	}
}