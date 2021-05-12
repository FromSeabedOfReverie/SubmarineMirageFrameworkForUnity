//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Base;
	using FSM.State;
	using Debug;



	public class SMFSMGenerateList : SMLightBase, IEnumerable<SMFSMGenerateData> {
		readonly List<SMFSMGenerateData> _data = new List<SMFSMGenerateData>();


		public override void Dispose() {
			_data.ForEach( d => d.Dispose() );
			_data.Clear();
		}


		IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
		public IEnumerator<SMFSMGenerateData> GetEnumerator() => _data.GetEnumerator();


		public void Add( IEnumerable<SMState> states, Type baseStateType = null, Type startStateType = null,
							bool isInitialEnter = true, bool isLockBeforeInitialize = false
		) => _data.Add( new SMFSMGenerateData(
			states, baseStateType, startStateType, isInitialEnter, isLockBeforeInitialize
		) );

		public void Add( IEnumerable<Type> stateTypes, Type baseStateType = null, Type startStateType = null,
							bool isInitialEnter = true, bool isLockBeforeInitialize = false
		) => _data.Add( new SMFSMGenerateData(
			stateTypes, baseStateType, startStateType, isInitialEnter, isLockBeforeInitialize
		) );
	}
}