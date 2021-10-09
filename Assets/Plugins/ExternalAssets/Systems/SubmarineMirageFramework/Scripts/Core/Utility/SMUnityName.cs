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
	using Service;
	using Task;
	///====================================================================================================
	/// <summary>
	/// ■ Unity設定の名称使用の基盤クラス
	/// </summary>
	///====================================================================================================
	public abstract class SMUnityName<TName> : SMTask, ISMService where TName : Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Dont;
		/// <summary>名称のキャッシュ一覧の辞書</summary>
		readonly Dictionary<TName, string> _namesCache = EnumUtils.GetValues<TName>()
				.ToDictionary( name => name, name => name.ToString() );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（文字列）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string Get( TName name ) => _namesCache[name];
	}
}