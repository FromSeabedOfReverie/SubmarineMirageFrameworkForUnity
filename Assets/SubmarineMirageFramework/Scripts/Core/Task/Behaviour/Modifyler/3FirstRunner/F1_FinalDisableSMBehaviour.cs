//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Cysharp.Threading.Tasks;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalDisableSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public FinalDisableSMBehaviour( SMBehaviourBody target ) : base( target )
			=> _target._isFinalizing = true;

		protected override void Cancel() {}


		public override async UniTask Run() {
			if ( _target._owner._type == SMTaskType.DontWork )		{ return; }
			if ( _target._ranState >= SMTaskRunState.FinalDisable )	{ return; }

			await DisableSMBehaviour.Run( _target );
			_target._isRunFinalize = _target._ranState >= SMTaskRunState.SelfInitialize;
			_target._ranState = SMTaskRunState.FinalDisable;
		}
	}
}