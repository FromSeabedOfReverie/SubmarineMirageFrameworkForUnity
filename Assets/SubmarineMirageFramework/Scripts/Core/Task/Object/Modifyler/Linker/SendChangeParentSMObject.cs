//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object.Modifyler {
	using System;
	using System.Linq;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Behaviour;
	using Object;
	using Group;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SendChangeParentSMObject : SMObjectModifyData {
		[SMShowLine] Transform _parent	{ get; set; }
		[SMShowLine] bool _isWorldPositionStays	{ get; set; }



		public SendChangeParentSMObject( SMObject target, Transform parent, bool isWorldPositionStays )
			: base( target )
		{
			_type = SMTaskModifyType.Linker;
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;

			if ( !_target._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、親変更不可 :\n{_target}" );
			}
		}

		protected override void Cancel() {}



		public override async UniTask Run() {
#if TestObjectModifyler
			SMLog.Debug( $"{nameof( Run )} : start\n{this}" );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( _target )} : {_target}",
				$"{nameof( _target._group )} : {_target._group}",
				$"{nameof( _target._group._groups )}",
				string.Join( "\n", _target._group._groups._groups.Select(
					pair => $"    {pair.Key} : {pair.Value?.ToLineString()}"
				) )
			) );
#endif

			// ゲーム物の親を変更
			_target._owner.transform.SetParent( _parent, _isWorldPositionStays );

			// 元グループで子の場合
			if ( !_owner.IsTop( _target ) ) {
				SMObjectApplyer.Unlink( _target );	// 子の接続解除
				_owner.SetAllData();				// 子が抜けた為、元グループを再設定
			}

			// 新親タスクを取得
			var parentObject = _target._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;

			// 新親が居る場合
			if ( parentObject != null ) {
				// 新グループに、子の追加を予約（タスク未走査時に、接続変更しないと破綻する為）
				parentObject._group._modifyler.Register( new ReceiveChangeParentSMObject( _target, parentObject ) );

				// 元グループで最上親だった場合
				if ( _owner.IsTop( _target ) ) {
					parentObject._group.Move( _owner );	// グループ全体を、新グループに移植

				// 元グループで子だった場合
				} else {
					// 全子の変更データを、元グループから、新グループに再登録
					_target.GetAllChildren().ForEach( o => o._group = parentObject._group );
					_modifyler.Reregister( parentObject._group );
				}

			// 新親が居らず、元グループで子だった場合
			} else if ( !_owner.IsTop( _target ) ) {
				// 新グループ作成し、アクティブ変更を予約（親の影響で無効だった場合は、有効化の必要あり）
				var g = new SMGroup( _target );

				g._modifyler.Register( new ChangeActiveSMObject( _target, _target._owner.activeSelf, false ) );
// TODO : ChangeActiveSMObjectより前に、再登録されると、変になるので、上記を割り込みタイプにする

				// 元グループの変更データを、新グループに再登録
				_modifyler.Reregister( g );
			}


#if TestObjectModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( _target )} : {_target}",
				$"{nameof( parentObject )} : {parentObject}",
				$"{nameof( _target._group )} : {_target._group}",
				$"{nameof( _target._group._groups )} :",
				string.Join( "\n", _target._group._groups._groups.Select(
					pair => $"    {pair.Key} : {pair.Value?.ToLineString()}"
				) )
			) );
			SMLog.Debug( $"{nameof( Run )} : end\n{this}" );
#endif

			await UTask.DontWait();
		}
	}
}