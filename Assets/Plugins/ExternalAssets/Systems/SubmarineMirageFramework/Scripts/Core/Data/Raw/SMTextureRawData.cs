//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 画像の生情報クラス
	///		参考URL : https://github.com/Veselov-Dmitry/TestSerialize/blob/master/Assets/Scripts/TEST.cs
	///			該当ソースは、参考元のライセンスが適用されます。
	/// </summary>
	///====================================================================================================
	public class SMTextureRawData : BaseSMUnityRawData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>画像の種類</summary>
		public enum Type {
			/// <summary>.exr画像</summary>
			EXR,
			/// <summary>.jpg画像</summary>
			JPG,
			/// <summary>.png画像</summary>
			PNG,
			/// <summary>.tga画像</summary>
			TGA,
		}

		/// <summary>暗号化の種類</summary>
		public Type _type;
		/// <summary>大きさ</summary>
		public Vector2 _size;	// シリアライザ非対応の為、Vector2Intは使用不可
		/// <summary>生情報</summary>
		public byte[] _data;

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（非シリアル化用）
		/// </summary>
		public SMTextureRawData() {
		}

		/// <summary>
		/// ● コンストラクタ（画像情報を設定）
		/// </summary>
		public SMTextureRawData( Texture2D texture, Type type = Type.PNG, int? encodeOption = null ) {
			_type = type;
			_size = new Vector2( texture.width, texture.height );

			switch ( type ) {
				case Type.EXR:
					if ( encodeOption.HasValue ) {
						_data = texture.EncodeToEXR( ( Texture2D.EXRFlags )encodeOption.Value );
					} else {
						_data = texture.EncodeToEXR();
					}
					break;
				case Type.JPG:
					if ( encodeOption.HasValue ) {
						_data = texture.EncodeToJPG( encodeOption.Value );
					} else {
						_data = texture.EncodeToJPG();
					}
					break;
				case Type.PNG:
					_data = texture.EncodeToPNG();
					break;
				case Type.TGA:
					_data = texture.EncodeToTGA();
					break;
			}
		}

		/// <summary>
		/// ● 破棄
		/// </summary>
		public override void Dispose() {
			_size = Vector2.zero;
			_data = null;
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● テクスチャに変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Texture2D ToTexture2D() {
			if ( _size == Vector2.zero ) {
				SetSizeForPNG();
			}
			var texture = new Texture2D( ( int )_size.x, ( int )_size.y );
			texture.LoadImage( _data, true );
			return texture;
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● スプライトに変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Sprite ToSprite( Vector2? pivot = null ) {
			return ToTexture2D().ToSprite( pivot );
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 大きさを設定（PNG画像のみ対応）
		///		参考URL : https://qiita.com/r-ngtm/items/6cff25643a1a6ba82a6c
		///			該当ソースは、参考元のライセンスが適用されます。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetSizeForPNG() {
			var pos = 16;	// 16バイトから開始
			var width = 0;
			for ( var i = 0; i < 4; i++ ) {
				width = width * 256 + _data[pos++];
			}
			var height = 0;
			for ( var i = 0; i < 4; i++ ) {
				height = height * 256 + _data[pos++];
			}
			_size = new Vector2( width, height );
		}
	}
}