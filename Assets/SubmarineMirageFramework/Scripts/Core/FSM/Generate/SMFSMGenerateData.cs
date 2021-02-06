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
		IEnumerable<Type> _stateTypes	{ get; set; }
		public Type _baseStateType		{ get; private set; }
		public Type _startStateType		{ get; private set; }
		public bool _isInitialEnter		{ get; private set; }


		public SMFSMGenerateData( IEnumerable<T> states, Type baseStateType, Type startStateType,
									bool isInitialEnter
		) {
			_states = states;
			_baseStateType = baseStateType;
			_startStateType = startStateType;
			_isInitialEnter = isInitialEnter;
		}

		public SMFSMGenerateData( IEnumerable<Type> stateTypes, Type baseStateType, Type startStateType,
									bool isInitialEnter
		) {
			_stateTypes = stateTypes;
			_baseStateType = baseStateType;
			_startStateType = startStateType;
			_isInitialEnter = isInitialEnter;
		}


		public override void Dispose() {
			_states = null;
			_stateTypes = null;
		}


		public void CreateStates() {
			if ( _stateTypes == null )	{ return; }

			_states = _stateTypes.Select( t => t.Create<T>() );
			_stateTypes = null;
		}
	}
}