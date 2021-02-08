//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using Task;
	using FSM;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMSceneFSM : SMFSM<SMSceneManager, SMSceneFSM, SMScene> {
		public SMScene _scene => _state;



		public new UniTask ChangeState<T>() where T : SMScene
			=> throw new NotSupportedException( $"非対応、{nameof( ChangeScene )}を使用する" );

		public new UniTask ChangeState( Type sceneType )
			=> throw new NotSupportedException( $"非対応、{nameof( ChangeScene )}を使用する" );

		public async UniTask ChangeScene<T>() where T : SMScene {
			if ( !_isInitialEntered ) {
				throw new InvalidOperationException(
					$"初期遷移前に、別シーン遷移は不可能 : {typeof( T ).GetAboutName()}" );
			}
			await base.ChangeState<T>();
		}

		public async UniTask ChangeScene( Type sceneType ) {
			if ( !_isInitialEntered ) {
				throw new InvalidOperationException(
					$"初期遷移前に、別シーン遷移は不可能 : {sceneType.GetAboutName()}" );
			}
			await base.ChangeState( sceneType );
		}



		public new IEnumerable<SMScene> GetStates()
			=> throw new NotSupportedException( $"非対応、{nameof( GetScenes )}を使用する" );

		public new SMScene GetState( Type stateType )
			=> throw new NotSupportedException( $"非対応、{nameof( GetScene )}を使用する" );

		public new T GetState<T>() where T : SMScene
			=> throw new NotSupportedException( $"非対応、{nameof( GetScene )}を使用する" );

		public IEnumerable<SMScene> GetScenes()
			=> base.GetStates();

		public SMScene GetScene( Type stateType )
			=> base.GetState( stateType );

		public T GetScene<T>() where T : SMScene
			=> base.GetState<T>();

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