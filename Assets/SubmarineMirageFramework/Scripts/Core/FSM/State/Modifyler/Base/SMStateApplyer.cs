//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Task.Modifyler;
	using Utility;



	// TODO : コメント追加、整頓



	public static class SMStateApplyer {
		public static void FixedUpdate( SMState state ) {
			if ( state._runState != SMFSMRunState.Update )	{ return; }
			state._fixedUpdateEvent.Run();
		}

		public static void Update( SMState state ) {
			if ( state._runState != SMFSMRunState.Update )	{ return; }
			state._modifyler.Register( new UpdateSMState() );
			state._updateEvent.Run();
		}

		public static void LateUpdate( SMState state ) {
			if ( state._runState != SMFSMRunState.Update )	{ return; }
			state._lateUpdateEvent.Run();
		}
	}
}