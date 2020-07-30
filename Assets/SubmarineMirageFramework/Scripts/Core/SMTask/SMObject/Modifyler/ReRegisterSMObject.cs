//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Scene;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class ReRegisterSMObject : SMObjectModifyData {
		SMTaskType _lastType;
		BaseScene _lastScene;


		public ReRegisterSMObject( SMObject smObject, SMTaskType lastType, BaseScene lastScene )
			: base( smObject )
		{
			if ( !_object._isTop ) {
				throw new NotSupportedException( $"最上階の{nameof( SMObject )}で無い為、再登録不可 :\n{_object}" );
			}
			_lastType = lastType;
			_lastScene = lastScene;
		}

		public override void Cancel() {}


		public override async UniTask Run() {
			if ( _object._scene != _lastScene && _object._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _object._owner, _object._scene._scene );
			}
			if ( _lastScene._objects._objects[_lastType] == _object ) {
				_lastScene._objects._objects[_lastType] = _object._next;
			}
			UnLinkObject( _object );
			RegisterObject();

			await UTask.DontWait();
		}
	}
}