//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Object;
	using Object.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMBehaviourApplyer {
		public static void Link( SMObject smObject, SMMonoBehaviour add ) {
			var last = smObject.GetBehaviourAtLast();

#if TestBehaviourModifyler
			SMLog.Debug( $"{nameof( Link )} : start" );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( last )} : {last}",
				$"{nameof( add )} : {add}",
				$"{nameof( smObject )} : {smObject}"
			) );
#endif
			last._next = add;
			add._previous = last;
			add._object = smObject;

#if TestBehaviourModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( last )} : {last}",
				$"{nameof( add )} : {add}",
				$"{nameof( smObject )} : {smObject}"
			) );
			SMLog.Debug( $"{nameof( Link )} : end" );
#endif
		}


		public static void Unlink( SMBehaviourBody body ) {
			var b = body._behaviour;
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



		public static void StopAsyncOnDisable( SMBehaviourBody body ) {
			body._asyncCancelerOnDisable.Cancel();
		}



		public static bool IsActiveInMonoBehaviour( SMBehaviourBody body ) {
			if ( !body._behaviour._object._isGameObject )	{ return true; }

			var mb = (SMMonoBehaviour)body._behaviour;
			return mb.enabled;
		}

		public static bool IsActiveInHierarchyAndMonoBehaviour( ISMBehaviour behaviour ) {
			if ( !behaviour._object._isGameObject )	{ return true; }

			var mb = (SMMonoBehaviour)behaviour;
			return mb.gameObject.activeInHierarchy && mb.enabled;
		}



		public static void RegisterRunEventToOwner( SMObject smObject, ISMBehaviour add ) {
			if (	smObject._ranState >= SMTaskRunState.FinalDisable &&
					add._body._ranState < SMTaskRunState.FinalDisable &&
					add._type != SMTaskType.DontWork
			) {
				var isActive = SMObjectApplyer.IsActiveInHierarchy( smObject );
				add._body._modifyler.Register( new FinalDisableSMBehaviour( isActive ) );
			}

			if (	smObject._ranState >= SMTaskRunState.Finalize &&
					add._body._ranState < SMTaskRunState.Finalize
			) {
				if ( add._type != SMTaskType.DontWork ) {
					add._body._modifyler.Register( new FinalizeSMBehaviour() );
				} else {
					add.Dispose();
				}
			}

			if ( smObject._isFinalizing )	{ return; }


			if (	smObject._ranState >= SMTaskRunState.Create &&
					add._body._ranState < SMTaskRunState.Create
			) {
				add._body._modifyler.Register( new CreateSMBehaviour() );
			}

			if ( add._type == SMTaskType.DontWork )	{ return; }


			if (	smObject._ranState >= SMTaskRunState.SelfInitialize &&
					add._body._ranState < SMTaskRunState.SelfInitialize
			) {
				add._body._modifyler.Register( new SelfInitializeSMBehaviour() );
			}

			if (	smObject._ranState >= SMTaskRunState.Initialize &&
					add._body._ranState < SMTaskRunState.Initialize
			) {
				add._body._modifyler.Register( new InitializeSMBehaviour() );
			}

			if (	smObject._ranState >= SMTaskRunState.InitialEnable &&
					add._body._ranState < SMTaskRunState.InitialEnable
			) {
				var isActive = SMObjectApplyer.IsActiveInHierarchy( smObject );
				add._body._modifyler.Register( new InitialEnableSMBehaviour( isActive ) );
			}

			if ( !smObject._isInitialized )	{ return; }


			if ( SMObjectApplyer.IsActiveInHierarchy( smObject ) ) {
				add._body._modifyler.Register( new EnableSMBehaviour() );

			} else {
				add._body._modifyler.Register( new DisableSMBehaviour() );
			}
		}



		public static void FixedUpdate( SMBehaviourBody body ) {
			if ( !body._isOperable )	{ return; }
			if ( !body._isActive )		{ return; }
			if ( body._ranState < SMTaskRunState.InitialEnable )	{ return; }

#if TestBehaviourModifyler
			SMLog.Debug( $"{body._behaviour.GetAboutName()}.{nameof(FixedUpdate)} :\n{body}" );
#endif
			body._fixedUpdateEvent.Run();

			if ( body._ranState == SMTaskRunState.InitialEnable )	{ body._ranState = SMTaskRunState.FixedUpdate; }
		}


		public static void Update( SMBehaviourBody body ) {
			if ( !body._isOperable )	{ return; }
			if ( !body._isActive )		{ return; }
			if ( body._ranState < SMTaskRunState.FixedUpdate )	{ return; }

#if TestBehaviourModifyler
			SMLog.Debug( $"{body._behaviour.GetAboutName()}.{nameof(Update)} :\n{body}" );
#endif
			body._updateEvent.Run();

			if ( body._ranState == SMTaskRunState.FixedUpdate )	{ body._ranState = SMTaskRunState.Update; }
		}


		public static void LateUpdate( SMBehaviourBody body ) {
			if ( !body._isOperable )	{ return; }
			if ( !body._isActive )		{ return; }
			if ( body._ranState < SMTaskRunState.Update )	{ return; }

#if TestBehaviourModifyler
			SMLog.Debug( $"{body._behaviour.GetAboutName()}.{nameof(LateUpdate)} :\n{body}" );
#endif
			body._lateUpdateEvent.Run();

			if ( body._ranState == SMTaskRunState.Update )	{ body._ranState = SMTaskRunState.LateUpdate; }
		}
	}
}