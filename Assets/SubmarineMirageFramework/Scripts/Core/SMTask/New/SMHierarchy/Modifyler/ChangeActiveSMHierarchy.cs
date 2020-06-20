//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UniRx.Async;


	// TODO : コメント追加、整頓


	public class ChangeActiveSMHierarchy : SMHierarchyModifyData {
		bool _isActive;
		bool _isChangeOwner;


		public ChangeActiveSMHierarchy( SMHierarchy hierarchy, bool isActive, bool isChangeOwner )
			: base( hierarchy )
		{
			_isActive = isActive;
			_isChangeOwner = isChangeOwner;
		}


		protected override async UniTask Run() {
			await _hierarchy.ChangeActive( _isActive, _isChangeOwner );
		}
	}
}