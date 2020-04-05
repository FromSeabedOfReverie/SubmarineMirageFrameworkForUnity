//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Audio {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using KoganeUnityLib;
	using Singleton;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 音再生管理のクラス
	///----------------------------------------------------------------------------------------------------
	///		音楽、ジングル音、効果音（ループ用）、効果音の再生に対応。
	///		各種音の定数等、プロジェクト固有定義を、継承先クラスで行う。
	/// </summary>
	///====================================================================================================
	public abstract class AudioManager<TManager> : Singleton<TManager>, IAudioManager
		where TManager : IAudioManager, new()
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>音一覧の辞書</summary>
		protected readonly Dictionary<Type, IBaseAudioSource> _audioSources
			= new Dictionary<Type, IBaseAudioSource>();
		/// <summary>各種書類音量の一覧</summary>
		public Dictionary< Type, Dictionary<object, float> > _defaultVolumes	{ get; }
			= new Dictionary< Type, Dictionary<object, float> >();
		/// <summary>音の発生源</summary>
		public GameObject _speaker	{ get; private set; }
		/// <summary>一時停止中か？</summary>
		public bool _isPause		{ get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected AudioManager() {
			// 音発生用スピーカーを作成し、ゲーム内に配置
			_speaker = new GameObject( "_AudioSpeaker" );
			UnityEngine.Object.DontDestroyOnLoad( _speaker );	// シーン切り替え後も保持

			// 一時停止状態を設定
			Observable.EveryApplicationPause().Subscribe( is_ => _isPause = is_ );

#if DEVELOP
			// デバッグ表示を設定
			_lateUpdateEvent.Subscribe( _ => {
				DebugDisplay.s_instance.Add( Color.cyan );
				DebugDisplay.s_instance.Add( $"● {this.GetAboutName()}" );
				DebugDisplay.s_instance.Add( Color.white );
				_audioSources.ForEach( pair => pair.Value.UpdateDebug() );
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再生
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Play<T>( T enumName ) where T : struct, Enum {
			var type = enumName.GetType();
			var source = (BaseAudioSource<T>)_audioSources[type];
			source.Play( enumName );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 停止
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Stop<T>() where T : struct, Enum {
			var type = typeof( T );
			var source = _audioSources[type];
			source.Stop();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 全停止
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void StopAll() {
			_audioSources.ForEach( pair => pair.Value.Stop() );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Release() {
			// 各種管理クラスを解放
			_audioSources.ForEach( pair => pair.Value.Release() );
		}
	}
}