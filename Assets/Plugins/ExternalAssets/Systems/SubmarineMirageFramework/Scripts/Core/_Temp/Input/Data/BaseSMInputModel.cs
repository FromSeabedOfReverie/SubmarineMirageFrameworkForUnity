//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	///====================================================================================================
	/// <summary>
	/// ■ 入力モデルの基盤クラス
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMInputModel<T> : SMStandardBase where T : Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>入力の型</summary>
		[SMShowLine] public readonly T _type;
		/// <summary>更新イベント</summary>
		public readonly SMSubject _updateEvent = new SMSubject();

		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMInputModel( T type ) {
			_type = type;

			_disposables.AddFirst( () => {
				_updateEvent.Dispose();
			} );
		}
	}
}