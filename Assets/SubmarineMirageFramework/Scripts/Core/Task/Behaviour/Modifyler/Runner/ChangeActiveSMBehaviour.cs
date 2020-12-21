//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Cysharp.Threading.Tasks;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class ChangeActiveSMBehaviour : SMBehaviourModifyData {
		[SMShowLine] bool _isActive	{ get; set; }


		public ChangeActiveSMBehaviour( SMBehaviourBody target, bool isActive,
										SMTaskModifyType type = SMTaskModifyType.Runner
		) : base( target )
		{
			_isActive = isActive;
			_type = type;
		}

		protected override void Cancel() {}


		public override async UniTask Run() {
			if ( _target._owner._type == SMTaskType.DontWork )	{ return; }
			if ( _target._isActive == _isActive )				{ return; }
			if ( !_target._isOperable )							{ return; }
#if TestBehaviourModifyler
			SMLog.Debug( $"{_target._owner.GetAboutName()}.{nameof( Run )} : {_isActive}\n{_target}" );
#endif
			if ( _isActive ) {
				_target._enableEvent.Run();
			} else {
				_target.StopAsyncOnDisable();
				_target._disableEvent.Run();
			}
			_target._isActive = _isActive;

			await UTask.DontWait();
		}


		public static async UniTask RegisterAndRun( ISMBehaviour target, bool isActive ) {
			target._body._modifyler.Register( new ChangeActiveSMBehaviour( target._body, isActive ) );
			await target._body._modifyler.WaitRunning();
		}

		public static async UniTask RegisterAndRunInitial( ISMBehaviour target ) {
			target._body._modifyler.Register( new ChangeActiveSMBehaviour(
				target._body, target._body._isInitialActive, SMTaskModifyType.Linker
			) );
			await target._body._modifyler.WaitRunning();
		}
	}
}