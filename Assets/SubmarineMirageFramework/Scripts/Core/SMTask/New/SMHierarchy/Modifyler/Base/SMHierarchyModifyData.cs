//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UniRx.Async;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class SMHierarchyModifyData {
		public SMHierarchyModifyler _owner;
		public SMHierarchy _hierarchy;
		bool _isRunning;


		public SMHierarchyModifyData( SMHierarchy hierarchy ) {
			_hierarchy = hierarchy;
//			Debug.Log.Debug( $"{this.GetAboutName()}( {_hierarchy} )" );
		}


		public async UniTask RunTop() {
			_isRunning = true;
			await Run();
			_isRunning = false;
		}

		protected abstract UniTask Run();


		public override string ToString()
			=> $"{this.GetAboutName()}( {_hierarchy._process.GetAboutName()} )";
	}
}