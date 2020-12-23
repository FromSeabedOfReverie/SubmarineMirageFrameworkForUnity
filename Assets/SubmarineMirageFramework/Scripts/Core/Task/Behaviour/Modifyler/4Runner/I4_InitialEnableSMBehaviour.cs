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


	public class InitialEnableSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public InitialEnableSMBehaviour( SMBehaviourBody target ) : base( target ) {}

		protected override void Cancel() {}


		public override async UniTask Run() {
			// 単体でDontWorkじゃなくても、全体でDontWorkになる場合がある為、ここで判定
			if ( _target._owner._type == SMTaskType.DontWork ) {
				_target._isRunInitialActive = false;
				return;
			}
			if ( _target._ranState != SMTaskRunState.Initialize )	{ return; }


			if ( _target._isRunInitialActive ) {
				await EnableSMBehaviour.Run( _target );
				_target._isRunInitialActive = false;
			}
			_target._ranState = SMTaskRunState.InitialEnable;
		}
	}
}