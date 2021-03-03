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
	using Task.Base;
	using Task.Modifyler.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SendChangeParentObjectSMGroup : SMGroupModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Linker;
		[SMShowLine] Transform _parentTransform	{ get; set; }
		[SMShowLine] bool _isWorldPositionStays	{ get; set; }



		public SendChangeParentObjectSMGroup( SMObjectBody target, Transform parentTransform,
												bool isWorldPositionStays
		) : base( target )
		{
			_parentTransform = parentTransform;
			_isWorldPositionStays = isWorldPositionStays;

			if ( _parentTransform == null ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"親解除は未対応 : {nameof( _parentTransform )} == null",
					$"{nameof( ReleaseParentObjectSMGroup )}を使用する"
				) );
			}
		}


		public override async UniTask Run() {
			if ( _target._isFinalizing )	{ return; }


			// ゲーム物の親を変更
			_target._gameObject.transform.SetParent( _parentTransform, _isWorldPositionStays );
			// 親タスクを取得
			var newParent = _target._gameObject.GetComponentInParentUntilOneHierarchy<SMBehaviour>( true )
				?._object._body;
			ShowLog( newParent, true );


			// 過去親 == 新親の場合
			if ( _target._parent == newParent ) {
				// 過去親と新親が、存在する場合
				if ( _target._parent != null ) {
					ChangeTransformOnly();
					ShowLog( newParent, false );
					return;
				// 過去親と新親が、存在しない場合
				} else {
					ChangeTransformAndScene();
					ShowLog( newParent, false );
					return;
				}
			}
			// トップになる場合
			if ( newParent == null ) {
				ChangeToTop();	// null一致でない為、_targetは子である
				ShowLog( newParent, false );
				return;
			}
			// 同じグループ内で、変更の場合
			if ( _target == newParent._groupBody ) {
				ChangeParentOnly( newParent );
				ShowLog( newParent, false );
				return;
			}
			// トップから子に、変更の場合
			if ( _target.IsTop( _target ) ) {
				ChangeGroupFromTop( newParent );
				ShowLog( newParent, false );
				return;
			}

			// 子から子に、グループ変更
			ChangeGroupFromChild( newParent );
			ShowLog( newParent, false );

			await UTask.DontWait();
		}


		void ChangeTransformOnly() {
			// 子 → 子
			// 新親は同じ（間のTransformが異なる可能性あり）

			// Transform変更を考慮し、親アクティブに一致させる変更を予約
			_modifyler.Register( new AdjustObjectRunSMGroup( _target ) );
		}


		void ChangeTransformAndScene() {
			// トップ → トップ（親Transformあり）
			// 新親なし（親Transformあり）

			// Transform変更を考慮し、親アクティブに一致させる変更を予約
			_modifyler.Register( new AdjustObjectRunSMGroup( _target ) );

			// シーンが異なる場合
			var lastManager = _target._managerBody;
			var lastScene = lastManager._scene._rawScene;
			var newScene = _parentTransform.gameObject.scene;
			if ( lastScene != newScene ) {
				var newManager = lastManager._scene._owner._body.GetScene( newScene )._groupManagerBody;

				// 管理者を再設定
				_target._managerBody = newManager;
				lastManager._modifyler.Register( new SendReregisterGroupSMGroupManager( _target ) );
			}
		}


		void ChangeToTop() {
			// 子 → トップ（親Transformあり）

			// 新グループ作成
			var newScene = _parentTransform.gameObject.scene;
			var newManager = _target._managerBody._scene._owner._body.GetScene( newScene )._groupManagerBody;
			new SMGroup( newManager, _target );
		}


		void ChangeParentOnly( SMObjectBody parent ) {
			// 子 → 子
			// 新親あり（グループは同じ）

			// トップ → 子の場合、同じグループのトップが子にくっつく為、循環参照で破綻
			if ( _target.IsTop( _target ) ) {
				throw new InvalidOperationException(
					$"同一グループ内でトップが子に接続する為、循環参照となり親変更不可 :\n{_target}" );
			}

			// 親を解除、新親と接続
			_target.Unlink();
			parent.LinkChild( _target );

			// 親に合わせた活動変更を予約
			_modifyler.Register( new AdjustObjectRunSMGroup( _target ) );
		}


		void ChangeGroupFromTop( SMObjectBody parent ) {
			// トップ → 子
			// 新親あり

			// 新グループに、子の追加を予約
			parent._groupBody._modifyler.Register( new ReceiveChangeParentObjectSMGroup( _target, parent ) );

			// グループ全体を、新グループに移植
			parent._groupBody._modifyler.Move( _target._modifyler );
			_target._objectBody.SetGroupOfAllChildren( parent._groupBody );

			// グループ解放、登録解除を予約
			_target._group.Dispose();
			_target._managerBody._modifyler.Register( new UnregisterGroupSMGroupManager( _target ) );
		}


		void ChangeGroupFromChild( SMObjectBody parent ) {
			// 子 → 子
			// 新親あり

			// 親を解除
			_target.Unlink();

			// 新グループに、子の追加を予約
			parent._groupBody._modifyler.Register( new ReceiveChangeParentObjectSMGroup( _target, parent ) );

			// 全子の変更データを、元グループから、新グループに再登録
			_target.SetGroupOfAllChildren( parent._groupBody );
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