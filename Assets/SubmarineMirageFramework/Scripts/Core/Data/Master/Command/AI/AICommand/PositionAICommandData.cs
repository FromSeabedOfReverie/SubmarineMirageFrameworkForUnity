//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 会話位置変更のAI命令情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class PositionAICommandData : AICommandData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>会話窓の位置</summary>
		public enum TalkPosition {
			/// <summary>下</summary>
			Lower,
			/// <summary>中央</summary>
			Middle,
			/// <summary>上</summary>
			Upper,
		}

		/// <summary>会話窓の位置</summary>
		public TalkPosition _talkPosition;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public PositionAICommandData( List<string> commands ) : base( commands ) {
			_talkPosition = commands[0].ToEnumOrDefault<TalkPosition>();
		}
	}
}