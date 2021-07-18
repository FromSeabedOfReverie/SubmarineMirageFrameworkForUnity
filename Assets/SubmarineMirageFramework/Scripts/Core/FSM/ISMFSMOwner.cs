//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using Base;
	using Event;



	public interface ISMFSMOwner : ISMStandardBase {
		SMSubject _fixedUpdateEvent	{ get; }
		SMSubject _updateEvent		{ get; }
		SMSubject _lateUpdateEvent	{ get; }
	}
}