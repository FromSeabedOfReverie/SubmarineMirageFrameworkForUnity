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



	public class ChangeStateSMFSM : SMFSMModifyData {
		public override SMFSMModifyType _type => SMFSMModifyType.Runner;
		BaseSMState _state	{ get; set; }


		public ChangeStateSMFSM( BaseSMState state ) {
			_state = state;
		}

		public override async UniTask Run() {
			if ( !_owner._isOperable ) {
				throw new InvalidOperationException( $"操作不能時の状態変更 : {_owner}" );
			}


			if ( _owner._rawState != null ) {
				await _owner._rawState._modifyler.RegisterAndRun( new ExitSMState() );
			}
			if ( _owner._isFinalizing ) {
				_owner._rawState = null;
				return;
			}

			_owner._rawState = _state;
			if ( _owner._rawState == null )	{ return; }

			await _owner._rawState._modifyler.RegisterAndRun( new EnterSMState() );
			if ( _owner._isFinalizing )	{ return; }

			_owner._rawState._modifyler.Register( new UpdateSMState() );
		}
	}
}