//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Task.Behaviour;
	using Singleton;
	///====================================================================================================
	/// <summary>
	/// ■ Unity設定の名称使用の基盤クラス
	/// </summary>
	///====================================================================================================
	public abstract class SMUnityName<TManager, TName> : SMSingleton<TManager>
		where TManager : SMBehaviour, new()
		where TName : Enum
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>名称のキャッシュ一覧の辞書</summary>
		readonly Dictionary<TName, string> _namesCache = EnumUtils.GetValues<TName>()
				.ToDictionary( name => name, name => name.ToString() );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected SMUnityName() {}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（文字列）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string Get( TName name ) => _namesCache[name];
	}
}