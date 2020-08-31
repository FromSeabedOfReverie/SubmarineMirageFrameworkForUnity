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
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class ChangeParentSMObject : SMObjectModifyData {
		public override ModifyType _type => ModifyType.Linker;
		Transform _parent;
		bool _isWorldPositionStays;



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
			_object._owner.transform.SetParent( _parent, _isWorldPositionStays );

			UnLinkObject( _object );
			if ( !_object._isTop )	{ SetAllObjectData( _object._top ); }

			var parent = _object._owner.GetComponentInParentUntilOneHierarchy<SMMonoBehaviour>( true )
				?._object;
			if ( parent != null )	{ AddChildObject( parent, _object ); }
			else					{ RegisterObject(); }

			var lastTop = _object._top;
			SetTopObject( _object );

// TODO : 親も子も、順番を保ったまま、移動しないといけない
// TODO : ChangeActiveSMObjectを待機中に、移動後のトップがどんどん次を実行して、待機されない
			lastTop._modifyler.ReRegister( _object );

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