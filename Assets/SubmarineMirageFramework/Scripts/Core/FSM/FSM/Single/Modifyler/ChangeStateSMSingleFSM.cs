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


			SMFSMApplyer.StopAsyncOnDisableAndExit( _owner );

			if ( _owner._state != null ) {
				await SMStateApplyer.Exit( _owner._state );
			}
			if ( _modifyler.IsHaveData() ) {
				_owner._state = null;
				return;
			}

			_owner._state = state;
			if ( _owner._state == null )	{ return; }

			await SMStateApplyer.Enter( _owner._state );
			if ( _modifyler.IsHaveData() )	{ return; }

			SMStateApplyer.UpdateAsync( _owner._state );
		}
	}
}