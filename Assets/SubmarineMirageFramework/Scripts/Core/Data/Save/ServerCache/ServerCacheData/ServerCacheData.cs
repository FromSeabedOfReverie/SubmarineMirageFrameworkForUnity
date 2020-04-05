//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data.Save {
	using System;
	using UnityEngine;
	using UniRx.Async;
	using File;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ サーバーキャッシュの情報クラス
	///----------------------------------------------------------------------------------------------------
	///		サーバー受信情報のキャッシュ情報を格納。
	///		このキャッシュに保存された情報は、必ずセーブされる。
	/// </summary>
	///====================================================================================================
	public class ServerCacheData<T> : SaveData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>登録鍵になる、書類の階層</summary>
		public string _path;
		/// <summary>情報の型名</summary>
		public string _typeName;	// Type型では、そのまま保存できない
		/// <summary>情報の型</summary>
		public FileLoader.DataType _typeEnum;
		/// <summary>生情報</summary>
		public T _rawData;

		/// <summary>書類の読み書き</summary>
		FileLoader _loader			=> FileManager.s_instance._fileLoader;
		/// <summary>一時キャッシュ情報</summary>
		CacheDataManager _tempCache	=> FileManager.s_instance._cacheData;
		///------------------------------------------------------------------------------------------------
		/// ● コンストラクタ
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（非シリアル化用）
		/// </summary>
		public ServerCacheData() {
		}
		/// <summary>
		/// ● コンストラクタ（情報登録用）
		/// </summary>
		public ServerCacheData( CacheData data, FileLoader.DataType typeEnum ) {
			_path = data._path;
			_typeName = data._type.FullName;
			_typeEnum = typeEnum;

			switch ( _typeEnum ) {
				case FileLoader.DataType.Texture:
					_rawData = (T)(object)( ( (Texture2D)data._data ).ToRawData() );
					break;
				case FileLoader.DataType.Sprite:
					_rawData = (T)(object)( ( (Sprite)data._data ).ToRawData() );
					break;
				case FileLoader.DataType.Audio:
					_rawData = (T)(object)( ( (AudioClip)data._data ).ToRawData() );
					break;
				case FileLoader.DataType.Text:
				case FileLoader.DataType.Raw:
					_rawData = (T)data._data;
					break;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			var data = new CacheData();
			data._path = _path;
			data._type = Type.GetType( _typeName );

			switch ( _typeEnum ) {
				case FileLoader.DataType.Texture:
					data._data = ( (TextureRawData)(object)_rawData ).ToTexture2D();
					break;
				case FileLoader.DataType.Sprite:
					data._data = ( (TextureRawData)(object)_rawData ).ToSprite();
					break;
				case FileLoader.DataType.Audio:
					data._data = ( (AudioRawData)(object)_rawData ).ToAudioClip();
					break;
				case FileLoader.DataType.Text:
				case FileLoader.DataType.Raw:
					data._data = _rawData;
					break;
			}

			_tempCache.Register( data );
			await base.Load();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			await base.Save();
		}
	}
}