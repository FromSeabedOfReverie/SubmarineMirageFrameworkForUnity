//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestAudio
namespace SubmarineMirage.Audio {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UnityEngine;
	using UniRx;
	using KoganeUnityLib;
	using Service;
	using Task;
	using Extension;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 音再生管理のクラス
	///		音楽、ジングル音、効果音（ループ用）、効果音の再生に対応。
	///		各種音の定数等、プロジェクト固有定義を、継承先クラスで行う。
	///		
	///		TODO : Unityエディタから、設定可能にする
	/// </summary>
	///====================================================================================================
	public class SMAudioManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public override SMTaskRunType _type => SMTaskRunType.Sequential;

		/// <summary>音一覧の辞書</summary>
		protected readonly Dictionary<Type, IBaseSMAudioSource> _audioSources;
		/// <summary>各種書類音量の一覧</summary>
		public readonly Dictionary< Type, Dictionary<object, float> > _defaultVolumes
			= new Dictionary< Type, Dictionary<object, float> >();
		/// <summary>音の発生源</summary>
		public GameObject _speaker	{ get; private set; }
		/// <summary>一時停止中か？</summary>
		public bool _isPause		{ get; private set; }

		/// <summary>音楽</summary>
		public SMBGMSource<SMBGM> _bgm
			=> _audioSources.GetOrDefault( typeof( SMBGM ) ) as SMBGMSource<SMBGM>;
		/// <summary>環境音</summary>
		public SMBGSSource<SMBGS> _bgs
			=> _audioSources.GetOrDefault( typeof( SMBGS ) ) as SMBGSSource<SMBGS>;
		/// <summary>ジングル音</summary>
		public SMJingleSource<SMJingle> _jingle
			=> _audioSources.GetOrDefault( typeof( SMJingle ) ) as SMJingleSource<SMJingle>;
		/// <summary>声音</summary>
		public SMVoiceSource<SMVoice> _voice
			=> _audioSources.GetOrDefault( typeof( SMVoice ) ) as SMVoiceSource<SMVoice>;
		/// <summary>効果音（ループ用）</summary>
		public SMLoopSESource<SMLoopSE> _loopSE
			=> _audioSources.GetOrDefault( typeof( SMLoopSE ) ) as SMLoopSESource<SMLoopSE>;
		/// <summary>効果音</summary>
		public SMSESource<SMSE> _se
			=> _audioSources.GetOrDefault( typeof( SMSE ) ) as SMSESource<SMSE>;

		/// <summary>音楽音量へのアクセサ</summary>
		public float _bgmVolume {
			get => _bgm._settingVolume.Value;
			set => _bgm._settingVolume.Value = value;
		}
		public float _bgsVolume {
			get => _bgs._settingVolume.Value;
			set => _bgs._settingVolume.Value = value;
		}
		public float _jingleVolume {
			get => _jingle._settingVolume.Value;
			set => _jingle._settingVolume.Value = value;
		}
		/// <summary>声音量へのアクセサ</summary>
		public float _voiceVolume {
			get => _voice._settingVolume.Value;
			set => _voice._settingVolume.Value = value;
		}
		/// <summary>効果音量へのアクセサ</summary>
		public float _seVolume {
			get => _se._settingVolume.Value;
			set {
				_loopSE._settingVolume.Value = value;
				_se._settingVolume.Value = value;
			}
		}

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMAudioManager() {
			// 音発生用スピーカーを作成し、ゲーム内に配置
			_speaker = new GameObject( "_AudioSpeaker" );
			_speaker.DontDestroyOnLoad();	// シーン切り替え後も保持

			// 一時停止状態を設定
			Observable.EveryApplicationPause().Subscribe( @is => _isPause = @is );

			// 各種管理クラスを作成
			_audioSources = new Dictionary<Type, IBaseSMAudioSource>() {
				{ typeof( SMBGM ),		new SMBGMSource<SMBGM>( this ) },
				{ typeof( SMBGS ),		new SMBGSSource<SMBGS>( this ) },
				{ typeof( SMJingle ),	new SMJingleSource<SMJingle>( this ) },
				{ typeof( SMVoice ),	new SMVoiceSource<SMVoice>( this ) },
				{ typeof( SMLoopSE ),	new SMLoopSESource<SMLoopSE>( this ) },
				{ typeof( SMSE ),		new SMSESource<SMSE>( this ) },
			};

			_defaultVolumes[typeof( SMBGM )] = new Dictionary<object, float> {
				{ SMBGM.Game,		0.3f },
				{ SMBGM.NightGame,	0.5f },
				{ SMBGM.Result,		0.3f },
				{ SMBGM.Title,		0.3f },
			};
			_defaultVolumes[typeof( SMBGS )] = new Dictionary<object, float> {
				{ SMBGS.Daytime,	0.15f },
				{ SMBGS.Evening,	0.15f },
				{ SMBGS.Night,		0.2f },
				{ SMBGS.Wind,		0.5f },
			};

			_disposables.AddFirst( () => {
				_audioSources.ForEach( pair => pair.Value.Dispose() );
				_audioSources.Clear();
				_defaultVolumes.Clear();
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
#if TestAudio
			if ( !SMDebugManager.IS_DEVELOP )	{ return; }

			// デバッグ表示を設定
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
			_lateUpdateEvent.AddFirst().Subscribe( _ => {
				displayLog.Add( Color.cyan );
				displayLog.Add( $"● {this.GetAboutName()}" );
				displayLog.Add( Color.white );
			} );

			_audioSources.ForEach( pair => pair.Value.Setup() );
#endif
		}

		///------------------------------------------------------------------------------------------------
		/// ● 再生、停止
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生
		/// </summary>
		public async UniTask Play<T>( T enumName ) where T : struct, Enum {
			var type = enumName.GetType();
			var source = _audioSources.GetOrDefault( type ) as BaseSMAudioSource<T>;
			if ( source != null ) {
				await source.Play( enumName );
			}
		}

		/// <summary>
		/// ● 停止
		/// </summary>
		public async UniTask Stop<T>() where T : struct, Enum {
			var type = typeof( T );
			var source = _audioSources.GetOrDefault( type );
			if ( source != null ) {
				await source.Stop();
			}
		}

		/// <summary>
		/// ● 全停止
		/// </summary>
		public async UniTask StopAll() {
			await _audioSources
				.Select( pair => pair.Value.Stop() );
		}
	}
}