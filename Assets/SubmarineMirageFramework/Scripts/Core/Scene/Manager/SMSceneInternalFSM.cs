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
	using Task;
	using FSM;
	using FSM.Modifyler;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneInternalFSM : SMInternalFSM<SMSceneManager, SMSceneFSM, SMScene, SMSceneType> {
		[SMHide] public SMScene _scene => _state;
		bool _isSetStartState	{ get; set; }


		public SMSceneInternalFSM( IEnumerable<SMScene> scenes, Type baseSceneType, bool isSetStartScene = true
		) : base( scenes, baseSceneType )
		{
			_isSetStartState = isSetStartScene;

			_body._setEvent.AddLast( ( topFSMOwner, fsmOwner ) => {
				_body._modifyler.Reset();

				_startStateType = null;
				if ( _isSetStartState ) {
					_startStateType = GetScenes()
						.FirstOrDefault( s => _owner.IsFirstLoaded( s ) )
						?.GetType();
				}
			} );
		}


		public UniTask InitialEnter() => _body._modifyler.RegisterAndRun(
			new InitialEnterSMSingleFSM<SMSceneManager, SMScene>( _startStateType ) );


		public UniTask ChangeScene<T>() where T : SMScene
			=> ChangeState<T>();

		public UniTask ChangeScene( Type stateType )
			=> ChangeState( stateType );



		public IEnumerable<SMScene> GetScenes()
			=> _states.Select( pair => pair.Value );

		public SMScene GetScene( Scene rawScene )
			=> GetScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );



		public T GetBehaviour<T>() where T : SMBehaviour
			=> _scene?.GetBehaviour<T>();

		public SMBehaviour GetBehaviour( Type type )
			=> _scene?.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : SMBehaviour
			=> _scene?.GetBehaviours<T>() ?? Enumerable.Empty<T>();

		public IEnumerable<SMBehaviour> GetBehaviours( Type type )
			=> _scene?.GetBehaviours( type ) ?? Enumerable.Empty<SMBehaviour>();
	}
}