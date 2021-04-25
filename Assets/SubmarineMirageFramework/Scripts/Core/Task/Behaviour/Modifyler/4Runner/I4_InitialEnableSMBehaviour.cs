//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class InitialEnableSMBehaviour : SMBehaviourModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		[SMShowLine] bool _isActiveInHierarchy	{ get; set; }


		public InitialEnableSMBehaviour( bool isActiveInHierarchy )
			=> _isActiveInHierarchy = isActiveInHierarchy;


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.Initialize )	{ return; }


			if ( _target._isRunInitialActive ) {
				_target._isRunInitialActive = false;

				if (	_isActiveInHierarchy && _target.IsActiveInComponent() &&
						_target._activeState != SMTaskActiveState.Enable
				) {
					_target._enableEvent.Run();
					_target._activeState = SMTaskActiveState.Enable;
				}
			}

			_target._ranState = SMTaskRunState.InitialEnable;

			await UTask.DontWait();
		}
	}
}