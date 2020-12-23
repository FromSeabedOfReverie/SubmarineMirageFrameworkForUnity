//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class EnableSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public EnableSMBehaviour( SMBehaviourBody target ) : base( target ) {}

		protected override void Cancel() {}


		public override async UniTask Run() {
			if ( !_target._isOperable )	{ return; }

			await Run( _target );
		}

		public static async UniTask Run( SMBehaviourBody target ) {
			if ( target._owner._type == SMTaskType.DontWork )		{ return; }
			if ( target._activeState == SMTaskActiveState.Enable )	{ return; }

			target._enableEvent.Run();
			target._activeState = SMTaskActiveState.Enable;
			await UTask.DontWait();
		}
	}
}