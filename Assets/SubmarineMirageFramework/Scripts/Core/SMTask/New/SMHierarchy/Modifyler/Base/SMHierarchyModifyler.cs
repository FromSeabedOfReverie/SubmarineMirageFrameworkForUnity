//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public class SMHierarchyModifyler : IDisposableExtension {
		public SMHierarchy _owner	{ get; private set; }
		readonly Queue<SMHierarchyModifyData> _data = new Queue<SMHierarchyModifyData>();
		public bool _isRunning	{ get; private set; }
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMHierarchyModifyler( SMHierarchy owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				_data.ForEach( d => d._hierarchy.Dispose() );
				_data.Clear();
			} );
		}

		~SMHierarchyModifyler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Register( SMHierarchyModifyData data ) {
			data._owner = this;
			_data.Enqueue( data );
			if ( !_isRunning )	{ Run().Forget(); }
		}


		async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( !_data.IsEmpty() ) {
				var d = _data.Dequeue();
				await d.RunTop();
			}
			_isRunning = false;
		}


		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    {nameof( _isRunning )} : {_isRunning}\n"
				+ $"    {nameof( _data )} : \n"
					+ string.Join( "\n", _data.Select( d => $"        {d}" ) )
				+ "\n)";
			return result;
		}
	}
}