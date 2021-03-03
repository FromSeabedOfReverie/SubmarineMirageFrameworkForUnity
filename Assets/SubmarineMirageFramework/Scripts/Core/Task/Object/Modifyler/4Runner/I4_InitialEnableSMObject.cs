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
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public InitialEnableSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }
			if ( _target._ranState != SMTaskRunState.Initialize )	{ return; }


			_target._isDisabling = false;

			var isActiveInHierarchy = _target.IsActiveInHierarchy();
			await RunLower( _runType, () => new InitialEnableSMBehaviour( isActiveInHierarchy ) );

			if ( _runType == SMTaskRunAllType.Parallel ) {
				_target._activeState = SMTaskActiveState.Enable;
				_target._ranState = SMTaskRunState.InitialEnable;
			}
		}
	}
}