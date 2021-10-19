//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestAudio
namespace SubmarineMirage {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using DG.Tweening;
	using DG.Tweening.Core;
	using DG.Tweening.Plugins.Options;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 音源管理の基盤クラス
	///		音楽、ジングル音、効果音（ループ用）、効果音の管理クラスに継承させて使う。
	/// </summary>
	///====================================================================================================
	public abstract class BaseSMAudioSource<T> : SMStandardBase, IBaseSMAudioSource where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>再生状態</summary>
		public SMAudioSourceState _state	{ get; private set; }
		/// <summary>実行中の音名</summary>
		public T? _runningName	{ get; protected set; }
		/// <summary>音の管理者</summary>
		SMAudioManager _manager { get; set; }

		/// <summary>音の発生源</summary>
		protected AudioSource _source { get; set; }
		/// <summary>再生中の音</summary>
		public SMAudioFile<T> _playingAudio	{ get; protected set; }
		/// <summary>音のキャッシュ一覧</summary>
		readonly Dictionary< T, SMAudioFile<T> > _audioCache = new Dictionary< T, SMAudioFile<T> >();

		/// <summary>設定音量</summary>
		public readonly ReactiveProperty<float> _settingVolume = new ReactiveProperty<float>( 1 );
		/// <summary>音比率</summary>
		readonly ReactiveProperty<float> _volumePercent = new ReactiveProperty<float>();

		/// <summary>アセット名の接頭辞（加算用）</summary>
		protected abstract string _prefixName		{ get; }
		/// <summary>階層名</summary>
		protected abstract string _path				{ get; }
		/// <summary>再生後の音量上昇期間</summary>
		protected virtual float _fadeInDuration		=> 0;
		/// <summary>停止後の音量下降期間</summary>
		protected virtual float _fadeOutDuration	=> 0;
		/// <summary>1ファイルのみ読み込むか？</summary>
		protected virtual bool _isLoadSingle		=> false;
		/// <summary>重複再生するか？</summary>
		protected virtual bool _isOverlapPlay		=> false;
		/// <summary>間引き再生するか？</summary>
		protected virtual bool _isPlayOneShot		=> false;

