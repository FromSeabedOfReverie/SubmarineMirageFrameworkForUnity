//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System;
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ 命令の情報インターフェース
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public interface ICommandData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>グループ鍵を設定したか？</summary>
		bool _isSetGroupKey	{ get; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● グループ鍵を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		object GetGroupKey();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 命令を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		Enum GetCommand();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void Set( string fileName, int index, List<string> texts, object groupKey );
	}
}