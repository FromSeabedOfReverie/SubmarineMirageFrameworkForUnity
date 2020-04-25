//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using System.Linq;
	using System.Collections.Generic;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public class HierarchyModifyler : IDisposableExtension {
		ProcessHierarchyManager _owner;
		readonly List<HierarchyModifyData> _allData = new List<HierarchyModifyData>();
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public HierarchyModifyler( ProcessHierarchyManager owner ) {
			_owner = owner;
			_disposables.AddLast( () => {
				_allData.ForEach( d => d._hierarchy.Dispose() );
				_allData.Clear();
			} );
		}

		~HierarchyModifyler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Register( HierarchyModifyData data ) {
			data._owner = _owner;
			data._modifyler = this;
			_allData.Add( data );
		}


		public void Run() {
			if ( _allData.IsEmpty() )	{ return; }
			UniTask.WhenAll(
				_allData.Select( d => d.RunTop() )
			).Forget();
			_allData.RemoveAll( d => !d._isRunning );
		}


		public async UniTask WaitForRunning() {
// TODO : Destroy中のオブジェクトを、管理クラスで監視する
//			削除中にシーン遷移を待機する等の、必要あり
			await UniTaskUtility.WaitWhile( _owner._activeAsyncCancel, () => !_allData.IsEmpty() );
		}


		public override string ToString() =>
			$"{this.GetAboutName()} (\n{string.Join( "\n", _allData.Select( data => $"    {data}" ) )}\n)";
	}
}