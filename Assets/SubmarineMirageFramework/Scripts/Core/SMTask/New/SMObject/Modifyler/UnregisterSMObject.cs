//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UnityEngine;
	using UniRx.Async;
	using Utility;


	// TODO : コメント追加、整頓


	public class UnregisterSMObject : SMObjectModifyData {
		public UnregisterSMObject( SMObject smObject ) : base( smObject ) {}


		protected override async UniTask Run() {
			_owner.Get( _object._type )
				.Remove( _object );
			_object.Dispose();
			if ( _object._owner != null )	{ Object.Destroy( _object._owner ); }

			await UniTaskUtility.DontWait();
		}
	}
}