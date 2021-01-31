//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using Cysharp.Threading.Tasks;
	using FSM.Base;
	using FSM.Modifyler.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class FinalExitSMSingleFSM<TOwner, TState> : SMSingleFSMModifyData<TOwner, TState>
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		[SMShowLine] public override SMFSMModifyType _type => SMFSMModifyType.FirstRunner;


		public override async UniTask Run() {
			_owner._body.StopAsyncOnDisableAndExit();

			if ( _owner._state != null ) {
				await _owner._state._body.Exit();
				_owner._state = null;
			}

			_modifyler.Dispose();
		}
	}
}