		/// <summary>非同期停止用の識別子</summary>
		readonly SMAsyncCanceler _runningCanceler = new SMAsyncCanceler();

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BaseSMAudioSource( SMAudioManager manager ) {
			// 管理者を登録
			_manager = manager;
			// 音の発生源を作成
			_source = _manager._speaker.AddComponent<AudioSource>();
			_source.playOnAwake = false;

			// 音量を作成（変更通知）
			_settingVolume.Subscribe( _ => ApplyVolume() );
			_volumePercent.Subscribe( _ => ApplyVolume() );


			_disposables.AddFirst( () => {
				_runningCanceler.Dispose();

				Stop().Forget(); // 停止

				_settingVolume.Dispose();
				_volumePercent.Dispose();

				// 音データを全て破棄
				_audioCache.ForEach( pair => pair.Value.Dispose() );
				_audioCache.Clear();
			} );
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public virtual void Setup() {
#if TestAudio
			if ( !SMDebugManager.IS_DEVELOP )	{ return; }

			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
			_manager._lateUpdateEvent.AddLast().Subscribe( _ => {
				// クラス名、音量描画
				displayLog.Add( Color.yellow ); // 見出し色に設定
				displayLog.Add( $"・{this.GetAboutName()} : {_state} : 音量 : {_source.volume:F2}" );
				displayLog.Add( Color.white );	// 基本色に設定

				// キャッシュ中の音を描画
				_audioCache.ForEach( pair => {
					var s = pair.Value._state.ToString();
					var n = pair.Value._audioClip == null ? " : null" : "";
					displayLog.Add( $"\t{s} : {pair.Key}{n}" );
				} );

				// 再生中の音を描画
				if ( !_isPlayOneShot && _playingAudio != null ) {
					var n = _playingAudio._audioClip == null ? " : null" : "";
					displayLog.Add( $"\t再生中 : {_playingAudio._enumName}{n}" );
				}
			} );
#endif
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 音量を適用
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void ApplyVolume() {
			if ( _source == null )	{ return; }

			var fileVolume = _playingAudio != null ? _playingAudio._volume : 1;
			_settingVolume.Value = Mathf.Clamp01( _settingVolume.Value );
			_volumePercent.Value = Mathf.Clamp01( _volumePercent.Value );
			_source.volume = fileVolume * _settingVolume.Value * _volumePercent.Value;
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 音を取得
		/// </summary>
		protected SMAudioFile<T> GetAudio( T enumName )
			=> _audioCache.GetOrDefault( enumName );

		/// <summary>
		/// ● 基本音量を取得
		/// </summary>
		float GetDefaultVolume( T enumName ) {
			var type = enumName.GetType();
			var volumes = _manager._defaultVolumes.GetOrDefault( type );
			return volumes?.GetOrDefault( enumName, 1 ) ?? 1;
		}

		///------------------------------------------------------------------------------------------------
		/// ● 判定
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込状態が等しいか？
		/// </summary>
		public bool IsState( T enumName, SMAudioFileState state ) {
			var audio = GetAudio( enumName );
			return audio != null && audio._state == state;
		}

		/// <summary>
		/// ● 再生中か？
		/// </summary>
		public bool IsPlaying( T enumName ) => (
			!_isPlayOneShot &&
			( _state == SMAudioSourceState.Playing || _state == SMAudioSourceState.Play ) &&
			IsRunningEnum( enumName )
		);

		/// <summary>
		/// ● 音発生源が再生中か？
		/// </summary>
		bool IsPlayingSource()
			// 一時停止中は停止判定になる為、とりあえず再生中とする
			=> ( _source != null && _source.isPlaying ) || _manager._isPause;

		/// <summary>
		/// ● 実行中定数と等しいか？
		/// </summary>
		bool IsRunningEnum( T enumName )
			=> _runningName.HasValue && enumName.Equals( _runningName.Value );

		///------------------------------------------------------------------------------------------------
		/// ● 読込、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public async UniTask< SMAudioFile<T> > Load( T enumName ) {
			// 読込済の場合、キャッシュを返す
			var audio = _audioCache.GetOrDefault( enumName );
			if ( audio != null ) {
				await UTask.WaitWhile( _runningCanceler, () => audio._state == SMAudioFileState.Loading );
				return audio;
			}

			// ファイル名を設定
			var fileName = _prefixName + enumName.ToString();
			// 階層名を設定
			// 空文字の場合、書類名と同じ階層名を使用する（アセットバンドルとの辻褄合わせ）
			var path = _path.IsNullOrEmpty() ? fileName : _path;
			// 音量を設定
			var volume = GetDefaultVolume( enumName );
			// 書類を作成
			audio = new SMAudioFile<T>( fileName, path, enumName, volume );
			_audioCache[enumName] = audio;	// キャッシュ登録

			try {
				// 読込
				await audio.Load();
				return audio;

			} catch {
				// まだ読み込まれていない場合、破棄
				if ( audio._state != SMAudioFileState.Loaded ) {
					Unload( enumName );
				}
				throw;
			}
		}

		/// <summary>
		/// ● 読込解除
		/// </summary>
		public void Unload( T enumName ) {
			// 停止
			StopSource( enumName );
			// 読み込まれている場合、削除
			var audio = GetAudio( enumName );
			if ( audio != null ) {
				audio.Dispose();
				_audioCache.Remove( enumName );
			}
		}

		/// <summary>
		/// ● 停止音の読込解除
		/// </summary>
		void UnloadStopAudio() {
			if ( !_isLoadSingle )	{ return; }

			// 再生中以外の音を削除
			_audioCache
				.Where( pair => !IsRunningEnum( pair.Value._enumName ) )
				.Select( pair => pair.Key )
				.ToArray()	// foreach内の削除によるエラーを防ぐ為、コピー
				.ForEach( name => Unload( name ) );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 再生
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生
		/// </summary>
		public async UniTask Play( T enumName ) {
			// 重複再生せず、指定音が再生中の場合、未処理
			if ( !_isOverlapPlay && IsPlaying( enumName ) )	{ return; }

			_runningCanceler.Cancel();

			_state = SMAudioSourceState.Playing;
			var isEqualRunning = IsRunningEnum( enumName );
			_runningName = enumName;

			// 読込、現在再生音に登録
			var audio = await Load( _runningName.Value );
			// 前回と違う音で、音発生源が再生中の場合
			if ( !isEqualRunning && audio != _playingAudio && IsPlayingSource() ) {
				// 音量フェードアウト
				await FadeVolume( SMAudioSourceFadeType.Out );
			}

			// 再生
			_playingAudio = audio;
			PlaySource();
			UnloadStopAudio();	// 停止中の音を全削除
			// 音量フェードイン
			await FadeVolume( SMAudioSourceFadeType.In );
			// 再生状態へ
			_state = SMAudioSourceState.Play;
			// 再生完了まで待機
			await UTask.WaitUntil( _runningCanceler, () => !IsPlayingSource() );
			// 停止
			if ( _runningName.HasValue ) {
				StopSource( _runningName.Value );
			}
			UnloadStopAudio();	// 停止中の音を全削除
		}

		/// <summary>
		/// ● 再生（AudioSource）
		/// </summary>
		protected void PlaySource() {
			// 前回音と異なる場合、新規適用
			if ( _source.clip != _playingAudio._audioClip ) {
				_source.clip = _playingAudio._audioClip;
			// 前回音と等しく、間引き再生でない場合、未処理
			} else if ( !_isPlayOneShot ) {
				return;
			}

			// フェードがスキップされた可能性を考慮し、初期値を設定
			_volumePercent.Value = 0;
			// 再生
			if ( _isPlayOneShot )	{ _source.PlayOneShot( _source.clip ); }	// 間引き再生
			else					{ _source.Play(); }							// 通常再生

/*
// TODO : 音のイントロ付ループ再生の実装
// 参考URL : http://kan-kikuchi.hatenablog.com/entry/IntroloopBGM
//	該当ソースは、参考元のライセンスが適用されます。
			AudioSource introSource = null;
			AudioSource loopSource = null;
			// イントロ音の再生
			introSource.PlayScheduled( AudioSettings.dspTime );
			// イントロ音の再生後、ループ音の再生を登録
			loopSource.PlayScheduled(
				AudioSettings.dspTime +
				( (float)introSource.clip.samples / introSource.clip.frequency )
			);
*/
		}

		///------------------------------------------------------------------------------------------------
		/// ● 停止
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 停止
		/// </summary>
		public async UniTask Stop() {
			// 未再生の場合、未処理
			if (	!_runningName.HasValue ||
					( !_isPlayOneShot && !IsPlaying( _runningName.Value ) )
			) {
				return;
			}

			_runningCanceler.Cancel();
			_state = SMAudioSourceState.Stopping;

			// 音量フェードアウト
			await FadeVolume( SMAudioSourceFadeType.Out );
			// 停止
			StopSource( _runningName.Value );
			UnloadStopAudio();	// 停止中の音を全削除
		}

		/// <summary>
		/// ● 停止（AudioSource）
		/// </summary>
		void StopSource( T enumName ) {
			// 指定音が未再生の場合、未処理
			if ( _state == SMAudioSourceState.Stop || !IsRunningEnum( enumName ) )	{ return; }

			_runningCanceler.Cancel();
			if ( _source != null ) {
				_source.Stop();
				_source.clip = null;
			}
			_playingAudio = null;
			_state = SMAudioSourceState.Stop;
			_runningName = null;
			// フェードがスキップされた可能性を考慮し、初期値を設定
			_volumePercent.Value = 0;
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 音量フェード
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask FadeVolume( SMAudioSourceFadeType type ) {
			var duration = type == SMAudioSourceFadeType.In ? _fadeInDuration : _fadeOutDuration;
			var target = type == SMAudioSourceFadeType.In ? 1 : 0;
			
			// フェードしない場合、即音量を設定
			if ( duration == 0 ) {
				_volumePercent.Value = target;
				return;
			}

			// 現在音量を考慮
			var rate = Mathf.Abs( target - _volumePercent.Value );
			duration *= rate;

			// フェードする場合、音量比率を徐々に変更
			await DOTween.To(
				() => _volumePercent.Value,
				time => _volumePercent.Value = time,
				target,
				duration
			)
			.SetEase( Ease.InOutCubic )
			.Play()
			.ToUniTask( _runningCanceler );
		}
	}
}