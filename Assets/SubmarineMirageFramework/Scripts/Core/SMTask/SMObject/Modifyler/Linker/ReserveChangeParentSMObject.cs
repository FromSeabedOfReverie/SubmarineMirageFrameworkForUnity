//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using System.Linq;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class ReserveChangeParentSMObject : SMObjectModifyData {
		Transform _parent;
		bool _isWorldPositionStays;



		public ReserveChangeParentSMObject( SMObject smObject, Transform parent, bool isWorldPositionStays )
			: base( smObject )
		{
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;
			_type = ModifyType.Linker;

			if ( !_object._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、親変更不可 :\n{_object}" );
			}
		}

		public override void Cancel() {}



		public override async UniTask Run() {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Run )} : start\n{this}" );
			Log.Debug( string.Join( "\n",
				$"{nameof( _object )} : {_object}",
				$"{nameof( _object._group )} : {_object._group}",
				$"{nameof( _object._group._groups )}",
				string.Join( "\n", _object._group._groups._groups.Select(
					pair => $"    {pair.Key} : {pair.Value?.ToLineString()}"
				) )
			) );
#endif

			// ゲーム物の親を変更
			_object._owner.transform.SetParent( _parent, _isWorldPositionStays );

			// 元グループで子の場合
			if ( !_group.IsTop( _object ) ) {
				UnLinkObject( _object );	// 子の接続解除
				_group.SetAllData();		// 子が抜けた為、元グループを再設定
			}

			// 新親タスクを取得
			var parentObject = _object._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;

			// 新親が居る場合
			if ( parentObject != null ) {
				// 新グループに、子の追加を予約（タスク未走査時に、接続変更しないと破綻する為）
				parentObject._group._modifyler.Register( new ApplyAddChildSMObject( _object, parentObject ) );

				// 元グループで最上親だった場合
				if ( _group.IsTop( _object ) ) {
					parentObject._group.Move( _group );	// グループ全体を、新グループに移植

				// 元グループで子だった場合
				} else {
					// 全子の変更データを、元グループから、新グループに再登録
					_object.GetAllChildren().ForEach( o => o._group = parentObject._group );
					_owner.ReRegister( parentObject._group );
				}

			// 新親が居らず、元グループで子だった場合
			} else if ( !_group.IsTop( _object ) ) {
				// 新グループ作成し、アクティブ変更を予約（親の影響で無効だった場合は、有効化の必要あり）
				var g = new SMGroup( _object );

				g._modifyler.Register( new ChangeActiveSMObject( _object, _object._owner.activeSelf, false ) );
// TODO : ChangeActiveSMObjectより前に、再登録されると、変になるので、上記を割り込みタイプにする

				// 元グループの変更データを、新グループに再登録
				_owner.ReRegister( g );
			}


#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( _object )} : {_object}",
				$"{nameof( parentObject )} : {parentObject}",
				$"{nameof( _object._group )} : {_object._group}",
				$"{nameof( _object._group._groups )} :",
				string.Join( "\n", _object._group._groups._groups.Select(
					pair => $"    {pair.Key} : {pair.Value?.ToLineString()}"
				) )
			) );
			Log.Debug( $"{nameof( Run )} : end\n{this}" );
#endif

			await UTask.DontWait();
		}



		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_parent?.name ?? "null",
				_isWorldPositionStays
			)
			+ ", "
		);
	}
}