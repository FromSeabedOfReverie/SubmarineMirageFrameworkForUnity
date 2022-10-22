//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using UnityEngine;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ 軸入力のモデルクラス
	/// </summary>
	///====================================================================================================
	public class SMAxisInputModel : BaseSMInputModel<SMInputAxis> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>軸位置</summary>
		[SMShowLine] public Vector2 _axis	{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMAxisInputModel( SMInputAxis type, Func<Vector2> getAxisEvent )
			: base( type )
		{
			_updateEvent.AddLast().Subscribe( _ => {
				_axis = getAxisEvent.Invoke();
			} );

			_disposables.AddFirst( () => {
				_axis = Vector2.zero;
			} );
		}
	}
}