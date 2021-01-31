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


	public class EnableSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public EnableSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( !SMObjectBody.IsActiveInHierarchy( _owner ) )	{ return; }
			if ( !_owner._isOperable )	{ return; }
			if ( _owner._activeState == SMTaskActiveState.Enable )	{ return; }


			_owner._isDisabling = false;

			await RunLower( _runType, () => new EnableSMBehaviour() );

			if ( _runType == SMTaskRunAllType.Parallel )	{ _owner._activeState = SMTaskActiveState.Enable; }
		}
	}
}