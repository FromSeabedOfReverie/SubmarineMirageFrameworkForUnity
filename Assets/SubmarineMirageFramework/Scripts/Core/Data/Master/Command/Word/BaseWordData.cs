//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 単語情報の基盤クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class BaseWordData : CommandData<AllWordDataManager.WordDepth, BaseWordData.Command> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>命令の型</summary>
		public enum Command {
			/// <summary>単語読込の命令</summary>
			Word,
			/// <summary>深度変更の命令</summary>
			Depth,
		}

		/// <summary>読込深度</summary>
		public int _depth => (int)_groupKey;
		/// <summary>複数の単語表現と一致した場合、最終的に出力する単語</summary>
		public string _extractionWord	{ get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public BaseWordData() : base( Command.Depth ) {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts, object groupKey ) {
			base.Set( fileName, index, texts, groupKey );

			// 単語登録の場合
			if ( _command == Command.Word ) {
				_extractionWord = _commands[0];
				_commands.RemoveAt( 0 );
				SetWord();
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 単語を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected virtual void SetWord() {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● グループ鍵を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void SetGroupKey( Command groupKey ) {
			if ( groupKey == Command.Depth ) {
				_groupKey = _commands[0].ToEnum<AllWordDataManager.WordDepth>();
			}
		}
	}
}