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
	using UTask;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class ChangeParentSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;
		Transform _parent;
		bool _isWorldPositionStays;
		LockSMObject _lockData;



		public ChangeParentSMObject( SMObject smObject, Transform parent, bool isWorldPositionStays )
			: base( smObject )
		{
			if ( _object._owner == null ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、親変更不可 :\n{_object}" );
			}
			_parent = parent;
			_isWorldPositionStays = isWorldPositionStays;
		}

		public override void Cancel() {}



		public override async UniTask Run() {
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Run )} : start\n{this}" );
			var lastTopDebug = _object._top;
			Log.Debug( string.Join( "\n",
				$"{nameof( _object )} : {_object}",
				$"{nameof( lastTopDebug )} : {lastTopDebug}",
				$"lastObjects :",
				string.Join( "\n", lastTopDebug._objects._objects.Select( pair =>
					$"    {pair.Key} : {pair.Value?.ToLineString()}" ) )
			) );
#endif

// TODO : LockSMObjectだと、同時に複数の子供を追加する際に、最初の子を追加後に、解除してしまう
/*
			var parentObject = (
				_parent.GetComponent<SMMonoBehaviour>() ??
				_parent.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
			)?._object;
			_lockData = new LockSMObject( parentObject );
			parentObject._top._modifyler.Register( _lockData );
			await UTask.WaitWhile( _object._asyncCanceler, () => !_lockData._isRunning && !_lockData._isCancel );
*/


			_object._owner.transform.SetParent( _parent, _isWorldPositionStays );

			var lastGroup = _object._group;
			var isLastTop = lastGroup.IsTop( _object );
			UnLinkObject( _object );
			if ( !isLastTop )	{ lastGroup.SetAllData(); }

			var parent = _object._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;
			if ( parent != null ) {
				AddChildObject( parent, _object );
// TODO : 下記で、元のGroupが解放されるので、Dispose削除されないようにする
				parent._group.SetAllData();
			} else if ( !isLastTop ) {
				_object._group = new SMObjectGroup( _object );
			}

// TODO : 親も子も、順番を保ったまま、移動させる（特に孫とか未対応）
// TODO : ChangeActiveSMObjectを待機中に、移動後のトップがどんどん次を実行して、待機されない
			lastGroup._modifyler.ReRegister( _object );

// TODO : 非活動親の子が、独立した親になった時に活動状態にしない、抜けがある
			if ( _object._parent != null && _object._owner.activeSelf ) {
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{nameof( ChangeActiveSMObject )} : start",
					$"isActive : {_object._owner.activeSelf}",
					$"isParentActive : {_object._parent._owner.activeInHierarchy}",
					$"{nameof( _object )} : {_object}"
				) );
#endif
				var isParentActive = _object._parent._owner.activeInHierarchy;
				await new ChangeActiveSMObject( _object, isParentActive, false ).Run();
#if TestSMTaskModifyler
				Log.Debug( string.Join( "\n",
					$"{nameof( ChangeActiveSMObject )} : end",
					$"isActive : {_object._owner.activeSelf}",
					$"isParentActive : {_object._parent._owner.activeInHierarchy}",
					$"{nameof( _object )} : {_object}"
				) );
#endif
			}



//			_lockData.Cancel();



#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( _object )} : {_object}",
				$"{nameof( parent )} : {parent}",
				$"{nameof( lastTopDebug )} : {lastTopDebug}",
				$"{nameof( _object._top )} : {_object._top}",
				$"lastObjects :",
				string.Join( "\n", lastTopDebug._objects._objects.Select( pair =>
					$"    {pair.Key} : {pair.Value?.ToLineString()}" ) ),
				$"{nameof( _object._objects )} :",
				string.Join( "\n", _object._objects._objects.Select( pair =>
					$"    {pair.Key} : {pair.Value?.ToLineString()}" ) )
			) );
			Log.Debug( $"{nameof( Run )} : end\n{this}" );
#endif
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