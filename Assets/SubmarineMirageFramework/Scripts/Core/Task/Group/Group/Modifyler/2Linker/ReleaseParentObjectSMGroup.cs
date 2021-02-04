//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class ReleaseParentObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;
		[SMShowLine] bool _isWorldPositionStays	{ get; set; }



		public ReleaseParentObjectSMGroup( SMObjectBody target, bool isWorldPositionStays ) : base( target ) {
			_isWorldPositionStays = isWorldPositionStays;
		}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }


			// ゲーム物の親を解除
			_target._gameObject.transform.SetParent( null, _isWorldPositionStays );

			// トップの場合、未処理
			if ( _owner.IsTop( _target ) )	{ return; }

			// 新グループ作成
			new SMGroup( _owner._managerBody, _target );


			await UTask.DontWait();
		}
	}
}