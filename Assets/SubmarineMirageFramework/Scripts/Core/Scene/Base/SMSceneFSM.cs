//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using FSM;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMSceneFSM : SMInternalFSM<SMSceneFSM, SMSceneManager, SMScene> {
		public ForeverSMScene _foreverScene	{ get; private set; }
		public SMScene _startScene	{ get; private set; }
		[SMHide] public SMScene _scene => _state;
		public Scene _currentScene => _scene._rawScene;
		public string _currentSceneName => _scene._name;
		public bool _isSkipLoadForFirstScene	{ get; set; } = true;


		public SMSceneFSM( SMSceneManager owner ) : base(
			owner,
			new SMScene[] {
				new UnknownSMScene(),
			}
		) {
			_foreverScene = new ForeverSMScene();
			_disposables.AddLast( _foreverScene );

			_startScene = _states
				.Select( pair => pair.Value )
				.Where( s => !( s is UnknownSMScene ) )
				.FirstOrDefault( s => s._name == SceneManager.GetActiveScene().name );
			if ( _startScene == null )	{ _startScene = _states[typeof( UnknownSMScene )]; }
			_startState = _startScene.GetType();
		}


		public SMScene Get( Scene scene ) {
			var result = _states
				.Select( pair => pair.Value )
				.FirstOrDefault( s => s._rawScene == scene );
			if ( result == null )	{ result = _states[typeof( UnknownSMScene )]; }
			return result;
		}

		public IEnumerable<SMScene> GetAllScene() {
			yield return _foreverScene;
			foreach ( var pair in _states ) {
				yield return pair.Value;
			}
		}


// TODO : MultiFSM実装後、複数シーン読込に対応する
		public async UniTask ChangeScene<T>() where T : SMScene
			=> await ChangeState<T>();

		public async UniTask ChangeScene( Type stateType )
			=> await ChangeState( stateType );
	}
}