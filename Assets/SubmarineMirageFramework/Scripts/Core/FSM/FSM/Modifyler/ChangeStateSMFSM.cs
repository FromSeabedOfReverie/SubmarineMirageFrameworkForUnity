//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using FSM.Modifyler.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class ChangeStateSMFSM : SMFSMModifyData {
		[SMShowLine] public override SMFSMModifyType _type => SMFSMModifyType.SingleRunner;
		[SMShowLine] Type _stateType	{ get; set; }


		public ChangeStateSMFSM( Type stateType ) {
			_stateType = stateType;
		}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )		{ return; }
			if ( !_owner._isInitialEntered ) {
				_owner._startStateType = _stateType;
				return;
			}

			SMStateBody state = null;
			if ( _stateType != null ) {
				state = _owner.GetState( _stateType );
				if ( state == null ) {
					throw new InvalidOperationException( $"状態遷移に、未所持状態を指定 : {_stateType}" );
				}
			}


			_owner.StopAsyncOnDisableAndExit();

			if ( _owner._stateBody != null ) {
				await _owner._stateBody.Exit();
			}
			if ( _modifyler.IsHaveData() ) {
				_owner._stateBody = null;
				return;
			}

			_owner._stateBody = state;
			if ( _owner._stateBody == null )	{ return; }

			await _owner._stateBody.Enter();
			if ( _modifyler.IsHaveData() )	{ return; }

			_owner._stateBody.UpdateAsync();
		}
	}
}