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
	using Base;
	using UTask;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public class SMGroupModifyler : SMStandardBase {
		public SMGroup _owner	{ get; private set; }
		readonly LinkedList<SMObjectModifyData> _data = new LinkedList<SMObjectModifyData>();
		bool _isRunning;


		public SMGroupModifyler( SMGroup owner ) {
			_owner = owner;

			_disposables.AddLast( () => {
				_data.ForEach( d => d.Cancel() );
				_data.Clear();
			} );
		}



		public void Move( SMGroupModifyler remove ) {
			remove._data.ForEach( d => Register( d ) );
			remove._data.Clear();
			remove.Dispose();
		}

		public void Register( SMObjectModifyData data ) {
			data._owner = this;
			switch( data._type ) {
				case SMObjectModifyData.ModifyType.Interrupter:
				case SMObjectModifyData.ModifyType.Linker:
					_data.AddBefore(
						data,
						d => d._type > data._type,
						() => _data.Enqueue( data )
					);
					break;
				case SMObjectModifyData.ModifyType.Runner:
					_data.Enqueue( data );
					break;
			}
			if ( !_isRunning )	{ Run().Forget(); }
		}

		public void ReRegister( SMGroup register ) => _data.RemoveAll(
			d => d._object._group == register,
			d => register._modifyler.Register( d )
		);

		public void Unregister( SMObject remove ) => _data.RemoveAll(
			d => d._object == remove,
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
			=> UTask.WaitWhile( _owner._topObject._asyncCanceler, () => _isRunning );


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _owner.ToLineString() );
			_toStringer.SetValue( nameof( _data ), i => "\n" + string.Join( ",\n",
				_data.Select( d => $"{StringSMUtility.IndentSpace( i )}{d}" )
			) );
		}
	}
}