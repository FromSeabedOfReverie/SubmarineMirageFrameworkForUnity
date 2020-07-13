//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;


	// TODO : コメント追加、整頓


	public class DestroySMObject : SMObjectModifyData {
		public DestroySMObject( SMObject smObject ) : base( smObject ) {}

		public override void Cancel() {}


		public override async UniTask Run() {
			var top = _object._top;
			UnLinkObject( _object );
			if ( top != _object )	{ SetAllObjectData( top ); }

			var topData = top._modifyler._data
				.Where( d => {
					if ( d._object != _object ) {
						return true;
					} else {
						d.Cancel();
						return false;
					}
				} );
			top._modifyler._data = new Queue<SMObjectModifyData>( topData );

			await RunObject();
		}


		async UniTask RunObject() {
			try {
				await new ChangeActiveSMObject( _object, false, true ).Run();
				await new RunStateSMObject( _object, SMTaskRanState.Finalizing ).Run();
			} finally {
				_object.Dispose();
				if ( _object._owner != null )	{ Object.Destroy( _object._owner ); }
			}
		}
	}
}