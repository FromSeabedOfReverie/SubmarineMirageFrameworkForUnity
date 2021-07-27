//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using File;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ サーバーキャッシュ情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		サーバー受信情報のキャッシュ情報を格納。
	///		このキャッシュに保存された情報は、必ずセーブされる。
	/// </summary>
	///====================================================================================================
	public class ServerCacheDataManager : BaseSMSaveData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>未配信状態の、初期サーバー版</summary>
		public const string INITIAL_SERVER_VERSION = "0.00";

		/// <summary>サーバー版</summary>
		public string _serverVersion = INITIAL_SERVER_VERSION;

		// 以下辞書達は、基底クラスの辞書で纏めると、シリアライザ非対応の為、復元されない
		// その為、仕方無くジェネリック型ごとに分離した
		/// <summary>画像の生情報の辞書</summary>
		public Dictionary< string, ServerCacheData<SMTextureRawData> > _textureData
			= new Dictionary< string, ServerCacheData<SMTextureRawData> >();
		/// <summary>音の生情報の辞書</summary>
		public Dictionary< string, ServerCacheData<SMAudioRawData> > _audioData
			= new Dictionary< string, ServerCacheData<SMAudioRawData> >();
		/// <summary>文章情報の辞書</summary>
		public Dictionary< string, ServerCacheData<string> > _textData
			= new Dictionary< string, ServerCacheData<string> >();
		/// <summary>生情報の辞書</summary>
		public Dictionary< string, ServerCacheData<byte[]> > _rawData
			= new Dictionary< string, ServerCacheData<byte[]> >();

		/// <summary>書類の読み書き</summary>
		SMFileLoader _loader	=> SMFileManager.s_instance._fileLoader;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public ServerCacheDataManager() {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Register( CacheData data ) {
			var typeEnum = _loader.GetDataType<object>( data._type );

			switch ( typeEnum ) {
				case SMFileLoader.DataType.Texture:
				case SMFileLoader.DataType.Sprite:
					_textureData[data._path] = new ServerCacheData<SMTextureRawData>( data, typeEnum );
					break;

				case SMFileLoader.DataType.Audio:
					_audioData[data._path] = new ServerCacheData<SMAudioRawData>( data, typeEnum );
					break;

				case SMFileLoader.DataType.Text:
					_textData[data._path] = new ServerCacheData<string>( data, typeEnum );
					break;

				case SMFileLoader.DataType.Raw:
					_rawData[data._path] = new ServerCacheData<byte[]>( data, typeEnum );
					break;

				default:
					Log.Error( $"未対応保存情報 : {typeEnum}\n{data}" );
					break;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録解除
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Unregister( string path ) {
			_textureData.Remove( path );
			_audioData.Remove( path );
			_textData.Remove( path );
			_rawData.Remove( path );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			await Enumerable.Empty<UniTask>()
				.Concat( _textureData.Select( pair => pair.Value.Load() ) )
				.Concat( _audioData.Select( pair => pair.Value.Load() ) )
				.Concat( _textData.Select( pair => pair.Value.Load() ) )
				.Concat( _rawData.Select( pair => pair.Value.Load() ) );
			await base.Load();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			await Enumerable.Empty<UniTask>()
				.Concat( _textureData.Select( pair => pair.Value.Save() ) )
				.Concat( _audioData.Select( pair => pair.Value.Save() ) )
				.Concat( _textData.Select( pair => pair.Value.Save() ) )
				.Concat( _rawData.Select( pair => pair.Value.Save() ) );
			await base.Save();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Dispose() {
			_textureData.Clear();
			_audioData.Clear();
			_textData.Clear();
			_rawData.Clear();
		}
	}
}