//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM.Modifyler {
	using Cysharp.Threading.Tasks;
	using State.Modifyler;



	// TODO : コメント追加、整頓



	public class FinalizeSMFSM : SMFSMModifyData {
		public override SMFSMModifyType _type => SMFSMModifyType.FirstRunner;


		public FinalizeSMFSM() {
			_owner._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _owner._rawState == null )	{ return; }


			await _owner._rawState._modifyler.RegisterAndRun( new ExitSMState() );
			_owner._rawState = null;
		}
	}
}