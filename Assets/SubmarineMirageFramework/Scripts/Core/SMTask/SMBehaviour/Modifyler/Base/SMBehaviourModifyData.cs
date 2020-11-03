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


	public abstract class SMBehaviourModifyData {
		public enum ModifyType {
			Interrupter,
			Runner,
		}

		static uint s_idCount;
		public uint _id			{ get; private set; }
		public ModifyType _type	{ get; protected set; }
		public SMBehaviourBody _body	{ get; private set; }


		public SMBehaviourModifyData( SMBehaviourBody body ) {
			_id = ++s_idCount;
			_body = body;
			
			if ( _body == null || _body._isDispose ) {
				throw new ObjectDisposedException( $"{nameof( _body )}", $"既に解放、削除済\n{_body}" );
			}

#if TestSMTaskModifyler
			Log.Debug( $"{nameof( SMBehaviourModifyData )}() : {this}" );
#endif
		}

		public abstract void Cancel();

		public abstract UniTask Run();


		public static void UnLinkBehaviour( SMBehaviourBody body ) {
			var b = body._owner;
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( SMBehaviourBody )}.{nameof( UnLinkBehaviour )} : start\n{b}" );
/*
			var p = b._previous;
			var n = b._next;
*/
#endif
			if ( b._object._behaviour == b )	{ b._object._behaviour = b._next; }
			if ( b._previous != null )			{ b._previous._next = b._next; }
			if ( b._next != null )				{ b._next._previous = b._previous; }
			b._previous = null;
			b._next = null;
#if TestSMTaskModifyler
/*
			Log.Debug( string.Join( "\n",
				$"_object : {b._object.ToLineString()}",
				$"_object._behaviour : {b._object._behaviour?.ToLineString()}",
				$"_owner._previous : {p?.ToLineString()}",
				$"_owner._next : {n?.ToLineString()}"
			) );
*/
			Log.Debug( $"{nameof( SMBehaviourBody )}.{nameof( UnLinkBehaviour )} : end\n{b}" );
#endif
		}


		public override string ToString() => string.Join( " ",
			$"{this.GetAboutName()}(",
			string.Join( ", ",
				_id,
				_type,
				_body?._owner.ToLineString()
			),
			")"
		);
	}
}