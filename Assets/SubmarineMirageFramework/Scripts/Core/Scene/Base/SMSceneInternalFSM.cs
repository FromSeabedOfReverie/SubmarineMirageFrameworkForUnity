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
	using FSM.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneInternalFSM : SMInternalFSM<SMSceneManager, SMSceneFSM, SMScene> {
		[SMHide] public SMScene _scene => _state;


		public SMSceneInternalFSM( IEnumerable<SMScene> states, Type baseStateType )
			: base( states, baseStateType )
		{
		}


		public override void Set( IBaseSMFSMOwner topOwner, IBaseSMFSMOwner owner ) {
			var fsm = (SMSceneFSM)owner;

			_startStateType = _states
				.Select( pair => pair.Value )
				.FirstOrDefault( s => fsm.IsFirstLoaded( s ) )
				?.GetType();

			base.Set( topOwner, owner );
		}



		public IEnumerable<SMScene> GetAllScenes()
			=> _states.Select( pair => pair.Value );

		public SMScene GetScene( Scene rawScene )
			=> GetAllScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );



		public UniTask ChangeScene<T>() where T : SMScene
			=> ChangeState<T>();

		public UniTask ChangeScene( Type stateType )
			=> ChangeState( stateType );
	}
}