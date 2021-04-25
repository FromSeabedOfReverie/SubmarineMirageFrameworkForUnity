//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class DisableSMObject : SMObjectModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		[SMShowLine] bool _isAdjustParent	{ get; set; }


		public DisableSMObject( SMTaskRunAllType runType, bool isAdjustParent = false ) : base( runType )
			=> _isAdjustParent = isAdjustParent;


		public override async UniTask Run() {
			if ( !_isAdjustParent && !_target.IsActiveInHierarchy() )	{ return; }
			if ( !_target._isOperable )	{ return; }
			if ( _target._activeState == SMTaskActiveState.Disable && !_target._isDisabling )	{ return; }


			_target._activeState = SMTaskActiveState.Disable;
			_target._isDisabling = true;

			await RunLower( _runType, () => new DisableSMBehaviour() );

			if ( _runType == SMTaskRunAllType.ReverseSequential )	{ _target._isDisabling = false; }
		}
	}
}