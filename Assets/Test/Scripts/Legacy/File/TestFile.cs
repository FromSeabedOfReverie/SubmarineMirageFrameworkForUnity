//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using Process;
	using Data;
	using Data.File;
	using Data.Save;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 書類の試験クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TestFile : MonoBehaviourProcess {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>書類読み書き</summary>
		FileLoader _loader			=> FileManager.s_instance._fileLoader;
		/// <summary>暗号化読み書き</summary>
		CryptoLoader _crypto		=> FileManager.s_instance._cryptoLoader;
		/// <summary>CSV書類読み書き</summary>
		CSVLoader _csv				=> FileManager.s_instance._csvLoader;
		/// <summary>遊戯情報</summary>
		PlayDataManager _play		=> AllDataManager.s_instance._save._play;
		/// <summary>設定情報</summary>
		SettingDataManager _setting	=> AllDataManager.s_instance._save._setting;
		/// <summary>サーバーキャッシュ情報</summary>
		ServerCacheDataSaver _cache	=> AllDataManager.s_instance._save._cache;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void Constructor() {
			base.Constructor();

			// ● 読込
			_loadEvent += async () => {
//				await LoadSaveData();
//				await LoadCache();
//				await LoadPlay();
//				await LoadCrypt();
//				await LoadCSV();
//				await LoadExternalFile();
//				await LoadExternalFile();

				await UTask.DontWait();
			};
		}


		async UniTask LoadSaveData() {
			await _play.SaveCurrentData();
			await _setting.Save();

			var c = new System.Threading.CancellationTokenSource();
			foreach ( var pair in _cache._data._textureData) {
				ShowImage( pair.Value._rawData.ToSprite() );
				await UTask.Delay( c.Token, 2000 );
			}
			c.Dispose();
		}


		async UniTask LoadCache() {
			await _play.SaveCurrentData();
			await _setting.Save();
			await _cache.Save();

			var sprite1 = await _loader.LoadExternal<Sprite>( "TestServerData/TestIcon.png", true );
			var sprite2 = await _loader.LoadExternal<Sprite>( "TestServerData/TestIcon.png", true );

			var sprite = await _loader.LoadServer<Sprite>(
				"https://drive.google.com/uc?export=view&id=1jfbizd8DuumCNpZjLvKhBVPYhorzpt6V" );
			var audio = await _loader.LoadServer<AudioClip>(
				"https://drive.google.com/uc?export=view&id=1L9PoewR7M2Wv-7vaN2Uw-ubjbraKB79y",
				AudioType.OGGVORBIS );
			var text = await _loader.LoadServer<string>(
				"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1615869423&single=true&output=csv"
			);

			await _cache.Save();

			ShowImage( sprite );
			PlayAudio( audio );
			Log.Debug( _csv.TextToCSV( text ).ToDeepString() );
		}


		async UniTask LoadPlay() {
			Sprite sprite = null;

			sprite = await _loader.LoadExternal<Sprite>( "TestServerData/TestIcon.png" );
			await _play.LoadCurrentData( 0 );
			_play._currentData.RegisterPicture( sprite );
			await _play.SaveCurrentData();

			sprite = await _loader.LoadExternal<Sprite>( "TestServerData/TestBanner.png" );
			await _play.LoadCurrentData( 1 );
			_play._currentData.RegisterPicture( sprite );
			await _play.SaveCurrentData();

			await _play.LoadCurrentData( 0 );
			sprite = _play._currentData._pictures.FirstOrDefault();

			ShowImage( sprite );
		}


		async UniTask LoadCrypt() {
			var sprite = await _loader.LoadExternal<Sprite>( "TestServerData/TestIcon.png" );
			var raw = sprite.ToRawData();
			await _crypto.Save( "TestServerData/TestSave.raw", raw );

			raw = await _crypto.Load<TextureRawData>( "TestServerData/TestSave.raw" );
			sprite = raw.ToSprite();

			ShowImage( sprite );
		}


		async UniTask LoadCSV() {
			var data = await _csv.Load( FileLoader.Type.Server,
				"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1615869423&single=true&output=csv"
			);
			var ss = data
				.Select( list => string.Join( "\n", list ) );
			Log.Debug( "● Server\n\n" + string.Join( "\n\n", ss ) );

			await _csv.SaveExternal( "TestServerData/TestSave", data );

			data = await _csv.Load( FileLoader.Type.External, "TestServerData/TestSave" );
			ss = data
				.Select( list => string.Join( "\n", list ) );
			Log.Debug( "● Save\n\n" + string.Join( "\n\n", ss ) );
		}


		async UniTask LoadExternalFile2() {
			var audio = await _loader.LoadExternal<AudioClip>( "TestServerData/TestGameClear.ogg" );
			var raw = audio.ToRawData();
			await _loader.SaveExternal( "TestServerData/TestSave.pack", raw );

			raw = await _loader.LoadExternal<AudioRawData>( "TestServerData/TestSave.pack" );
			audio = raw.ToAudioClip();

			PlayAudio( audio );
		}



		async UniTask LoadExternalFile() {
			var sprite = await _loader.LoadExternal<Sprite>( "TestServerData/TestIcon.png" );
			await _loader.SaveExternal( "TestServerData/TestSave.tga", sprite, TextureRawData.Type.TGA );
			ShowImage( sprite );

			var audio = await _loader.LoadExternal<AudioClip>( "TestServerData/TestGameClear.ogg" );
			await _loader.SaveExternal( "TestServerData/TestSave.wav", audio );
			PlayAudio( audio );
		}



		void ShowImage( Sprite sprite ) {
			var image = GameObject.Find( "Canvas/Image" ).GetComponent<Image>();
			image.sprite = sprite;
			image.color = Color.white;
			image.SetNativeSize();
		}

		void PlayAudio( AudioClip audio ) {
			var source = CoreProcessManager.s_instance.gameObject.AddComponent<AudioSource>();
			source.clip = audio;
			source.loop = true;
			source.Play();
		}
	}
}