//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Group.Modifyler {
	using System;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Behaviour;
	using Object;
	using Object.Modifyler;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SendChangeParentObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;
		[SMShowLine] Transform _parent	{ get; set; }
		[SMShowLine] bool _isWorldPositionStays	{ get; set; }



		public SendChangeParentObjectSMGroup( SMObject target, Transform parent, bool isWorldPositionStays )
			: base( target )
		{
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;

			if ( !_target._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}未所持の為、親変更不可 :\n{_target}" );
			}
		}


		public override async UniTask Run() {
			// ゲーム物の親を変更、タスクを取得
			_target._owner.transform.SetParent( _parent, _isWorldPositionStays );
			var parentObject = _target._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;

			ShowLog( parentObject, true );

			// 親が既に同じ場合
			if ( _target._parent == parentObject ) {
				ChangeTransformOnly();	// null一致も含む
				ShowLog( parentObject, false );
				return;
			}
			// トップになる場合
			if ( parentObject == null ) {
				ChangeToTop();	// null一致でない為、_targetは子である
				ShowLog( parentObject, false );
				return;
			}
			// 同じグループ内で、変更の場合
			if ( _owner == parentObject._group ) {
				ChangeParentOnly( parentObject );
				ShowLog( parentObject, false );
				return;
			}
			// トップから子に、変更の場合
			if ( _owner.IsTop( _target ) ) {
				ChangeGroupFromTop( parentObject );
				ShowLog( parentObject, false );
				return;
			}

			// 子から子に、グループ変更
			ChangeGroupFromChild( parentObject );
			ShowLog( parentObject, false );

			await UTask.DontWait();
		}


		void ChangeTransformOnly() {
			// null一致の場合、トップからトップへの変更だが、Transformが異なる可能性がある
			// 同じ親へ変更する場合も、間のTransformが異なる可能性がある

			// Transform変更を考慮し、親に合わせた活動変更を予約
			_modifyler.Register( new ChangeParentActiveObjectSMGroup( _target ) );
		}


		void ChangeToTop() {
			// 元々子だった

			// 子を解除し、元グループを再設定
			SMObjectApplyer.Unlink( _target );
			SMGroupApplyer.SetAllData( _owner );

			// 新グループ作成し、親に合わせた活動変更を予約
			var g = new SMGroup( _target );
			g._ranState = _owner._ranState;
			g._activeState = _owner._activeState;
			g._isFinalizing = _owner._isFinalizing;
			g._modifyler.Register( new ChangeParentActiveObjectSMGroup( _target ) );
			_modifyler.Reregister( g );	// 元グループの変更を、新グループに再登録
		}


		void ChangeParentOnly( SMObject parent ) {
			// 元々トップだった場合、子にトップを付ける為、破綻
			if ( _owner.IsTop( _target ) ) {
				throw new NotSupportedException( $"子にトップを接続する為、親変更不可 :\n{_target}" );
			}

			// 元々子だった

			// 子を解除、新親と接続
			SMObjectApplyer.Unlink( _target );
			SMObjectApplyer.LinkChild( parent, _target );

			// 親に合わせた活動変更を予約
			_modifyler.Register( new ChangeParentActiveObjectSMGroup( _target ) );
		}


		void ChangeGroupFromTop( SMObject parent ) {
			// 元々トップだった

			// 新グループに、子の追加を予約
			parent._group._modifyler.Register( new ReceiveChangeParentObjectSMGroup( _target, parent ) );
			SMGroupApplyer.Move( parent._group, _owner );	// グループ全体を、新グループに移植
		}


		void ChangeGroupFromChild( SMObject parent ) {
			// 元々子だった

			// 子を解除し、元グループを再設定
			SMObjectApplyer.Unlink( _target );
			SMGroupApplyer.SetAllData( _owner );

			// 新グループに、子の追加を予約
			parent._group._modifyler.Register( new ReceiveChangeParentObjectSMGroup( _target, parent ) );

			// 全子の変更データを、元グループから、新グループに再登録
			_target.GetAllChildren().ForEach( o => o._group = parent._group );
			_modifyler.Reregister( parent._group );
		}



		void ShowLog( SMObject parent, bool isStart ) {
#if TestGroupModifyler
			if ( isStart )	{ SMLog.Debug( $"{nameof( Run )} : start\n{this}" ); }
			SMLog.Debug( string.Join( "\n",
				$"{nameof( _target )} : {_target}",
				$"{nameof( parent )} : {parent}",
				$"{nameof( _target._group )} : {_target._group}",
				$"{nameof( _target._group._groups )} : {_target._group._groups}"
			) );
			if ( !isStart )	{ SMLog.Debug( $"{nameof( Run )} : end\n{this}" ); }
#endif
		}
	}
}