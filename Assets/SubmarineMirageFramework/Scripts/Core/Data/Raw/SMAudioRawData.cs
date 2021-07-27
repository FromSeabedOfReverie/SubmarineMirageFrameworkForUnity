//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 音の生情報クラス
	///		参考URL : https://github.com/Veselov-Dmitry/TestSerialize/blob/master/Assets/Scripts/TEST.cs
	///			該当ソースは、参考元のライセンスが適用されます。
	/// </summary>
	///====================================================================================================
	public class SMAudioRawData : BaseSMUnityRawData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>名前</summary>
		public string _name;
		/// <summary>サンプルの長さ</summary>
		public int _samples;
		/// <summary>チャンネル数</summary>
		public int _channels;
		/// <summary>サンプル周波数（ヘルツ）</summary>
		public int _frequency;
		/// <summary>生情報</summary>
		public float[] _data;

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（非シリアル化用）
		/// </summary>
		public SMAudioRawData() {
		}

		/// <summary>
		/// ● コンストラクタ（音情報を設定）
		/// </summary>
		public SMAudioRawData( AudioClip audio ) {
			_name = audio.name;
			_samples = audio.samples;
			_channels = audio.channels;
			_frequency = audio.frequency;
			_data = new float[audio.samples * audio.channels];
			audio.GetData( _data, 0 );
		}

		/// <summary>
		/// ● 解放
		/// </summary>
		public override void Dispose() {
			_name = null;
			_samples = 0;
			_channels = 0;
			_frequency = 0;
			_data = null;
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 音に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AudioClip ToAudioClip() {
			var audio = AudioClip.Create( _name, _samples, _channels, _frequency, false );
			audio.SetData( _data, 0 );
			return audio;
		}
	}
}