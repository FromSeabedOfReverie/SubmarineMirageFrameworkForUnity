//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	///====================================================================================================
	/// <summary>
	/// ■ 効果音（ループ用）の管理クラス
	///		ゲームオブジェクトが立てる効果音を、繰り返し再生させる為に、使用する。
	/// </summary>
	///====================================================================================================
	public class SMLoopSESource<T> : BaseSMAudioSource<T> where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アセット名の接頭辞（加算用）</summary>
		protected override string _prefixName	=> "";
		/// <summary>階層名</summary>
		protected override string _path			=> "SE";

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMLoopSESource( SMAudioManager manager ) : base( manager ) {
			_source.loop = true;	// ループ再生を行う
		}
	}
}