//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
// TODO : 場面遷移時に、下記をマージ追加

// まだ、リファクタリング中
#if false
namespace SubmarineMirageFramework.Scene {
	using UnityEngine;
	using UnityEngine.Events;

	public class SceneManager : FSM<SceneManager> {

		/// <summary>
		/// シーンを変更する
		/// </summary>
		public void ChangeScene( string sceneName, UnityAction<Scene, LoadSceneMode> onLoaded, bool isWhiteFade = false ) {
//		LoadingScreen.instance.Display( onDone: ()=> {
			if ( m_currState != null ) {
				m_currState.OnExit();
				m_prevState = m_currState;
			}
			DG.Tweening.DOTween.KillAll();
			AudioManager.instance.bgm.Stop();	// BGM停止（DOTweenが全停止しちゃうので、再停止）
			m_onSceneLoaded = onLoaded;
			SceneManager.UnloadSceneAsync( SceneManager.GetActiveScene() );
			SceneManager.LoadSceneAsync( sceneName );
			SceneManager.sceneLoaded += m_onSceneLoaded;
			SceneManager.sceneLoaded += OnSceneLoaded;
//		}, isWhiteFade:isWhiteFade );
		}

		void OnSceneLoaded( Scene arg0, LoadSceneMode arg1 ) {
			SceneManager.sceneLoaded -= m_onSceneLoaded;
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	}
}
#endif