//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using SubmarineMirage.Base;
	using FSM.State.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMFSMGenerateData<T> : SMLightBase where T : BaseSMState {
		public IEnumerable<T> _states	{ get; private set; }
		public Type _baseStateType		{ get; private set; }
		public Type _startStateType		{ get; private set; }


		public SMFSMGenerateData( IEnumerable<T> states, Type baseStateType = null,
									Type startStateType = null
		) {
			_states = states;
			_baseStateType = baseStateType;
			_startStateType = startStateType;
		}

		public SMFSMGenerateData( IEnumerable<Type> stateTypes, Type baseStateType = null,
									Type startStateType = null
		) {
			_states = stateTypes.Select( t => t.Create<T>() );
			_baseStateType = baseStateType;
			_startStateType = startStateType;
		}

		public override void Dispose() {
			_states = null;
		}
	}
}