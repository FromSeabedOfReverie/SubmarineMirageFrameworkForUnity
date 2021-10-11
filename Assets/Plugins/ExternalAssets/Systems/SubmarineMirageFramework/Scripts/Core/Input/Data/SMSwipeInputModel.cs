//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ スワイプ入力のモデルクラス
	/// </summary>
	///====================================================================================================
	public class SMSwipeInputModel : BaseSMInputModel<SMInputSwipe> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>有効時か？</summary>
		[SMShowLine] public bool _isEnabled	{ get; private set; }
		/// <summary>有効時イベント</summary>
		public readonly SMSubject _enabledEvent = new SMSubject();

		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMSwipeInputModel( SMInputSwipe type, Func<bool> isCheckEnabledEvent )
			: base( type )
		{
			_updateEvent.AddLast().Subscribe( _ => {
				_isEnabled = isCheckEnabledEvent.Invoke();
				if ( _isEnabled )	{ _enabledEvent.Run(); }
			} );

			_disposables.AddFirst( () => {
				_isEnabled = false;
				_enabledEvent.Dispose();
			} );
		}
	}
}