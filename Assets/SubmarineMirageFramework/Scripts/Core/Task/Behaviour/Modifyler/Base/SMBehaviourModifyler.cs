//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public class SMBehaviourModifyler : SMStandardBase {
		public SMBehaviourBody _owner	{ get; private set; }
		readonly LinkedList<SMBehaviourModifyData> _data = new LinkedList<SMBehaviourModifyData>();
		bool _isRunning	{ get; set; }


		public SMBehaviourModifyler( SMBehaviourBody owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				_data.ForEach( d => d.Cancel() );
				_data.Clear();
			} );
		}

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


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _owner._owner.ToLineString() );
			_toStringer.SetValue( nameof( _data ), i => "\n" + string.Join( ",\n",
				_data.Select( d => d.ToLineString( i + 1 ) )
			) );
		}
	}
}