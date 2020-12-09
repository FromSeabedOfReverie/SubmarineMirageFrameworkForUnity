//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using Cysharp.Threading.Tasks;
	using UTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class ChangeActiveSMBehaviour : SMBehaviourModifyData {
		bool _isActive;


		public ChangeActiveSMBehaviour( SMBehaviourBody body, bool isActive, ModifyType type = ModifyType.Operator )
			: base( body )
		{
			_isActive = isActive;
			_type = type;
		}

		public override void Cancel() {}


		public override async UniTask Run() {
			if ( _body._owner._type == SMTaskType.DontWork )	{ return; }
			if ( _body._isActive == _isActive )					{ return; }
			if ( !_body._isOperable )							{ return; }
#if TestSMTaskModifyler
			Log.Debug( $"{_body._owner.GetAboutName()}.{nameof( Run )} : {_isActive}\n{_body}" );
#endif
			if ( _isActive ) {
				_body._enableEvent.Run();
			} else {
				_body.StopAsyncOnDisable();
				_body._disableEvent.Run();
			}
			_body._isActive = _isActive;

			await UTask.DontWait();
		}


		public static async UniTask RegisterAndRun( ISMBehaviour behaviour, bool isActive ) {
			behaviour._body._modifyler.Register( new ChangeActiveSMBehaviour( behaviour._body, isActive ) );
			await behaviour._body._modifyler.WaitRunning();
		}

		public static async UniTask RegisterAndRunInitial( ISMBehaviour behaviour ) {
			behaviour._body._modifyler.Register( new ChangeActiveSMBehaviour(
				behaviour._body, behaviour._body._isInitialActive, ModifyType.Initializer
			) );
			await behaviour._body._modifyler.WaitRunning();
		}


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_isActive
			)
			+ ", "
		);
	}
}