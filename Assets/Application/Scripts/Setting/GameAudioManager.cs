//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Audio {
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ ゲーム音の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		音の管理クラスの派生で、プロジェクト固有定義を設定する。
	///		TODO : Unityエディタから、設定可能にする
	/// </summary>
	///====================================================================================================
	public class GameAudioManager : AudioManager<GameAudioManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>音楽の定数（文字列変換で書類名とする）</summary>
		public enum BGM {
			TestTitle,
			TestBattle,
		}
		/// <summary>環境音の定数（文字列変換で書類名とする）</summary>
		public enum BGS {
			TestWater,
			TestWind,
		}
		/// <summary>脚色音の定数（文字列変換で書類名とする）</summary>
		public enum Jingle {
			TestGameClear,
			TestGameOver,
		}
		/// <summary>声音の定数（文字列変換で書類名とする）</summary>
		public enum Voice {
			TestRidicule,
			TestScream,
		}
		/// <summary>繰り返し効果音の定数（文字列変換で書類名とする）</summary>
		public enum LoopSE {
			TestTalk1,
			TestTalk2,
		}
		/// <summary>効果音の定数（文字列変換で書類名とする）</summary>
		public enum SE {
			TestDecision,
			TestGun,
		}

		/// <summary>音楽</summary>
		public BGMSource<BGM> _bgm			{ get; private set; }
		/// <summary>環境音</summary>
		public BGSSource<BGS> _bgs			{ get; private set; }
		/// <summary>ジングル音</summary>
		public JingleSource<Jingle> _jingle	{ get; private set; }
		/// <summary>声音</summary>
		public VoiceSource<Voice> _voice	{ get; private set; }
		/// <summary>効果音（ループ用）</summary>
		public LoopSESource<LoopSE> _loopSE	{ get; private set; }
		/// <summary>効果音</summary>
		public SESource<SE> _se				{ get; private set; }

		/// <summary>音楽音量へのアクセサ</summary>
		public float _bgmVolume {
			get { return _bgm._settingVolume.Value; }
			set { _bgm._settingVolume.Value = value; }
		}
		/// <summary>声音量へのアクセサ</summary>
		public float _voiceVolume {
			get { return _voice._settingVolume.Value; }
			set { _voice._settingVolume.Value = value; }
		}
		/// <summary>効果音量へのアクセサ</summary>
		public float _seVolume {
			get { return _se._settingVolume.Value; }
			set {
				// SE設定は、共通で使用
				_bgs._settingVolume.Value = value;
				_jingle._settingVolume.Value = value;
				_loopSE._settingVolume.Value = value;
				_se._settingVolume.Value = value;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public GameAudioManager() {
			// 各種書類音量を設定
			_defaultVolumes[typeof( BGM )] = new Dictionary<object, float> {
				{ BGM.TestTitle,	1 },
				{ BGM.TestBattle,	0.5f },
			};
			_defaultVolumes[typeof( BGS )] = new Dictionary<object, float> {
				{ BGS.TestWater,	1 },
				{ BGS.TestWind,		1 },
			};
			_defaultVolumes[typeof( Jingle )] = new Dictionary<object, float> {
				{ Jingle.TestGameClear,	0.5f },
				{ Jingle.TestGameOver,	0.5f },
			};
			_defaultVolumes[typeof( Voice )] = new Dictionary<object, float> {
				{ Voice.TestRidicule,	1 },
				{ Voice.TestScream,		0.5f },
			};

			// 各種管理クラスを作成
			_bgm	= new BGMSource<BGM>( this );
			_bgs	= new BGSSource<BGS>( this );
			_jingle	= new JingleSource<Jingle>( this );
			_voice	= new VoiceSource<Voice>( this );
			_loopSE	= new LoopSESource<LoopSE>( this );
			_se		= new SESource<SE>( this );

			// 音一覧を作成
			_audioSources[typeof( BGM )]	= _bgm;
			_audioSources[typeof( BGS )]	= _bgs;
			_audioSources[typeof( Jingle )]	= _jingle;
			_audioSources[typeof( Voice )]	= _voice;
			_audioSources[typeof( LoopSE )]	= _loopSE;
			_audioSources[typeof( SE )]		= _se;
		}
	}
}