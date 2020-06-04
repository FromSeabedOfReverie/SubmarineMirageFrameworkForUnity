//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using UniRx.Async;
	using Scene;
	using Utility;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
	using Type = ProcessBody.Type;


	// TODO : コメント追加、整頓


	public class ReRegisterHierarchyModifyData : HierarchyModifyData {
		Type _lastType;
		BaseScene _lastScene;


		public ReRegisterHierarchyModifyData( ProcessHierarchy hierarchy, Type lastType, BaseScene lastScene )
			: base( hierarchy )
		{
			_lastType = lastType;
			_lastScene = lastScene;
		}


		protected override async UniTask Run() {
			if ( _hierarchy._scene != _lastScene && _hierarchy._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _hierarchy._owner, _hierarchy._scene._scene );
			}
			_lastScene._hierarchies.Get( _lastType )
				.Remove( _hierarchy );
			_owner.Get( _hierarchy._type )
				.Add( _hierarchy );

			await UniTaskUtility.DontWait();
		}
	}
}