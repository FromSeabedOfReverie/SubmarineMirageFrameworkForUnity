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


	public class AddHierarchyModifyData : HierarchyModifyData {
		MonoBehaviourProcess _process;


		public AddHierarchyModifyData( ProcessHierarchy hierarchy, MonoBehaviourProcess process ) {
			_hierarchy = hierarchy;
			_process = process;
		}


		protected override async UniTask Run() {
// TODO : 兄達の既存属性、弟の属性を考慮し、新規設定、親にも伝搬される
			_hierarchy._processes.Add( _process );
			_process._hierarchy = _hierarchy;

			await RunProcess( _process );
		}


		public async UniTask RunProcess( MonoBehaviourProcess process ) {
			process.Constructor();

// TODO : _ownerの総合状態により、実行するか決める
			switch ( process._type ) {
				case Type.DontWork:
					await UniTaskUtility.Yield( process._activeAsyncCancel );
					await process.RunStateEvent( RanState.Creating );
					return;
				case Type.Work:
				case Type.FirstWork:
					if ( _owner._isEnter ) {
						await UniTaskUtility.Yield( process._activeAsyncCancel );
						await process.RunStateEvent( RanState.Creating );
						await process.RunStateEvent( RanState.Loading );
						await process.RunStateEvent( RanState.Initializing );
						await process.RunActiveEvent();
					}
					return;
			}
		}
	}
}