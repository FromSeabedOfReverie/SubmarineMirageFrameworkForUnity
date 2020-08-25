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
		readonly LinkedList<SMObjectModifyData> _data = new LinkedList<SMObjectModifyData>();
		bool _isRunning;
		public bool _isDispose => _disposables._isDispose;
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
			switch( data._type ) {
				case SMObjectModifyData.ModifyType.Linker:
					_data.AddBefore(
						data,
						d => d._type == SMObjectModifyData.ModifyType.Runner,
						() => _data.Enqueue( data )
					);
					break;

				case SMObjectModifyData.ModifyType.Runner:
					_data.Enqueue( data );
					break;
			}

			if ( !_isRunning )	{ Run().Forget(); }
		}

		public void ReRegister( SMObject removeObject ) => _data.RemoveAll(
			d => d._object == removeObject,
			d => removeObject._top._modifyler.Register( d )
		);

		public void Unregister( SMObject removeObject ) => _data.RemoveAll(
			d => d._object == removeObject,
			d => d.Cancel()
		);


		async UniTask Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( !_data.IsEmpty() ) {
				var d = _data.Dequeue();
				await d.Run();
			}
			_isRunning = false;
		}

		public UniTask WaitRunning()
			=> UTask.WaitWhile( _owner._asyncCanceler, () => _isRunning );


		public override string ToString() => string.Join( "\n",
			$"    {this.GetAboutName()}(",
			$"        {nameof( _owner )} : {_owner.ToLineString()}",
			$"        {nameof( _isRunning )} : {_isRunning}",
			$"        {nameof( _data )} :",
			string.Join( "\n", _data.Select( d => $"            {d}" ) ),
			"    )"
		);
	}
}