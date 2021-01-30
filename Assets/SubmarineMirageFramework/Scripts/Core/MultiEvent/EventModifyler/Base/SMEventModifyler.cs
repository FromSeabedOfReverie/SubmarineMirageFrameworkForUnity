//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMEventModifyler<T> : SMRawBase {
		[SMHide] BaseSMMultiEvent<T> _owner	{ get; set; }
		readonly LinkedList< SMEventModifyData<T> > _data = new LinkedList< SMEventModifyData<T> >();
		[SMShowLine] bool _isRunning	{ get; set; }


		public SMEventModifyler( BaseSMMultiEvent<T> owner ) {
			_owner = owner;

			_disposables.Add( () => {
				_data
					.Where( data => data._function != null )
					.ForEach( data => _owner.OnRemove( data._function ) );
				_data.Clear();
			} );
		}


		public void Register( SMEventModifyData<T> data ) {
			_owner.CheckDisposeError( data );
			data._owner = _owner;
			_data.Enqueue( data );

			Run();
		}


		void Run() {
			if ( _isRunning )	{ return; }

			_isRunning = true;
			while ( !_data.IsEmpty() ) {
				var d = _data.Dequeue();
				d.Run();
			}
			_isRunning = false;
		}


		public override string ToString( int indent ) {
			var nameI = StringSMUtility.IndentSpace( indent );
			var memberI = StringSMUtility.IndentSpace( indent + 1 );

			return string.Join( "\n",
				$"{nameI}{this.GetAboutName()}(",
				$"{memberI}{nameof( _isRunning )} : {_isRunning},",
				$"{memberI}{nameof( _data )} :",
				string.Join( ",\n", _data.Select( d =>
					d.ToLineString( indent + 2 )
				) ),
				$"{nameI})"
			);
		}
	}
}