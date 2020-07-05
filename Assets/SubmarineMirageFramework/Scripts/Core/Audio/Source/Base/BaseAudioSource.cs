//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Audio {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using DG.Tweening;
	using DG.Tweening.Core;
	using DG.Tweening.Plugins.Options;
	using KoganeUnityLib;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 音源管理の基盤クラス
	///----------------------------------------------------------------------------------------------------
	///		音楽、ジングル音、効果音（ループ用）、効果音の管理クラスに継承させて使う。
	/// </summary>
	///====================================================================================================
	public abstract class BaseAudioSource<T> : IBaseAudioSource where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>再生状態</summary>
		public enum State {
			/// <summary>再生中</summary>
			Playing,
			/// <summary>再生</summary>
			Play,
			/// <summary>停止中</summary>
			Stopping,
			/// <summary>停止</summary>
			Stop,
		}
		/// <summary>音フェード型</summary>
		enum FadeType {
			/// <summary>フェードイン</summary>
			In,
			/// <summary>フェードアウト</summary>
			Out,
		}

		/// <summary>再生状態</summary>
		public State _state	{ get; private set; } = State.Stop;
		/// <summary>実行中の音名</summary>
		public T? _runningName	{ get; protected set; }
		/// <summary>実行処理停止用の記号</summary>
		CancellationTokenSource _runningCancel = new CancellationTokenSource();
		/// <summary>音の管理者</summary>
		IAudioManager _manager;

		/// <summary>音の発生源</summary>
		protected AudioSource _source;
		/// <summary>再生中の音</summary>
		public AudioFile<T> _playingAudio	{ get; protected set; }
		/// <summary>音のキャッシュ一覧</summary>
		readonly Dictionary< T, AudioFile<T> > _audioCache = new Dictionary< T, AudioFile<T> >();

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
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected BaseAudioSource( IAudioManager manager ) {
			// 管理者を登録
			_manager = manager;
			// 音の発生源を作成
			_source = _manager._speaker.AddComponent<AudioSource>();
			_source.playOnAwake = false;

			// 音量を作成（変更通知）
			_settingVolume.Subscribe( _ => ApplyVolume() );
			_volumePercent.Subscribe( _ => ApplyVolume() );
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
		/// <summary>
		/// ● 音を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected AudioFile<T> GetAudio( T enumName ) {
			return _audioCache.GetOrDefault( enumName );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 基本音量を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		float GetDefaultVolume( T enumName ) {
			var type = enumName.GetType();
			var volumes = _manager._defaultVolumes.GetOrDefault( type );
			return volumes != null ? volumes.GetOrDefault( enumName, 1 ) : 1;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込状態が等しいか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsState( T enumName, AudioFile<T>.State state ) {
			var audio = GetAudio( enumName );
			return audio != null && audio._state == state;
		}
		///------------------------------------------------------------------------------------------------
		/// ● 再生中か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生中か？
		/// </summary>
		public bool IsPlaying( T enumName ) {
			return (
				!_isPlayOneShot &&
				( _state == State.Playing || _state == State.Play ) &&
				IsRunningEnum( enumName )
			);
		}
		/// <summary>
		/// ● 音発生源が再生中か？
		/// </summary>
		bool IsPlayingSource() {
			// 一時停止中は停止判定になる為、とりあえず再生中とする
			return ( _source != null && _source.isPlaying ) || _manager._isPause;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 実行中定数と等しいか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		bool IsRunningEnum( T enumName ) {
			return _runningName.HasValue && enumName.Equals( _runningName.Value );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読み込み
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask< AudioFile<T> > Load( T enumName, CancellationTokenSource cancel = null ) {
			// 読込済の場合、キャッシュを返す
			var audio = _audioCache.GetOrDefault( enumName );
			if ( audio != null )	{ return audio; }


			// ファイル名を設定
			var fileName = _prefixName + enumName.ToString();
			// 階層名を設定
			// 空文字の場合、書類名と同じ階層名を使用する（アセットバンドルとの辻褄合わせ）
			var path = _path.IsNullOrEmpty() ? fileName : _path;
			// 音量を設定
			var volume = GetDefaultVolume( enumName );
			// 書類を作成
			audio = new AudioFile<T>( fileName, path, enumName, volume );
			_audioCache[enumName] = audio;	// キャッシュ登録


			// 停止命令が無の場合、作成
			if ( cancel == null )	{ cancel = new CancellationTokenSource(); }
			var cancelToken = cancel.Token;

			// 処理停止命令が入っても、続行
			try {
				// 読込
				await audio.Load( cancel );
			} catch ( OperationCanceledException ) {}	// 処理停止例外を無視

			// 読込失敗の場合、処理停止エラー発生
			if ( audio._state != AudioFile<T>.State.Loaded ) {
				Unload( enumName );
				throw new OperationCanceledException( cancelToken );
			}

			return audio;
		}
		///------------------------------------------------------------------------------------------------
		/// ● 読み込み解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読み込み解除
		/// </summary>
		public void Unload( T enumName ) {
			// 停止
			StopSource( enumName );
			// 読み込まれている場合、削除
			var audio = GetAudio( enumName );
			if ( audio != null ) {
				audio.Unload();
				_audioCache.Remove( enumName );
			}
		}
		/// <summary>
		/// ● 停止音の読み込み解除
		/// </summary>
		void UnloadStopAudio() {
			if ( !_isLoadSingle )	{ return; }

			// 再生中以外の音を削除
			_audioCache
				.Where( pair => !IsRunningEnum( pair.Value._enumName ) )
				.Select( pair => pair.Key )
				.ToList()	// foreach内の削除によるエラーを防ぐ為、コピー
				.ForEach( name => Unload( name ) );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 再生
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生
		/// </summary>
		public void Play( T enumName ) {
			PlaySub( enumName ).Forget();
		}
		/// <summary>
		/// ● 再生（補助）
		/// </summary>
		async UniTask PlaySub( T enumName ) {
			// 重複再生せず、指定音が再生中の場合、未処理
			if ( !_isOverlapPlay && IsPlaying( enumName ) )	{ return; }

			_runningCancel.Cancel();
			_runningCancel = new CancellationTokenSource();
			var cancel = _runningCancel.Token;
			_state = State.Playing;
			var isEqualRunning = IsRunningEnum( enumName );
			_runningName = enumName;

			// 停止例外発生箇所は、再生と停止のみの為、即別の状態が入り、停止しっぱなしで問題ない

			// 読込、現在再生音に登録
			var audio = await Load( _runningName.Value, _runningCancel );	// 停止例外になる
			// 前回と違う音で、音発生源が再生中の場合
			if ( !isEqualRunning && audio != _playingAudio && IsPlayingSource() ) {
				// 音量フェードアウト
				await FadeVolume( FadeType.Out );	// 停止例外になる
			}
			// 再生
			_playingAudio = audio;
			PlaySource();
			UnloadStopAudio();	// 停止中の音を全削除
			// 音量フェードイン
			await FadeVolume( FadeType.In );		// 停止例外になる
			// 再生状態へ
			_state = State.Play;
			// 再生完了まで待機
			await UniTask.WaitUntil( () => !IsPlayingSource(), PlayerLoopTiming.Update, cancel );
			// 停止
			StopSource( _runningName.Value );
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
		/// ● 停止（再生中音）
		/// </summary>
		public void Stop() {
			StopSub().Forget();
		}
		/// <summary>
		/// ● 停止（補助）
		/// </summary>
		async UniTask StopSub() {
			// 未再生の場合、未処理
			if (	!_runningName.HasValue ||
					( !_isPlayOneShot && !IsPlaying( _runningName.Value ) )
			) { return; }

			_runningCancel.Cancel();
			_runningCancel = new CancellationTokenSource();
			var cancel = _runningCancel.Token;
			_state = State.Stopping;

			// 停止例外発生箇所は、再生と停止のみの為、即別の状態が入り、停止しっぱなしで問題ない

			// 音量フェードアウト
			await FadeVolume( FadeType.Out );	// 停止例外になる
			// 停止
			StopSource( _runningName.Value );
			UnloadStopAudio();	// 停止中の音を全削除
		}
		/// <summary>
		/// ● 停止（AudioSource）
		/// </summary>
		void StopSource( T enumName ) {
			// 指定音が未再生の場合、未処理
			if ( _state == State.Stop || !IsRunningEnum( enumName ) )	{ return; }

			_runningCancel.Cancel();
			if ( _source != null ) {
				_source.Stop();
				_source.clip = null;
			}
			_playingAudio = null;
			_state = State.Stop;
			_runningName = null;
			// フェードがスキップされた可能性を考慮し、初期値を設定
			_volumePercent.Value = 0;
		}
		/// <summary>
		/// ● 全停止
		/// </summary>
		public void StopAll() {
			Stop();	// スピーカーを停止
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 音量フェード
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask FadeVolume( FadeType type ) {
			var duration = type == FadeType.In ? _fadeInDuration : _fadeOutDuration;
			var target = type == FadeType.In ? 1 : 0;
			var cancel = _runningCancel.Token;

			// フェードする場合、音量比率を徐々に変更
			if ( duration != 0 ) {
				var isDone = false;
				var volumePercentSequence = DOTween.Sequence();
				volumePercentSequence
					.Append( GetVolumePercentTween( target, duration ) )
					.OnComplete( () => isDone = true )
					.Play();

				// 停止処理例外を無視
				try {
					// フェードを待機
					await UniTask.WaitUntil( () => isDone, PlayerLoopTiming.Update, cancel );
				} catch ( OperationCanceledException ) {}

				volumePercentSequence.Kill();	// killったら、作り直さないと可笑しくなる

				// 処理中断例外要求の場合、エラー発生
				if ( cancel.IsCancellationRequested ) {
					throw new OperationCanceledException( cancel );
				}


			// フェードしない場合、即音量を設定
			} else {
				_volumePercent.Value = target;
			}
		}
		/// <summary>
		/// ● 音量補完動作を取得
		/// </summary>
		TweenerCore<float, float, FloatOptions> GetVolumePercentTween( float end, float duration ) {
			// 現在音量を考慮
			var rate = Mathf.Abs( end - _volumePercent.Value );
			duration *= rate;
			// 補完処理を返す
			return DOTween.To(
				()		=> _volumePercent.Value,
				time	=> _volumePercent.Value = time,
				end,
				duration
			).SetEase( Ease.InOutCubic );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新（デバッグ）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void UpdateDebug() {
#if !DEVELOP
			return;
#endif
			// クラス名、音量描画
			DebugDisplay.s_instance.Add( Color.yellow );	// 見出し色に設定
			DebugDisplay.s_instance.Add(
				$"・{this.GetAboutName()} : {_state} : 音量 : {_source.volume:F2}" );
			DebugDisplay.s_instance.Add( Color.white );	// 基本色に設定

			// キャッシュ中の音を描画
			_audioCache.ForEach( pair => {
				var s = "";
				switch ( pair.Value._state ) {
					case AudioFile<T>.State.Loading:	s = "読込中";	break;
					case AudioFile<T>.State.Loaded:		s = "読込完了";	break;
					case AudioFile<T>.State.LoadError:	s = "読込失敗";	break;
					case AudioFile<T>.State.Unload:	s = "読込解除";	break;
				}
				if ( !s.IsNullOrEmpty() ) {
					var n = pair.Value._audioClip == null ? " : null" : "";
					DebugDisplay.s_instance.Add( $"\t{s} : {pair.Key}{n}" );
				}
			} );

			// 再生中の音を描画
			if ( !_isPlayOneShot && _playingAudio != null ) {
				var n = _playingAudio._audioClip == null ? " : null" : "";
				DebugDisplay.s_instance.Add( $"\t再生中 : {_playingAudio._enumName}{n}" );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Release() {
			StopAll();	// 全停止

			// 音データを全て解放
			_audioCache.ForEach( pair => pair.Value.Unload() );
			_audioCache.Clear();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		~BaseAudioSource() {
			_runningCancel.Cancel();
			Release();
		}
	}
}