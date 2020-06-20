//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UniRx.Async;


	// TODO : コメント追加、整頓


	public class RunActiveSMHierarchy : SMHierarchyModifyData {
		public RunActiveSMHierarchy( SMHierarchy hierarchy ) : base( hierarchy )	{}


		protected override async UniTask Run() {
			await _hierarchy.RunActiveEvent();
		}
	}
}