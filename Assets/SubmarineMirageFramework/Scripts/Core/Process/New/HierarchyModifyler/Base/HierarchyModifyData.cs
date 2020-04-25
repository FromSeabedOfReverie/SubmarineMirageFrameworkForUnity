//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using System.Linq;
	using UniRx.Async;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class HierarchyModifyData {
		public ProcessHierarchyManager _owner;
		public HierarchyModifyler _modifyler;
		public ProcessHierarchy _hierarchy;
		public bool _isRunning	{ get; private set; }


		public async UniTask RunTop() {
			if ( _isRunning )	{ return; }
			_isRunning = true;
			await Run();
			_isRunning = false;
		}

		protected abstract UniTask Run();


		public override string ToString()
			=> $"{this.GetAboutName()}( {_owner._owner}, {_hierarchy._processes.First()} )";
	}
}