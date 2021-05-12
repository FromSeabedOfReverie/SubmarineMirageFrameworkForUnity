//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using FSM.State;
	using Debug;



	public class ChangeStateSMFSM : SMFSMModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.SingleRunner;
		[SMShowLine] Type _stateType	{ get; set; }


		public ChangeStateSMFSM( Type stateType ) {
			_stateType = stateType;
		}


		public override async UniTask Run() {
			if ( _target._isFinalizing )		{ return; }

			if ( !_target._isInitialEntered ) {
				if ( _target._isLockBeforeInitialize ) {
					throw new InvalidOperationException( string.Join( "\n",
						$"初期遷移前の状態遷移は、ロック中 : {_stateType}",
						nameof( _target._isLockBeforeInitialize )
					) );
				}
				_target._startStateType = _stateType;
				return;
			}

			SMStateBody state = null;
			if ( _stateType != null ) {
				state = _target.GetState( _stateType );
				if ( state == null ) {
					throw new InvalidOperationException( $"状態遷移に、未所持状態を指定 : {_stateType}" );
				}
			}


			_target.StopAsyncOnDisableAndExit();

			if ( _target._stateBody != null ) {
				_target._stateBody.Disable();
				await _target._stateBody.Exit();
			}
			if ( _modifyler._isHaveData ) {
				_target._stateBody = null;
				return;
			}

			_target._stateBody = state;
			if ( _target._stateBody == null )	{ return; }

			await _target._stateBody.Enter();
			_target._stateBody.Enable();
			if ( _modifyler._isHaveData )	{ return; }

			_target._stateBody.UpdateAsync();
		}
	}
}