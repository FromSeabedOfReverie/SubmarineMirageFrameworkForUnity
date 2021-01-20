//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Modifyler.Base {
	using Task;
	using FSM.Modifyler.Base;
	using FSM.State.Base;



	// TODO : コメント追加、整頓



	public class SMStateModifyler : BaseSMFSMModifyler<BaseSMState, SMStateModifyler, SMStateModifyData> {
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCancelerOnDispose;


		public SMStateModifyler( BaseSMState owner ) : base( owner ) {}
	}
}