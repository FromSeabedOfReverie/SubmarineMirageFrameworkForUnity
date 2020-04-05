//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------

// まだ、リファクタリング中
#if false
namespace SubmarineMirageFramework.Scene {
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using KoganeUnityLib;
	using Data;
	using Data.Server;
	using Data.Save;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 初期読込の場面クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class InitializeScene {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		/// <summary>システム情報</summary>
		UISystemInfo _uiSystemInfo => UIManager.s_instance.Get<UISystemInfo>();
		UINowLoading _uiNowLoading => UIManager.s_instance.Get<UINowLoading>();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読み込み率を計算
		/// </summary>
		///------------------------------------------------------------------------------------------------
		float CalculationLoadRate( float loadRate, float maxRate ) {
			return Mathf.Min( loadRate + 0.1f * TimeManager.s_instance._unscaledDeltaTime, maxRate );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask Load() {
			// ServerDataManager処理を待機

			await CheckMonetizeIntiialized();
			await CheckUpdateNotification();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 金策の初期化済を判定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask CheckMonetizeIntiialized() {
			// TODO : 失敗してたら、return


			// このタイミングで、生成初期化しないと、失敗を取得できない

			// 課金初期化完了、待機状態まで待機
			await UniTask.WaitUntil( () => GamePurchaseManager.s_instance._isInitialized );
			await UniTask.WaitUntil( () => !GamePurchaseManager.s_instance._isProcessing );
			Log.Debug( "システム全体初期化 : 課金初期化完了", Log.Tag.Process );
		
			// 広告初期化終了まで待機
			await UniTask.WaitUntil( () => GameAdvertisementManager.s_instance._isInitialized );
			Log.Debug( "システム全体初期化 : 広告初期化完了", Log.Tag.Process );

			// システム情報窓が閉じるまで待機
			await UniTask.WaitUntil( () => !_uiSystemInfo.is_play() );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新のお知らせ表示を判定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask CheckUpdateNotification() {
			// TODO : 失敗してたら、return

			// アプリケーション版が等しい場合、未処理
			if ( _applicationData._version == _settingData._version ) {
				return;
			}

			// ダウンロード通知を表示
			var isEnd = false;
			// TODO : _uiSystemInfo.playを asyncにする
			_uiSystemInfo.play(
				SystemInfoData.Type.ApplicationUpdateNotice,
				() => isEnd = true
			);
			// ダウンロード通知が終了するまで待機
			await UniTask.WaitUntil( () => isEnd );

#if !UNITY_EDITOR
			// ちょっと待機後、Web表示
			await UniTask.Delay( 500 );
			Application.OpenURL( _applications.Get()._url );
#endif
		}
	}
}
#endif