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
	using Utility;


	// TODO : コメント追加、整頓


	public class SMObjectModifyler : IDisposableExtension {
		public SMObject _owner	{ get; private set; }
		public Queue<SMObjectModifyData> _data = new Queue<SMObjectModifyData>();
		public bool _isRunning	{ get; private set; }
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMObjectModifyler( SMObject owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				_data.ForEach( d => d.Cancel() );
				_data.Clear();
			} );
		}

		~SMObjectModifyler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Register( SMObjectModifyData data ) {
			_data.Enqueue( data );
			if ( !_isRunning )	{ Run().Forget(); }
		}


		async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( !_data.IsEmpty() ) {
				var d = _data.Dequeue();
				await d.Run();
			}
			_isRunning = false;
		}

		public async UniTask WaitRunning()
			=> await UniTaskUtility.WaitWhile( _owner._asyncCancel, () => _isRunning );

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