//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestFile
namespace SubmarineMirageFramework {
	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using Cysharp.Threading.Tasks;
	///====================================================================================================
	/// <summary>
	/// ■ 暗号の読み書きクラス
	///		オブジェクトをクラスごと、暗号化して、シリアライズ保存する。
	///		参考URL : http://qiita.com/tempura/items/ad154d1269882ceda0f4
	///			該当ソースは、参考元のライセンスが適用されます。
	/// </summary>
	///====================================================================================================
	public class SMCryptoLoader : BaseSMDataLoader {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>パスワードの長さ</summary>
		static readonly int PASSWORD_CHARS_LENGTH = SMMainSetting.CRYPTO_PASSWORD.Length;

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMCryptoLoader( SMFileManager fileManager ) : base( fileManager ) {
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup() {
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask<T> Load<T>( string path ) {
			var data = default( T );


			try {
				path = Path.Combine( SMMainSetting.LOAD_EXTERNAL_PATH, SMMainSetting.CRYPTO_PATH, path );
				_loadingCount++;
				
				// 読み込み
				byte[] ivBytes = null;
				byte[] loadData = null;
				var fs = new FileStream( path, FileMode.Open, FileAccess.Read );
				var br = new BinaryReader( fs );
				var length = br.ReadInt32();
				ivBytes = br.ReadBytes( length );
				length = br.ReadInt32();
				loadData = br.ReadBytes( length );
				br.Dispose();
				fs.Dispose();

				// 複合化
				var iv = Encoding.UTF8.GetString( ivBytes );
				var bytes = await DecryptAES( loadData, iv );
				// 保存データ復元
				data = SerializerSMUtility.Deserialize<T>( bytes );
#if TestFile
				SMLog.Debug( $"読込成功 : {path}\n{data}", SMLogTag.File );
#endif
				_loadingCount--;


			} catch ( Exception e ) {
				SMLog.Error( $"読込中に問題発生 : {path}\n{data}\n{e}", SMLogTag.File );
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

			var rijndael = new RijndaelManaged();
			rijndael.Padding = PaddingMode.PKCS7;
			rijndael.Mode = CipherMode.CBC;
			rijndael.KeySize = 256;
			rijndael.BlockSize = 128;

			var key = Encoding.UTF8.GetBytes( SMMainSetting.ENCRYPT_KEY );
			var vec = Encoding.UTF8.GetBytes( iv );

			var decryptor = rijndael.CreateDecryptor( key, vec );
			var ms = new MemoryStream( src );
			var cs = new CryptoStream( ms, decryptor, CryptoStreamMode.Read );
			await cs.ReadAsync( dst, 0, dst.Length, _asyncCanceler.ToToken() );
			cs.Dispose();
			ms.Dispose();
			decryptor.Dispose();

			rijndael.Dispose();

			return dst;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask Save<T>( string path, T data ) {
			try {
				path = Path.Combine( SMMainSetting.SAVE_EXTERNAL_PATH, SMMainSetting.CRYPTO_PATH, path );
				_savingCount++;

				// 暗号化
				var bytes = SerializerSMUtility.Serialize( data );
				var pair = await EncryptAES( bytes );
				var iv = pair.Key;
				var saveData = pair.Value;

				// フォルダを作成
				var folder = Path.GetDirectoryName( path );
				PathSMUtility.Create( folder );

				// 保存
				var ivBytes = Encoding.UTF8.GetBytes( iv );
				var fs = new FileStream( path, FileMode.Create, FileAccess.Write );
				var bw = new BinaryWriter( fs );
				bw.Write( ivBytes.Length );
				bw.Write( ivBytes );
				bw.Write( saveData.Length );
				bw.Write( saveData );
				bw.Dispose();
				fs.Dispose();
#if TestFile
				SMLog.Debug( $"保存成功 : {path}\n{data}", SMLogTag.File );
#endif
				_savingCount--;


			} catch ( Exception e ) {
				SMLog.Error( $"保存中に問題発生 : {path}\n{data}\n{e}", SMLogTag.File );
				_errorCount++;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● AES暗号化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask< KeyValuePair<string, byte[]> > EncryptAES( byte[] src ) {
			var iv = CreatePassword( SMMainSetting.ENCRYPT_PASSWORD_COUNT );
			byte[] dst = null;

			var rijndael = new RijndaelManaged();
			rijndael.Padding = PaddingMode.PKCS7;
			rijndael.Mode = CipherMode.CBC;
			rijndael.KeySize = 256;
			rijndael.BlockSize = 128;

			var key = Encoding.UTF8.GetBytes( SMMainSetting.ENCRYPT_KEY );
			var vec = Encoding.UTF8.GetBytes( iv );

			var encryptor = rijndael.CreateEncryptor( key, vec );
			var ms = new MemoryStream();
			var cs = new CryptoStream( ms, encryptor, CryptoStreamMode.Write );
			await cs.WriteAsync( src, 0, src.Length, _asyncCanceler.ToToken() );
			cs.FlushFinalBlock();
			dst = ms.ToArray();
			cs.Dispose();
			ms.Dispose();
			encryptor.Dispose();

			rijndael.Dispose();

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
				var c = SMMainSetting.CRYPTO_PASSWORD[random];
				sb.Append( c );
			}
			return sb.ToString();
		}
	}
}