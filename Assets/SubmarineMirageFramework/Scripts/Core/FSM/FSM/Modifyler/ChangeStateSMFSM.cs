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
	using FSM.Modifyler.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class ChangeStateSMFSM : SMFSMModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.SingleRunner;
		[SMShowLine] Type _stateType	{ get; set; }


		public ChangeStateSMFSM( Type stateType ) {
			_stateType = stateType;
		}


		public override async UniTask Run() {
			if ( _target._isFinalizing )		{ return; }
			if ( !_target._isInitialEntered ) {
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
				await _target._stateBody.Exit();
			}
			if ( _modifyler.IsHaveData() ) {
				_target._stateBody = null;
				return;
			}

			_target._stateBody = state;
			if ( _target._stateBody == null )	{ return; }

			await _target._stateBody.Enter();
			if ( _modifyler.IsHaveData() )	{ return; }

			_target._stateBody.UpdateAsync();
		}
	}
}