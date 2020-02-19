//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.IO;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Purchasing;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 購入商品の情報クラス
	///----------------------------------------------------------------------------------------------------
	///		商品情報を保存している。
	/// </summary>
	///====================================================================================================
	public class PurchaseProductData : CSVData<string> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アイコン画像の頭階層</summary>
		const string ICON_TOP_PATH = "Purchase";

		/// <summary>辞書への登録鍵</summary>
		public override string _registerKey => _name;

		/// <summary>名前</summary>
		public string _name { get; private set; }
		/// <summary>日本円</summary>
		public int _jpy { get; private set; }
		/// <summary>説明文</summary>
		public string _info { get; private set; }
		/// <summary>アイコン画像階層</summary>
		public string _iconPath { get; private set; }
		/// <summary>GooglePlayStoreの商品名</summary>
		public string _googlePlayStoreID { get; private set; }
		/// <summary>AppStoreの商品名</summary>
		public string _appleStoreID { get; private set; }
		/// <summary>商品型</summary>
		public ProductType _type { get; private set; }
		/// <summary>公開版か？</summary>
		public bool _isRelease { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			_name				= texts[0];
			_jpy				= texts[1].ToInt();
			_info				= texts[2];
			_iconPath			= Path.Combine( ICON_TOP_PATH, texts[3] );

			var id = $"{Application.identifier.ToLower()}.{texts[4]}";
			_googlePlayStoreID	= id;
			_appleStoreID		= id;

			_type				= texts[5].ToEnum<ProductType>();
			_isRelease			= texts[6].ToBoolean();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Register( ConfigurationBuilder builder ) {
			var ids = new IDs();

			if ( !_googlePlayStoreID.IsNullOrEmpty() )	{ ids.Add( _googlePlayStoreID,	GooglePlay.Name ); }
			if ( !_appleStoreID.IsNullOrEmpty() )		{ ids.Add( _appleStoreID,		AppleAppStore.Name ); }

			builder.AddProduct( _name, _type, ids );
		}
	}
}