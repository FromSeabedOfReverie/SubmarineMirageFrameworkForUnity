//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {
	using System;
	using UnityEngine;
	using Base;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 軸入力の情報クラス
	/// </summary>
	///====================================================================================================
	public class SMAxisInputData : SMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>入力の型</summary>
		[SMShowLine] public readonly SMInputAxis _type;

		/// <summary>軸位置</summary>
		[SMShowLine] public Vector2 _axis	{ get; private set; }
		/// <summary>軸取得イベント</summary>
		Func<Vector2> _getAxisEvent	{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMAxisInputData( SMInputAxis type, Func<Vector2> getAxisEvent ) {
			_type = type;
			_getAxisEvent = getAxisEvent;

			_disposables.AddFirst( () => {
				_axis = Vector2.zero;
				_getAxisEvent = null;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Update() {
			_axis = _getAxisEvent();
		}
	}
}