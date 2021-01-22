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



	public class SMSceneInternalFSM : SMInternalFSM<SMSceneManager, SMSceneFSM, SMScene> {
		[SMHide] public SMScene _scene => _state;


		public SMSceneInternalFSM( IEnumerable<SMScene> states ) : base(
			states,
			typeof( SMScene )
		) {
			_startStateType = _states
				.Select( pair => pair.Value )
				.Where( s => !( s is UnknownSMScene ) )
				.FirstOrDefault( s => s._name == SceneManager.GetActiveScene().name )
				?.GetType()
				?? _startStateType;
		}



		public IEnumerable<SMScene> GetAllScenes()
			=> _states.Select( pair => pair.Value );

		public SMScene GetScene( Scene rawScene )
			=> GetAllScenes()
				.FirstOrDefault( s => s._rawScene == rawScene )
				?? GetState( typeof( UnknownSMScene ) );



		public UniTask ChangeScene<T>() where T : SMScene
			=> ChangeState<T>();

		public UniTask ChangeScene( Type stateType )
			=> ChangeState( stateType );
	}
}