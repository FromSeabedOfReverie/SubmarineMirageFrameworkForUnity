//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestScene
namespace SubmarineMirage {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;



	public class SMSceneManager : SMTask, ISMService {
		public override SMTaskRunType _type => SMTaskRunType.Sequential;

		[SMShow] SMFSM _fsm		{ get; set; }
		public Scene _foreverRawScene	{ private get; set; }
		[SMShow] public List<Scene> _firstLoadedRawScenes	{ get; set; } = new List<Scene>();



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _fsm ), i =>
				_toStringer.DefaultValue( _fsm?.GetFSMs(), i, true ) );
			_toStringer.SetValue( nameof( _firstLoadedRawScenes ), i =>
				_toStringer.DefaultValue( _firstLoadedRawScenes.Select( s => s.name ), i, false ) );
		}
#endregion



		public SMSceneManager() {
			_foreverRawScene = SceneManager.CreateScene( "Forever" );

			_disposables.AddFirst( () => {
				_fsm?.Dispose();
			} );
		}

		public override void Create() {
			var setting = SMServiceLocator.Resolve<BaseSMSceneSetting>();
			setting.Setup();
			_fsm = SMFSM.Generate( this, setting._datas );
			SMServiceLocator.Unregister<BaseSMSceneSetting>();

			_firstLoadedRawScenes = Enumerable.Range( 0, SceneManager.sceneCount )
				.Select( i => SceneManager.GetSceneAt( i ) )
				.ToList();


			_selfInitializeEvent.AddLast( async canceler => {
				// 初期読込場面を設定
				var firstScenes = _fsm.GetFSMs().ToDictionary(
					fsm => fsm,
					fsm => {
						var scenes = fsm.GetStates()
						.Select( s => s as SMScene )
						.ToList();
						return scenes.FirstOrDefault( s => IsFirstLoaded( s ) )
							?? scenes.FirstOrDefault();
					}
				);

				// 永久場面に、生場面を設定
				var foreverFSM = GetFSM<ForeverSMScene>();
				var forever = firstScenes[foreverFSM] as ForeverSMScene;
				if ( forever != null ) {
					forever.Setup( _foreverRawScene );
				}

				// 最初に不明場面を読む場合、設定
				var mainFSM = GetFSM<MainSMScene>();
				var unknown = firstScenes[mainFSM] as UnknownSMScene;
				if ( unknown != null ) {
					var unknownScenes = _firstLoadedRawScenes.Copy();
					firstScenes
						.Select( pair => pair.Value )
						.ForEach( s => unknownScenes.Remove( rs => rs.name == s._name ) );
					unknown.Setup( unknownScenes );
				}

				// 全有限状態機械を初期遷移
				foreach ( var pair in firstScenes ) {
					var fsm = pair.Key;
					var type = pair.Value.GetType();
					await fsm.ChangeState( type );
				}
			} );


			_finalizeEvent.AddLast( async canceler => {
				await _fsm.AllChangeNullState( _type );
			} );
		}

		public override void Dispose() => base.Dispose();



		public bool IsFirstLoaded( SMScene scene )
			=> _firstLoadedRawScenes
				.Any( s => s.name == scene._name );

		public bool RemoveFirstLoaded( SMScene scene )
			=> _firstLoadedRawScenes
				.RemoveFind( s => s.name == scene._name );



		public IEnumerable<string> GetScenePathsInBuild()
			=> Enumerable.Range( 0, SceneManager.sceneCountInBuildSettings )
				.Select( i => SceneUtility.GetScenePathByBuildIndex( i ) );

		public bool IsExistSceneInBuild( string path, bool isNameOnly = false )
			=> GetScenePathsInBuild()
				.Select( p => isNameOnly ? PathSMUtility.GetName( p ) : p )
				.Any( p => p == path );



		public void MoveForeverScene( GameObject gameObject )
			=> SceneManager.MoveGameObjectToScene( gameObject, _foreverRawScene );



		public SMFSM GetFSM( Type baseStateType )
			=> _fsm.GetFSM( baseStateType );

		public SMFSM GetFSM<T>() where T : SMScene
			=> _fsm.GetFSM<T>();



		public IEnumerable<SMScene> GetScenes() {
			if ( _fsm == null )	{ return Enumerable.Empty<SMScene>(); }

			return _fsm.GetFSMs()
				.SelectMany( fsm => fsm.GetStates() )
				.Select( s => s as SMScene )
				.Distinct();
		}

		public SMScene GetScene( Scene rawScene )
			=> GetScenes()
				.FirstOrDefault( s => s._rawScene == rawScene );

		public SMScene GetScene( Type baseStateType )
			=> GetFSM( baseStateType )
				._state as SMScene;
	}
}