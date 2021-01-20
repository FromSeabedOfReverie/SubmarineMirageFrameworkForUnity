//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using FSM.Base;
	using FSM.Modifyler.Base;
	using FSM.State.Base;
	using FSM.State.Modifyler;



	// TODO : コメント追加、整頓



	public class ChangeStateSMSingleFSM<TOwner, TState> : SMSingleFSMModifyData<TOwner, TState>
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		public override SMFSMModifyType _type => SMFSMModifyType.SingleRunner;
		Type _stateType	{ get; set; }
		TState _state	{ get; set; }


		public ChangeStateSMSingleFSM( Type stateType ) {
			_stateType = stateType;
		}

		public override void Set( SMFSM owner ) {
			base.Set( owner );

			if ( _stateType == null ) {
				_state = null;
				return;
			}
			_state = _owner.GetState( _stateType );
			if ( _state == null ) {
				throw new InvalidOperationException( $"状態遷移に、未所持状態を指定 : {_stateType}" );
			}
		}


		public override async UniTask Run() {
			if ( _owner._state != null ) {
				await _owner._state._modifyler.RegisterAndRun( new ExitSMState() );
			}
			if ( _owner._isFinalizing ) {
				_owner._state = null;
				return;
			}

			_owner._state = _state;
			if ( _owner._state == null )	{ return; }

			await _owner._state._modifyler.RegisterAndRun( new EnterSMState() );
			if ( _owner._isFinalizing )	{ return; }

			_owner._state._modifyler.Register( new UpdateSMState() );
		}
	}
}