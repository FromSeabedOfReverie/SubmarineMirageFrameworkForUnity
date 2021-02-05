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
	using SubmarineMirage.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMFSMGenerateList<T> : SMLightBase, IEnumerable< SMFSMGenerateData<T> >
		where T : BaseSMState
	{
		readonly List< SMFSMGenerateData<T> > _data = new List< SMFSMGenerateData<T> >();


		public override void Dispose() {
			_data.ForEach( d => d.Dispose() );
			_data.Clear();
		}


		IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
		public IEnumerator< SMFSMGenerateData<T> > GetEnumerator() => _data.GetEnumerator();


		public void Add( IEnumerable<T> states, Type baseStateType = null, Type startStateType = null )
			=> _data.Add( new SMFSMGenerateData<T>( states, baseStateType, startStateType ) );

		public void Add( IEnumerable<Type> stateTypes, Type baseStateType = null, Type startStateType = null )
			=> _data.Add( new SMFSMGenerateData<T>( stateTypes, baseStateType, startStateType ) );
	}
}