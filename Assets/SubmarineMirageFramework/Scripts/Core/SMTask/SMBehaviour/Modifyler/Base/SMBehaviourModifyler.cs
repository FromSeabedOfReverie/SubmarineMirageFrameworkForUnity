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


	public class SMBehaviourModifyler : IDisposableExtension {
		public SMBehaviourBody _owner	{ get; private set; }
		readonly LinkedList<SMBehaviourModifyData> _data = new LinkedList<SMBehaviourModifyData>();
		bool _isRunning;
		public bool _isDispose => _disposables._isDispose;
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMBehaviourModifyler( SMBehaviourBody owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				_data.ForEach( d => d.Cancel() );
				_data.Clear();
			} );
		}

		~SMBehaviourModifyler() => Dispose();

		public void Dispose() => _disposables.Dispose();


		public void Register( SMBehaviourModifyData data ) {
			data._owner = this;
			switch( data._type ) {
				case SMBehaviourModifyData.ModifyType.Finalizer:
				case SMBehaviourModifyData.ModifyType.Initializer:
					_data.AddBefore(
						data,
						d => d._type > data._type,
						() => _data.Enqueue( data )
					);
					break;
				case SMBehaviourModifyData.ModifyType.Operator:
					_data.Enqueue( data );
					break;
			}
			if ( !_isRunning )	{ Run().Forget(); }
		}

		public void Unregister( SMBehaviourModifyData remove ) => _data.RemoveAll(
			d => d == remove,
			d => d.Cancel()
		);

		public void UnregisterAll() {
			_data.ForEach( d => d.Cancel() );
			_data.Clear();
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

		public UniTask WaitRunning()
			=> UTask.WaitWhile( _owner._asyncCancelerOnDispose, () => _isRunning );


		public override string ToString() => string.Join( "\n",
			$"    {this.GetAboutName()}(",
			$"        {nameof( _owner )} : {_owner._owner.ToLineString()}",
			$"        {nameof( _isRunning )} : {_isRunning}",
			$"        {nameof( _data )} :",
			string.Join( "\n", _data.Select( d => $"            {d}" ) ),
			"    )"
		);
	}
}