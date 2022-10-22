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
	/// ■ 効果音の管理クラス
	///		ゲームオブジェクトが立てる短い効果音を、単発再生させる為に、使用する。
	/// </summary>
	///====================================================================================================
	public class SMSESource<T> : BaseSMAudioSource<T> where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アセット名の接頭辞（加算用）</summary>
		protected override string _prefixName	=> "";
		/// <summary>階層名</summary>
		protected override string _path			=> "SE";
		/// <summary>重複再生するか？</summary>
		protected override bool _isOverlapPlay	=> true;
		/// <summary>間引き再生するか？</summary>
		protected override bool _isPlayOneShot	=> true;

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMSESource( SMAudioManager manager ) : base( manager ) {
		}
	}
}