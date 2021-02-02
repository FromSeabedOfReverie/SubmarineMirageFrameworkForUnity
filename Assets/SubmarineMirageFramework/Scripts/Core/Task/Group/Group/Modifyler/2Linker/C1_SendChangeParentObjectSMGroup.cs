//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestGroupModifyler
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task.Base;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SendChangeParentObjectSMGroup : SMGroupModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;
		[SMShowLine] Transform _parentTransform	{ get; set; }
		[SMShowLine] bool _isWorldPositionStays	{ get; set; }



		public SendChangeParentObjectSMGroup( SMObjectBody target, Transform parentTransform,
												bool isWorldPositionStays
		) : base( target )
		{
			_parentTransform = parentTransform;
			_isWorldPositionStays = isWorldPositionStays;

			if ( !_target._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}未所持の為、親変更不可 :\n{_target}" );
			}
		}


		public override async UniTask Run() {
			if ( _owner._isFinalizing )	{ return; }


			// ゲーム物の親を変更、タスクを取得
			_target._gameObject.transform.SetParent( _parentTransform, _isWorldPositionStays );
			var parent = _target._gameObject.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object._body;

			ShowLog( parent, true );

			// 親が既に同じ場合
			if ( _target._parent == parent ) {
				// 両方共、親が存在する場合
				if ( _target._parent != null ) {
					ChangeTransformOnly();
					ShowLog( parent, false );
					return;
				// 両方共、親が存在しない場合
				} else {
					ChangeTransformAndScene();
					ShowLog( parent, false );
					return;
				}
			}
			// トップになる場合
			if ( parent == null ) {
				ChangeToTop();	// null一致でない為、_targetは子である
				ShowLog( parent, false );
				return;
			}
			// 同じグループ内で、変更の場合
			if ( _owner == parent._groupBody ) {
				ChangeParentOnly( parent );
				ShowLog( parent, false );
				return;
			}
			// トップから子に、変更の場合
			if ( _owner.IsTop( _target ) ) {
				ChangeGroupFromTop( parent );
				ShowLog( parent, false );
				return;
			}

			// 子から子に、グループ変更
			ChangeGroupFromChild( parent );
			ShowLog( parent, false );

			await UTask.DontWait();
		}


		void ChangeTransformOnly() {
			// 元々子だった、新親は同じ
			// 同じ親へ変更する場合、間のTransformが異なる可能性がある

			// Transform変更を考慮し、親に合わせた活動変更を予約
			_modifyler.Register( new AdjustObjectRunSMGroup( _target ) );
		}


		void ChangeTransformAndScene() {
			// 元々トップだった、新親は居ない
			// トップからトップへの変更でも、親のTransform（SMObject無し）が異なる可能性がある

			// Transform変更を考慮し、親に合わせた活動変更を予約
			_modifyler.Register( new AdjustObjectRunSMGroup( _target ) );

			// シーンが異なる場合、再設定し、内部で管理者を変更
			if ( _owner._managerBody._scene._rawScene != _target._gameObject.scene ) {
				_owner.SetAllData();
			}
		}


		void ChangeToTop() {
			// 元々子だった、新親は居ない

			// 子を解除し、元グループを再設定
			_target.Unlink();
			_owner.SetAllData();

			// 新グループ作成し、親に合わせた活動変更を予約
			var group = new SMGroup( _target );
			var groupBody = group._body;
			groupBody._ranState = _owner._ranState;
			groupBody._activeState = _owner._activeState;
			groupBody._isFinalizing = _owner._isFinalizing;
			groupBody._modifyler.Register( new AdjustObjectRunSMGroup( _target ) );
			_modifyler.Reregister( groupBody );	// 元グループの変更を、新グループに再登録
		}


		void ChangeParentOnly( SMObjectBody parent ) {
			//  元々トップだった、新親は居る
			// 元々トップだった場合、子にトップを付ける為、破綻
			if ( _owner.IsTop( _target ) ) {
				throw new NotSupportedException( $"子にトップを接続する為、親変更不可 :\n{_target}" );
			}

			// 元々子だった、新親は居る
			// 子を解除、新親と接続
			_target.Unlink();
			parent.LinkChild( _target );

			// 親に合わせた活動変更を予約
			_modifyler.Register( new AdjustObjectRunSMGroup( _target ) );
		}


		void ChangeGroupFromTop( SMObjectBody parent ) {
			// 元々トップだった、新親は居る

			// 新グループに、子の追加を予約
			parent._groupBody._modifyler.Register( new ReceiveChangeParentObjectSMGroup( _target, parent ) );

			// グループ全体を、新グループに移植
			parent._groupBody._modifyler.Move( _owner._modifyler );
			_owner._objectBody.GetAllChildren().ForEach( o => o._groupBody = parent._groupBody );
			_owner._group.Dispose();
			// 解放したグループの、登録解除を予約
			_owner._managerBody._modifyler.Register( new UnregisterGroupSMGroupManager( _owner ) );
		}


		void ChangeGroupFromChild( SMObjectBody parent ) {
			// 元々子だった、新親は居る

			// 子を解除し、元グループを再設定
			_target.Unlink();
			_owner.SetAllData();

			// 新グループに、子の追加を予約
			parent._groupBody._modifyler.Register( new ReceiveChangeParentObjectSMGroup( _target, parent ) );

			// 全子の変更データを、元グループから、新グループに再登録
			_target.GetAllChildren().ForEach( o => o._groupBody = parent._groupBody );
			_modifyler.Reregister( parent._groupBody );
		}



		void ShowLog( SMObjectBody parent, bool isStart ) {
#if TestGroupModifyler
			if ( isStart )	{ SMLog.Debug( $"{nameof( Run )} : start\n{this}" ); }
			SMLog.Debug( string.Join( "\n",
				$"{nameof( _target )} : {_target}",
				$"{nameof( parent )} : {parent}",
				$"{nameof( _target._groupBody )} : {_target._groupBody}",
				$"{nameof( _target._groupBody._managerBody )} : {_target._groupBody._managerBody}"
			) );
			if ( !isStart )	{ SMLog.Debug( $"{nameof( Run )} : end\n{this}" ); }
#endif
		}
	}
}