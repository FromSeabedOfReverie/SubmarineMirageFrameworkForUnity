//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Debug;
	using UnityObject = UnityEngine.Object;


	// TODO : コメント追加、整頓


	public class UnregisterSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;


		public UnregisterSMObject( SMObject smObject ) : base( smObject ) {
			if ( !_object._isTop ) {
				throw new NotSupportedException(
					$"最上階の{nameof( SMObject )}で無い為、登録解除不可 :\n{_object}" );
			}
		}

		public override void Cancel() {}


		public override async UniTask Run() {
			_object.Dispose();

			if ( _object._owner != null ) {
				UnityObject.Destroy( _object._owner );
#if TestSMTaskModifyler
				Log.Debug( $"{nameof( GameObject )}を削除 : {this}" );
			} else {
				Log.Debug( $"{nameof( GameObject )}は無い為、未削除 : {this}" );
#endif
			}

			await UTask.DontWait();
		}
	}
}