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
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using UTask;
	using Extension;


	// TODO : コメント追加、整頓


	public class SMObjectModifyler : IDisposableExtension {
		public SMObject _owner	{ get; private set; }
		public Queue<SMObjectModifyData> _data = new Queue<SMObjectModifyData>();
		bool _isRunning;
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
			=> await UTask.WaitWhile( _owner._asyncCanceler, () => _isRunning );


		public override string ToString() {
			var result = string.Join( "\n",
				$"{this.GetAboutName()}(",
				$"    {nameof( _isRunning )} : {_isRunning}",
				$"    {nameof( _data )} : \n"
					+ string.Join( "\n", _data.Select( d => $"        {d}" ) ),
				")"
			);
			return result;
		}
	}
}