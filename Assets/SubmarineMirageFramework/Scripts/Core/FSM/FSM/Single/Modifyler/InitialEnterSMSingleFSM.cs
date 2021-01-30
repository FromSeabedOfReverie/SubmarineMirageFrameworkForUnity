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
	using Debug;



	// TODO : コメント追加、整頓



	public class InitialEnterSMSingleFSM<TOwner, TState> : SMSingleFSMModifyData<TOwner, TState>
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		[SMShowLine] public override SMFSMModifyType _type => SMFSMModifyType.Runner;
		[SMShowLine] Type _stateType	{ get; set; }


		public InitialEnterSMSingleFSM( Type stateType ) {
			_stateType = stateType;
		}


		public override async UniTask Run() {
			if ( _owner._isInitialEntered )	{ return; }
			if ( _owner._state != null ) {
				throw new InvalidOperationException(
					$"初期状態遷移前に、既に状態設定済み : {nameof( _owner._state )}" );
			}
			TState state = null;
			if ( _stateType != null ) {
				state = _owner.GetState( _stateType );
				if ( state == null ) {
					throw new InvalidOperationException(
						$"初期状態遷移に、未所持状態を指定 : {nameof( _stateType )}" );
				}
			}


			_owner._state = state;
			if ( _owner._state == null )	{ return; }

			await SMStateApplyer.Enter( _owner._state );
			_owner._isInitialEntered = true;

			if ( _modifyler.IsHaveData() )	{ return; }

			SMStateApplyer.UpdateAsync( _owner._state );
		}
	}
}