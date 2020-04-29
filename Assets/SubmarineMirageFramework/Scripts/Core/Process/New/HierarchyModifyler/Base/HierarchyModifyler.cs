//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public class HierarchyModifyler : IDisposableExtension {
		ProcessHierarchyManager _owner;
		public readonly List<HierarchyModifyData> _registerData = new List<HierarchyModifyData>();
		public readonly List<HierarchyModifyData> _runningData = new List<HierarchyModifyData>();
		public readonly ReactiveProperty<bool> _isLock = new ReactiveProperty<bool>();
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public HierarchyModifyler( ProcessHierarchyManager owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				_registerData.ForEach( d => d._hierarchy.Dispose() );
				_registerData.Clear();
				_runningData.ForEach( d => d._hierarchy.Dispose() );
				_runningData.Clear();
			} );

			_isLock
				.SkipLatestValueOnSubscribe()
				.Where( isLock => !isLock )
				.Subscribe( _ => Run() );
			_disposables.AddLast( _isLock );
		}

		~HierarchyModifyler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Register( HierarchyModifyData data ) {
			data._owner = _owner;
			data._modifyler = this;
			_registerData.Add( data );

			if ( !_isLock.Value )	{ Run(); }
		}


		void Run() {
			if ( _registerData.IsEmpty() )	{ return; }
			var temp = new List<HierarchyModifyData>( _registerData );
			_runningData.Add( temp );
			_registerData.Clear();
			temp.ForEach( d => d.RunTop().Forget() );
		}


		public async UniTask WaitForRunning() {
// TODO : Destroy中のオブジェクトを、管理クラスで監視する
//			削除中にシーン遷移を待機する等の、必要あり
			await UniTaskUtility.WaitWhile( _owner._activeAsyncCancel, () => !_runningData.IsEmpty() );
		}


		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    _isLock : {_isLock}\n";
			result += "    _registerData : \n";
			_registerData.ForEach( data => result += $"        {data}\n" );
			result += "    _runningData : \n";
			_runningData.ForEach( data => result += $"        {data}\n" );
			result += ")";
			return result;
		}
	}
}