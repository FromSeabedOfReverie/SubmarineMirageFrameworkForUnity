//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM.Modifyler {
	using Task;
	using Base.Modifyler;



	// TODO : コメント追加、整頓



	public class SMFSMModifyler : BaseSMFSMModifyler<BaseSMFSM, SMFSMModifyler, SMFSMModifyData> {
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCanceler;


		public SMFSMModifyler( BaseSMFSM owner ) : base( owner ) {}
	}
}