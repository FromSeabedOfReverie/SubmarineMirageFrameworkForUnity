//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UniRx.Async;
	using Utility;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class RegisterSMHierarchy : SMHierarchyModifyData {
		public RegisterSMHierarchy( SMHierarchy hierarchy ) : base( hierarchy ) {}


		protected override async UniTask Run() {
			if ( _hierarchy._lifeSpan == SMTaskLifeSpan.Forever && _hierarchy._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _hierarchy._owner, _hierarchy._scene._scene );
//				Debug.Log.Debug( $"MoveGameObjectToScene : {_hierarchy._owner} {_hierarchy._scene._scene}" );
			} else {
//				Debug.Log.Debug( "Dont MoveGameObjectToScene" );
			}

			_hierarchy._hierarchies.Add( _hierarchy );
			await RunHierarchy();
		}


		public async UniTask RunHierarchy() {
			switch ( _hierarchy._type ) {
				case SMTaskType.DontWork:
					await UniTaskUtility.Yield( _hierarchy._asyncCancel );
					await _hierarchy.RunStateEvent( SMTaskRanState.Creating );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _hierarchy._hierarchies._isEnter ) {
						await UniTaskUtility.Yield( _hierarchy._asyncCancel );
						await _hierarchy.RunStateEvent( SMTaskRanState.Creating );
						await _hierarchy.RunStateEvent( SMTaskRanState.Loading );
						await _hierarchy.RunStateEvent( SMTaskRanState.Initializing );
						await _hierarchy.RunActiveEvent();
					}
					return;
			}
		}
	}
}