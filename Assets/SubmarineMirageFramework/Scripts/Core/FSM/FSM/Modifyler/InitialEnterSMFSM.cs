//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using FSM.State;
	using Debug;



	public class InitialEnterSMFSM : SMFSMModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		[SMShowLine] Type _stateType	{ get; set; }


		public InitialEnterSMFSM( Type stateType ) {
			_stateType = stateType;
		}


		public override async UniTask Run() {
			if ( _target._isFinalizing )		{ return; }
			if ( _target._isInitialEntered )	{ return; }


			if ( _target._stateBody != null ) {
				throw new InvalidOperationException(
					$"初期状態遷移前に、既に状態設定済み : {nameof( _target._stateBody )}" );
			}
			SMStateBody state = null;
			if ( _stateType != null ) {
				state = _target.GetState( _stateType );
				if ( state == null ) {
					throw new InvalidOperationException(
						$"初期状態遷移に、未所持状態を指定 : {nameof( _stateType )}" );
				}
			}


			_target._stateBody = state;
			if ( _target._stateBody == null )	{ return; }

			await _target._stateBody.Enter();
			_target._isInitialEntered = true;

			if ( !_target._owner._isInitialEnteredFSMs ) {
				_target._owner._isInitialEnteredFSMs =
					_target.GetBrothers().All( fsm => fsm._isInitialEntered );
			}

			if ( _modifyler._isHaveData )	{ return; }

			_target._stateBody.UpdateAsync();
		}
	}
}