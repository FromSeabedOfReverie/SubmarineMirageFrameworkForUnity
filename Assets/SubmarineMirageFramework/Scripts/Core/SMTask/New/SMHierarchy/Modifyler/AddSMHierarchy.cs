//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using UniRx.Async;
	using Utility;


	// TODO : コメント追加、整頓


	public class AddSMHierarchy : SMHierarchyModifyData {
		SMMonoBehaviour _process;


		public AddSMHierarchy( SMHierarchy hierarchy, SMMonoBehaviour process )
			: base( hierarchy )
		{
			_process = process;
			if ( hierarchy._owner == null ) {
				throw new NotSupportedException( $"{nameof(SMBehavior)}._hierarchyに、追加不可 :\n{hierarchy}" );
			}
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
				case SMTaskType.DontWork:
					await UniTaskUtility.Yield( _process._activeAsyncCancel );
					await _process.RunStateEvent( SMTaskRanState.Creating );
					return;
				case SMTaskType.Work:
				case SMTaskType.FirstWork:
					if ( _owner._isEnter ) {
						await UniTaskUtility.Yield( _process._activeAsyncCancel );
						await _process.RunStateEvent( SMTaskRanState.Creating );
						await _process.RunStateEvent( SMTaskRanState.Loading );
						await _process.RunStateEvent( SMTaskRanState.Initializing );
						await _process.RunActiveEvent();
					}
					return;
			}
		}
	}
}