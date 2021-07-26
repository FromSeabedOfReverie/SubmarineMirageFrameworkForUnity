//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {
	using System;
	using Base;
	using Event;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ スワイプ入力の情報クラス
	/// </summary>
	///====================================================================================================
	public class SMSwipeInputData : SMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>入力の型</summary>
		[SMShowLine] public readonly SMInputSwipe _type;

		/// <summary>有効時か？</summary>
		[SMShow] public bool _isEnabled	{ get; private set; }
		/// <summary>有効時か？判定イベント</summary>
		Func<bool> _isCheckEnabledEvent	{ get; set; }
		/// <summary>有効時イベント</summary>
		public readonly SMSubject _enabledEvent = new SMSubject();
		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMSwipeInputData( SMInputSwipe type, Func<bool> isCheckEnabledEvent ) {
			_type = type;
			_isCheckEnabledEvent = isCheckEnabledEvent;

			_disposables.AddFirst( () => {
				_isEnabled = false;
				_isCheckEnabledEvent = null;
				_enabledEvent.Dispose();
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Update() {
			_isEnabled = _isCheckEnabledEvent();
			if ( _isEnabled )	{ _enabledEvent.Run(); }
		}
	}
}