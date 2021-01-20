//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler.Base {
	using Task;
	using FSM.Base;



	// TODO : コメント追加、整頓



	public class SMFSMModifyler : BaseSMFSMModifyler<SMFSM, SMFSMModifyler, SMFSMModifyData> {
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCancelerOnDispose;


		public SMFSMModifyler( SMFSM owner ) : base( owner ) {}
	}
}