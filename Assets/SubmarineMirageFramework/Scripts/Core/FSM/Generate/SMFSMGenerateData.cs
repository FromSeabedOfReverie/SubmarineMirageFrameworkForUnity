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
	using Base;
	using FSM.State;
	using Extension;
	using Debug;



	public class SMFSMGenerateData : SMLightBase {
		public IEnumerable<SMState> _states	{ get; private set; }
		IEnumerable<Type> _stateTypes		{ get; set; }
		public Type _baseStateType			{ get; private set; }
		public Type _startStateType			{ get; private set; }
		public bool _isInitialEnter			{ get; private set; }
		public bool _isLockBeforeInitialize	{ get; private set; }


		public SMFSMGenerateData( IEnumerable<SMState> states, Type baseStateType, Type startStateType,
									bool isInitialEnter, bool isLockBeforeInitialize
		) {
			_states = states;
			_baseStateType = baseStateType;
			_startStateType = startStateType;
			_isInitialEnter = isInitialEnter;
			_isLockBeforeInitialize = isLockBeforeInitialize;
		}

		public SMFSMGenerateData( IEnumerable<Type> stateTypes, Type baseStateType, Type startStateType,
									bool isInitialEnter, bool isLockBeforeInitialize
		) {
			_stateTypes = stateTypes;
			_baseStateType = baseStateType;
			_startStateType = startStateType;
			_isInitialEnter = isInitialEnter;
			_isLockBeforeInitialize = isLockBeforeInitialize;
		}


		public override void Dispose() {
			_states = null;
			_stateTypes = null;
		}


		public void CreateStates() {
			if ( _stateTypes == null )	{ return; }

			_states = _stateTypes.Select( t => t.Create<SMState>() );
			_stateTypes = null;
		}
	}
}