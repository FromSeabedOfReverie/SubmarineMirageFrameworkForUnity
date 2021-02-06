//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Modifyler.Base;
	using Debug;


	// TODO : コメント追加、整頓


	public class InitialEnableSMObject : SMObjectModifyData {
		[SMShowLine] public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public InitialEnableSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }
			if ( _owner._ranState != SMTaskRunState.Initialize )	{ return; }


			_owner._isDisabling = false;

			var isActiveInHierarchy = _owner.IsActiveInHierarchy();
			await RunLower( _runType, () => new InitialEnableSMBehaviour( isActiveInHierarchy ) );

			if ( _runType == SMTaskRunAllType.Parallel ) {
				_owner._activeState = SMTaskActiveState.Enable;
				_owner._ranState = SMTaskRunState.InitialEnable;
			}
		}
	}
}