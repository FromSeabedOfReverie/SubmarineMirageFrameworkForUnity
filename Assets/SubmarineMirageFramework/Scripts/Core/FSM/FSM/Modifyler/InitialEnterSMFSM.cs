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
	using FSM.Modifyler.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class InitialEnterSMFSM : SMFSMModifyData {
		[SMShowLine] public override SMFSMModifyType _type => SMFSMModifyType.Runner;
		[SMShowLine] Type _stateType	{ get; set; }


		public InitialEnterSMFSM( Type stateType ) {
			_stateType = stateType;
		}


		public override async UniTask Run() {
			if ( _owner._isInitialEntered )	{ return; }
			if ( _owner._stateBody != null ) {
				throw new InvalidOperationException(
					$"初期状態遷移前に、既に状態設定済み : {nameof( _owner._stateBody )}" );
			}
			SMStateBody state = null;
			if ( _stateType != null ) {
				state = _owner.GetState( _stateType );
				if ( state == null ) {
					throw new InvalidOperationException(
						$"初期状態遷移に、未所持状態を指定 : {nameof( _stateType )}" );
				}
			}


			_owner._stateBody = state;
			if ( _owner._stateBody == null )	{ return; }

			await _owner._stateBody.Enter();
			_owner._isInitialEntered = true;

			if ( !_owner._owner._isInitialEnteredFSMs ) {
				_owner._owner._isInitialEnteredFSMs =
					_owner.GetBrothers().All( fsm => fsm._isInitialEntered );
			}

			if ( _modifyler.IsHaveData() )	{ return; }

			_owner._stateBody.UpdateAsync();
		}
	}
}