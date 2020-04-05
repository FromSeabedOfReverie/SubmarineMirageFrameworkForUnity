//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Audio {
	using System;
	///====================================================================================================
	/// <summary>
	/// ■ 声音の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		台詞用の声音を、単発再生させる為に、使用する。
	/// </summary>
	///====================================================================================================
	public class VoiceSource<T> : BaseAudioSource<T> where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アセット名の接頭辞（加算用）</summary>
		protected override string _prefixName		=> "";
		/// <summary>階層名</summary>
		protected override string _path				=> "Voice";
		/// <summary>再生後の音量上昇期間</summary>
		protected override float _fadeInDuration	=> 0;
		/// <summary>停止後の音量下降期間</summary>
		protected override float _fadeOutDuration	=> 0.5f;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public VoiceSource( IAudioManager manager ) : base( manager ) {
		}
	}
}