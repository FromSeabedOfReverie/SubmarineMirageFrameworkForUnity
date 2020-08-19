//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Scene;
	using Extension;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class ReRegisterSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;
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
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Run )} : start\n{this}" );
			if ( _object?._objects != null ) {
				Log.Debug( $"{nameof( _object )} :\n" + string.Join( "\n",
					_object._objects._objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
			if ( _lastScene?._objects != null ) {
				Log.Debug( $"{nameof( _lastScene )} :\n" + string.Join( "\n",
					_lastScene._objects._objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
#endif

			if ( _object._scene != _lastScene && _object._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _object._owner, _object._scene._scene );
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"シーン移動 :",
					$"{_object?.ToLineString()}",
					$"{_object?._scene}"
				) );
			} else {
				Log.Debug( string.Join( "\n",
					$"シーン移動なし :",
					$"{_object?.ToLineString()}",
					$"{_object?._scene}"
				) );
#endif
			}
			if ( _lastScene._objects._objects[_lastType] == _object ) {
				_lastScene._objects._objects[_lastType] = _object._next;
			}
			UnLinkObject( _object );
			RegisterObject();

			await UTask.DontWait();

#if TestSMTaskModifyler
			if ( _object?._objects != null ) {
				Log.Debug( $"{nameof( _object )} :\n" + string.Join( "\n",
					_object._objects._objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
			if ( _lastScene?._objects != null ) {
				Log.Debug( $"{nameof( _lastScene )} :\n" + string.Join( "\n",
					_lastScene._objects._objects.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" ) ) );
			}
			Log.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}


		public override string ToString() => base.ToString().InsertLast( " ",
			string.Join( ", ",
				_lastType,
				_lastScene.GetAboutName()
			)
			+ ", "
		);
	}
}