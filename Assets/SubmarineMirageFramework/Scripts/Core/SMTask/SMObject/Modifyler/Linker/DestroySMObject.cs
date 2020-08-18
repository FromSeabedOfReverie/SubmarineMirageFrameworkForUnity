//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using Extension;


	// TODO : コメント追加、整頓


	public class DestroySMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;


		public DestroySMObject( SMObject smObject ) : base( smObject ) {}

		public override void Cancel() {}


		public override async UniTask Run() {
			var top = _object._top;
			UnLinkObject( _object );
			if ( top != _object )	{ SetAllObjectData( top ); }

			top._modifyler._data.RemoveAll(
				d => d._object == _object,
				d => d.Cancel()
			);

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