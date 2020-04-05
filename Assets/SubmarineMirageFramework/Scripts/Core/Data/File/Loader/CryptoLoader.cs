//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.File {
	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using UniRx.Async;
	using Utility;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 暗号の読み書きクラス
	///----------------------------------------------------------------------------------------------------
	///		オブジェクトをクラスごと、暗号化して、シリアライズ保存する。
	///		参考URL : http://qiita.com/tempura/items/ad154d1269882ceda0f4
	///			該当ソースは、参考元のライセンスが適用されます。
	/// </summary>
	///====================================================================================================
	public class CryptoLoader : DataLoader {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>読込階層</summary>
		public const string SUB_PATH = "Data";
		/// <summary>暗号化の鍵</summary>
		const string ENCRYPT_KEY = "ENCRYPT_KEY";
		/// <summary>暗号パスワードの数</summary>
		const int ENCRYPT_PASSWORD_COUNT = 16;
		/// <summary>パスワード</summary>
		const string PASSWORD = "PASSWORD";
		/// <summary>パスワードの長さ</summary>
		static readonly int PASSWORD_CHARS_LENGTH = PASSWORD.Length;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask<T> Load<T>( string path ) {
			var data = default( T );


			try {
				path = Path.Combine( FileManager.LOAD_EXTERNAL_PATH, SUB_PATH, path );
				_loadingCount++;
				
				// 読み込み
				byte[] ivBytes = null;
				byte[] loadData = null;
				using ( var fs = new FileStream( path, FileMode.Open, FileAccess.Read ) )
				using ( var br = new BinaryReader( fs ) ) {
					var length = br.ReadInt32();
					ivBytes = br.ReadBytes( length );
					length = br.ReadInt32();
					loadData = br.ReadBytes( length );
				}
				// 複合化
				var iv = Encoding.UTF8.GetString( ivBytes );
				var bytes = await DecryptAES( loadData, iv );
				// 保存データ復元
				data = SerializerUtility.Deserialize<T>( bytes );

				Log.Debug( $"読込成功 : {path}\n{data}", Log.Tag.File );
				_loadingCount--;


			} catch ( Exception e ) {
				Log.Error( $"読込中に問題発生 : {path}\n{data}\n{e}", Log.Tag.File );
				_errorCount++;
			}


			return data;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● AES複合化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask<byte[]> DecryptAES( byte[] src, string iv ) {
			var dst = new byte[src.Length];

			using ( var rijndael = new RijndaelManaged() ) {
				rijndael.Padding = PaddingMode.PKCS7;
				rijndael.Mode = CipherMode.CBC;
				rijndael.KeySize = 256;
				rijndael.BlockSize = 128;

				var key = Encoding.UTF8.GetBytes( ENCRYPT_KEY );
				var vec = Encoding.UTF8.GetBytes( iv );

				using ( var decryptor = rijndael.CreateDecryptor( key, vec ) )
				using ( var ms = new MemoryStream( src ) )
				using ( var cs = new CryptoStream( ms, decryptor, CryptoStreamMode.Read ) ) {
					await cs.ReadAsync( dst, 0, dst.Length );
				}
			}

			return dst;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask Save<T>( string path, T data ) {
			try {
				path = Path.Combine( FileManager.SAVE_EXTERNAL_PATH, SUB_PATH, path );
				_savingCount++;

				// 暗号化
				var bytes = SerializerUtility.Serialize( data );
				var pair = await EncryptAES( bytes );
				var iv = pair.Key;
				var saveData = pair.Value;

				// フォルダを作成
				var folder = Path.GetDirectoryName( path );
				FileManager.s_instance.CreatePath( folder );

				// 保存
				var ivBytes = Encoding.UTF8.GetBytes( iv );
				using ( var fs = new FileStream( path, FileMode.Create, FileAccess.Write ) )
				using ( var bw = new BinaryWriter( fs ) ) {
					bw.Write( ivBytes.Length );
					bw.Write( ivBytes );
					bw.Write( saveData.Length );
					bw.Write( saveData );
				}

				Log.Debug( $"保存成功 : {path}\n{data}", Log.Tag.File );
				_savingCount--;


			} catch ( Exception e ) {
				Log.Error( $"保存中に問題発生 : {path}\n{data}\n{e}", Log.Tag.File );
				_errorCount++;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● AES暗号化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask< KeyValuePair<string, byte[]> > EncryptAES( byte[] src ) {
			var iv = CreatePassword( ENCRYPT_PASSWORD_COUNT );
			byte[] dst = null;

			using ( var rijndael = new RijndaelManaged() ) {
				rijndael.Padding = PaddingMode.PKCS7;
				rijndael.Mode = CipherMode.CBC;
				rijndael.KeySize = 256;
				rijndael.BlockSize = 128;

				var key = Encoding.UTF8.GetBytes( ENCRYPT_KEY );
				var vec = Encoding.UTF8.GetBytes( iv );

				using ( var encryptor = rijndael.CreateEncryptor( key, vec ) )
				using ( var ms = new MemoryStream() )
				using ( var cs = new CryptoStream( ms, encryptor, CryptoStreamMode.Write ) ) {
					await cs.WriteAsync( src, 0, src.Length );
					cs.FlushFinalBlock();
					dst = ms.ToArray();
				}
			}

			return new KeyValuePair<string, byte[]>( iv, dst );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● パスワード生成
		///		count : 文字列数
		///		戻り値 : パスワード
		/// </summary>
		///------------------------------------------------------------------------------------------------
		string CreatePassword( int count ) {
			var sb = new StringBuilder( count );
			for ( var i = count - 1; i >= 0; i-- ) {
				var random = UnityEngine.Random.Range( 0, PASSWORD_CHARS_LENGTH );
				var c = PASSWORD[random];
				sb.Append( c );
			}
			return sb.ToString();
		}
	}
}