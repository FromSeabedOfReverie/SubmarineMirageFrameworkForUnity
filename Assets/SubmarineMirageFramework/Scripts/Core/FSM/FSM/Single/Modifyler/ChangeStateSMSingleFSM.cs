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
	using Debug;



	// TODO : コメント追加、整頓



	public class ChangeStateSMSingleFSM<TOwner, TState> : SMSingleFSMModifyData<TOwner, TState>
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		[SMShowLine] public override SMFSMModifyType _type => SMFSMModifyType.SingleRunner;
		[SMShowLine] Type _stateType	{ get; set; }


		public ChangeStateSMSingleFSM( Type stateType ) {
			_stateType = stateType;
		}


		public override async UniTask Run() {
			TState state = null;
			if ( _stateType != null ) {
				state = _owner.GetState( _stateType );
				if ( state == null ) {
					throw new InvalidOperationException( $"状態遷移に、未所持状態を指定 : {_stateType}" );
				}
			}


			_owner._body.StopAsyncOnDisableAndExit();

			if ( _owner._state != null ) {
				await _owner._state._body.Exit();
			}
			if ( _modifyler.IsHaveData() ) {
				_owner._state = null;
				return;
			}

			_owner._state = state;
			if ( _owner._state == null )	{ return; }

			await _owner._state._body.Enter();
			if ( _modifyler.IsHaveData() )	{ return; }

			_owner._state._body.UpdateAsync();
		}
	}
}