//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Utility;
	using Debug;



	public class ReleaseParentObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Linker;
		[SMShowLine] bool _isWorldPositionStays	{ get; set; }



		public ReleaseParentObjectSMGroup( SMObjectBody smObject, bool isWorldPositionStays ) : base( smObject ) {
			_isWorldPositionStays = isWorldPositionStays;
		}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }


			// ゲーム物の親を解除
			_object._gameObject.transform.SetParent( null, _isWorldPositionStays );

			// トップの場合、未処理
			if ( _target.IsTop( _object ) )	{ return; }

			// 新グループ作成
			new SMGroup( _target._managerBody, _object );


			await UTask.DontWait();
		}
	}
}