//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using UniRx.Async;
	using Utility;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class ReRegisterHierarchyModifyData : HierarchyModifyData {
		ProcessHierarchyManager _lastOwner;


		public ReRegisterHierarchyModifyData( ProcessHierarchy hierarchy, ProcessHierarchyManager lastOwner ) {
			_hierarchy = hierarchy;
			_lastOwner = lastOwner;
		}


		protected override async UniTask Run() {
			if ( _hierarchy._scene != _lastOwner._owner && _hierarchy._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _hierarchy._owner, _owner._owner._scene );
			}
			_lastOwner.Gets( _hierarchy._type )
				.Remove( _hierarchy );
			_owner.Gets( _hierarchy._type )
				.Add( _hierarchy );

			await UniTaskUtility.DontWait();
		}
	}
}