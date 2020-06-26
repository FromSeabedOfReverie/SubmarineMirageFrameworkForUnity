//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UniRx.Async;
	using Scene;
	using Utility;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class ReRegisterSMObject : SMObjectModifyData {
		SMTaskType _lastType;
		BaseScene _lastScene;


		public ReRegisterSMObject( SMObject smObject, SMTaskType lastType, BaseScene lastScene )
			: base( smObject )
		{
			_lastType = lastType;
			_lastScene = lastScene;
		}


		public override async UniTask Run() {
			if ( _object._scene != _lastScene && _object._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _object._owner, _object._scene._scene );
			}
			_lastScene._objects.Get( _lastType )
				.Remove( _object );
			_owner.Get( _object._type )
				.Add( _object );

			await UniTaskUtility.DontWait();
		}
	}
}