//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Audio {
	using System;
	///====================================================================================================
	/// <summary>
	/// ■ 環境音の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		環境音を、繰り返し再生させる為に、使用する。
	/// </summary>
	///====================================================================================================
	public class BGSSource<T> : BaseAudioSource<T> where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アセット名の接頭辞（加算用）</summary>
		protected override string _prefixName		=> "";
		/// <summary>階層名</summary>
		protected override string _path				=> "BGS";
		/// <summary>再生後の音量上昇期間</summary>
		protected override float _fadeInDuration	=> 2.5f;
		/// <summary>停止後の音量下降期間</summary>
		protected override float _fadeOutDuration	=> 2.5f;
		/// <summary>1ファイルのみ読み込むか？</summary>
		protected override bool _isLoadSingle		=> true;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public BGSSource( IAudioManager manager ) : base( manager ) {
			_source.loop = true;	// ループ再生を行う
		}
	}
}