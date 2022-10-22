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
	/// ■ ジングル音の管理クラス
	///		雰囲気脚色の為の音を、単発再生させる為に、使用する。
	/// </summary>
	///====================================================================================================
	public class SMJingleSource<T> : BaseSMAudioSource<T> where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アセット名の接頭辞（加算用）</summary>
		protected override string _prefixName		=> "";
		/// <summary>階層名</summary>
		protected override string _path				=> "Jingle";
		/// <summary>再生後の音量上昇期間</summary>
		protected override float _fadeInDuration	=> 0.25f;
		/// <summary>停止後の音量下降期間</summary>
		protected override float _fadeOutDuration	=> 0.5f;
		/// <summary>1ファイルのみ読み込むか？</summary>
		protected override bool _isLoadSingle		=> true;

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMJingleSource( SMAudioManager manager ) : base( manager ) {
		}
	}
}