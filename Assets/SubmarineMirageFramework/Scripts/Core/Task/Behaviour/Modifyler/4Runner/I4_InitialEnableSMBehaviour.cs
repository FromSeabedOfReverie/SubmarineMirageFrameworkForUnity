//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Modifyler.Base;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class InitialEnableSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;
		[SMShowLine] bool _isActiveInHierarchy	{ get; set; }


		public InitialEnableSMBehaviour( bool isActiveInHierarchy )
			=> _isActiveInHierarchy = isActiveInHierarchy;


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }
			if ( _owner._ranState != SMTaskRunState.Initialize )	{ return; }


			if ( _owner._isRunInitialActive ) {
				_owner._isRunInitialActive = false;

				if (	_isActiveInHierarchy && _owner.IsActiveInComponent() &&
						_owner._activeState != SMTaskActiveState.Enable
				) {
					_owner._enableEvent.Run();
					_owner._activeState = SMTaskActiveState.Enable;
				}
			}

			_owner._ranState = SMTaskRunState.InitialEnable;

			await UTask.DontWait();
		}
	}
}