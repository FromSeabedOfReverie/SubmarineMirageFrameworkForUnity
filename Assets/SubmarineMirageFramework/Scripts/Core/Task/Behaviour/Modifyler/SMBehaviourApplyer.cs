//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMBehaviourApplyer {
		public static void Unlink( SMBehaviourBody body ) {
			var b = body._owner;
#if TestBehaviourModifyler
			SMLog.Debug( $"{nameof( SMBehaviourBody )}.{nameof( Unlink )} : start\n{b}" );
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
#if TestBehaviourModifyler
/*
			SMLog.Debug( string.Join( "\n",
				$"_object : {b._object.ToLineString()}",
				$"_object._behaviour : {b._object._behaviour?.ToLineString()}",
				$"_owner._previous : {p?.ToLineString()}",
				$"_owner._next : {n?.ToLineString()}"
			) );
*/
			SMLog.Debug( $"{nameof( SMBehaviourBody )}.{nameof( Unlink )} : end\n{b}" );
#endif
		}



		static void FixedUpdate( SMBehaviourBody target ) {
			if ( target._owner._type == SMTaskType.DontWork )		{ return; }
			if ( !target._isActive )	{ return; }
			if ( target._ranState == SMTaskRunState.Initialize )	{ target._ranState = SMTaskRunState.FixedUpdate; }
			switch ( target._ranState ) {
				case SMTaskRunState.FixedUpdate:
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
#if TestBehaviourModifyler
					SMLog.Debug( $"{target._owner.GetAboutName()}.{nameof(FixedUpdate)} :\n{target}" );
#endif
					target._fixedUpdateEvent.Run();
					return;
			}
		}

		static void Update( SMBehaviourBody target ) {
			if ( target._owner._type == SMTaskType.DontWork )		{ return; }
			if ( !target._isActive )	{ return; }
			if ( target._ranState == SMTaskRunState.FixedUpdate )	{ target._ranState = SMTaskRunState.Update; }
			switch ( target._ranState ) {
				case SMTaskRunState.Update:
				case SMTaskRunState.LateUpdate:
#if TestBehaviourModifyler
					SMLog.Debug( $"{target._owner.GetAboutName()}.{nameof(Update)} :\n{target}" );
#endif
					target._updateEvent.Run();
					return;
			}
		}

		static void LateUpdate( SMBehaviourBody target ) {
			if ( target._owner._type == SMTaskType.DontWork )		{ return; }
			if ( !target._isActive )	{ return; }
			if ( target._ranState == SMTaskRunState.Update )		{ target._ranState = SMTaskRunState.LateUpdate; }
			switch ( target._ranState ) {
				case SMTaskRunState.LateUpdate:
#if TestBehaviourModifyler
					SMLog.Debug( $"{target._owner.GetAboutName()}.{nameof(LateUpdate)} :\n{target}" );
#endif
					target._lateUpdateEvent.Run();
					return;
			}
		}
	}
}