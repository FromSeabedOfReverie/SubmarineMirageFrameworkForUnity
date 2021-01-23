//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using UnityEngine.SceneManagement;


	// TODO : コメント追加、整頓


	public class TestFieldChunk2SMScene : FieldChunkSMScene {
		public TestFieldChunk2SMScene() {
			_rawScene = SceneManager.CreateScene( _name );

			_enterEvent.Remove( _registerEventName );
			_enterEvent.AddFirst( _registerEventName, async canceler => await _groups.Enter() );
			_exitEvent.Remove( _registerEventName );
			_exitEvent.AddFirst( _registerEventName, async canceler => await _groups.Exit() );
		}
	}
}