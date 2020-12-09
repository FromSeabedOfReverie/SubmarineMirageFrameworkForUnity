//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
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


		public ReRegisterSMObject( SMTaskType lastType, BaseScene lastScene ) : base( null ) {
			_lastType = lastType;
			_lastScene = lastScene;
		}

		public override void Cancel()	{}


		public override async UniTask Run() {
			_object = _group._topObject;	// コンストラクタで設定出来ない為、ここで設定

#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Run )} : start\n{this}" );
			if ( _group._objects != null ) {
				Log.Debug( string.Join( "\n",
					$"{nameof( _group )} :",
					string.Join( "\n",
						_group._objects._groups.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" )
					)
				) );
			}
			if ( _lastScene?._objects != null ) {
				Log.Debug( string.Join( "\n",
					$"{nameof( _lastScene )} :",
					string.Join( "\n",
						_lastScene._objects._groups.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" )
					)
				) );
			}
#endif

			if ( _group._isGameObject && _group._scene != _lastScene ) {
				UnitySceneManager.MoveGameObjectToScene( _group._gameObject, _group._scene._scene );
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{_group._lifeSpan}シーン移動 :",
					$"{_object?.ToLineString()}",
					$"{_group?._scene}"
				) );
			} else {
				Log.Debug( string.Join( "\n",
					$"シーン移動なし :",
					$"{_object?.ToLineString()}",
					$"{_group?._scene}"
				) );
#endif
			}
			_lastScene._objects.Unregister( _group, _lastType );
			_group._objects.Register( _group );

			await UTask.DontWait();

#if TestSMTaskModifyler
			if ( _group._objects != null ) {
				Log.Debug( string.Join( "\n",
					$"{nameof( _group )} :",
					string.Join( "\n",
						_group._objects._groups.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" )
					)
				) );
			}
			if ( _lastScene?._objects != null ) {
				Log.Debug( string.Join( "\n",
					$"{nameof( _lastScene )} :",
					string.Join( "\n",
						_lastScene._objects._groups.Select( pair => $"{pair.Key} : {pair.Value?.ToLineString()}" )
					)
				) );
			}
			Log.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
		}


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_lastType,
				_lastScene.GetAboutName()
			)
			+ ", "
		);
	}
}