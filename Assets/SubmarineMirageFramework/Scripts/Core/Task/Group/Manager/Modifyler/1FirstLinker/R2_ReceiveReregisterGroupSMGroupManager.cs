//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class ReceiveReregisterGroupSMGroupManager : SMGroupManagerModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstLinker;


		public ReceiveReregisterGroupSMGroupManager( SMGroupBody target ) : base( target ) {
			if ( !_target._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMBehaviour )}は、階層変更不可 :\n{_target}" );
			}
		}

		protected override void Cancel() {
			_target.DisposeAllObjects();
			_target._gameObject.Destroy();
		}


		public override async UniTask Run() {
			_owner.Link( _target );
			_target.RegisterRunEventToOwner();

			await UTask.DontWait();
		}
	}
}