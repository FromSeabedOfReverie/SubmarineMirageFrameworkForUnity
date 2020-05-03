//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using UniRx.Async;
	using Utility;
	using Type = ProcessBody.Type;
	using LifeSpan = ProcessBody.LifeSpan;
	using RanState = ProcessBody.RanState;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


	// TODO : コメント追加、整頓


	public class RegisterHierarchyModifyData : HierarchyModifyData {
		public RegisterHierarchyModifyData( ProcessHierarchy hierarchy ) : base( hierarchy ) {}


		protected override async UniTask Run() {
			if ( _hierarchy._lifeSpan == LifeSpan.Forever && _hierarchy._owner != null ) {
				UnitySceneManager.MoveGameObjectToScene( _hierarchy._owner, _hierarchy._scene._scene );
//				Debug.Log.Debug( $"MoveGameObjectToScene : {_hierarchy._owner} {_hierarchy._scene._scene}" );
			} else {
//				Debug.Log.Debug( "Dont MoveGameObjectToScene" );
			}
			_owner.Gets( _hierarchy._type )
				.Add( _hierarchy );

			await RunHierarchy();
		}


		public async UniTask RunHierarchy() {
			switch ( _hierarchy._type ) {
				case Type.DontWork:
					await UniTaskUtility.Yield( _owner._activeAsyncCancel );
					await _hierarchy.RunStateEvent( RanState.Creating );
					return;
				case Type.Work:
				case Type.FirstWork:
					if ( _owner._isEnter ) {
						await UniTaskUtility.Yield( _owner._activeAsyncCancel );
						await _hierarchy.RunStateEvent( RanState.Creating );
						await _hierarchy.RunStateEvent( RanState.Loading );
						await _hierarchy.RunStateEvent( RanState.Initializing );
						await _hierarchy.RunActiveEvent();
					}
					return;
			}
		}
	}
}