//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using Cysharp.Threading.Tasks;
	using Behaviour.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class DisableSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public DisableSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( !SMObjectApplyer.IsActiveInHierarchy( _owner ) )	{ return; }
			if ( !_owner._isOperable )	{ return; }
			if ( _owner._activeState == SMTaskActiveState.Disable && !_owner._isDisabling )	{ return; }


			_owner._activeState = SMTaskActiveState.Disable;
			_owner._isDisabling = true;

			await RunLower( _runType, b => new DisableSMBehaviour() );

			if ( _runType == SMTaskRunAllType.ReverseSequential )	{ _owner._isDisabling = false; }
		}
	}
}