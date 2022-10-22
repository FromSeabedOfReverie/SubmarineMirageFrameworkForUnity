//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using Cysharp.Threading.Tasks;
	///====================================================================================================
	/// <summary>
	/// ■ 遅延実行のクラス
	/// </summary>
	///====================================================================================================
	public class SMLazyRunner : SMStandardBase, ISMLazyRunner {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShowLine] bool _isRequestRun;
		Action _runEvent;
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		public SMLazyRunner( string name, Action runEvent ) {
			Name = name;
			_runEvent = runEvent;

			_disposables.AddFirst( () => {
				_canceler.Dispose();
				_runEvent = null;
				_isRequestRun = false;
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// ● プロパティ
		///------------------------------------------------------------------------------------------------
		[SMShowLine] public string Name { get; set; }

		///------------------------------------------------------------------------------------------------
		/// ● 実行
		///------------------------------------------------------------------------------------------------
		public void RequestRun() {
			Run().Forget();
		}

		async UniTask Run() {
			if ( _isDispose )		{ return; }
			if ( _isRequestRun )	{ return; }
			_isRequestRun = true;

			await UTask.NextFrame( _canceler );

			_runEvent?.Invoke();
			_isRequestRun = false;
		}
	}
}