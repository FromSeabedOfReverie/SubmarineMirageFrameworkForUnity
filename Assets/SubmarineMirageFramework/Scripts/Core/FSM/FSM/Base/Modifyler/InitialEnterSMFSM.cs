//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using State;
	using State.Modifyler;



	// TODO : コメント追加、整頓



	public class InitialEnterSMFSM : SMFSMModifyData {
		public override SMFSMModifyType _type => SMFSMModifyType.Runner;
		BaseSMState _state	{ get; set; }


		public InitialEnterSMFSM( Type stateType ) {
			_state = _owner.GetRawState( stateType );
			if ( _state == null ) {
				throw new InvalidOperationException( $"初期状態遷移に、未所持状態を指定 : {nameof( _state )}" );
			}
		}


		public override async UniTask Run() {
			if ( _owner._rawState != null ) {
				throw new InvalidOperationException(
					$"初期状態遷移前に、状態設定済み : {nameof( _owner._rawState )}" );
			}
// TODO : Enableとか、他条件を指定しないで良い？


			_owner._rawState = _state;

			await _owner._rawState._modifyler.RegisterAndRun( new EnterSMState() );
			_owner._isInitialEntered = true;

// TODO : _isFinalizing以外の条件を、見ないで良いの？
			if ( _owner._isFinalizing )	{ return; }
			_owner._rawState._modifyler.Register( new UpdateSMState() );
		}
	}
}