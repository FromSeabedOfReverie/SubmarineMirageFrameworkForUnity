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
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class AddHierarchyModifyData : HierarchyModifyData {
		MonoBehaviourProcess _process;


		public AddHierarchyModifyData( ProcessHierarchy hierarchy, MonoBehaviourProcess process )
			: base( hierarchy )
		{
			_process = process;
		}


		protected override async UniTask Run() {
			_hierarchy._processes.Add( _process );
			_process._hierarchy = _hierarchy;
			_hierarchy.SetAllData();

			await RunProcess();
		}


		public async UniTask RunProcess() {
			_process.Constructor();

			switch ( _hierarchy._type ) {
				case Type.DontWork:
					await UniTaskUtility.Yield( _process._activeAsyncCancel );
					await _process.RunStateEvent( RanState.Creating );
					return;
				case Type.Work:
				case Type.FirstWork:
					if ( _owner._isEnter ) {
						await UniTaskUtility.Yield( _process._activeAsyncCancel );
						await _process.RunStateEvent( RanState.Creating );
						await _process.RunStateEvent( RanState.Loading );
						await _process.RunStateEvent( RanState.Initializing );
						await _process.RunActiveEvent();
					}
					return;
			}
		}
	}
}