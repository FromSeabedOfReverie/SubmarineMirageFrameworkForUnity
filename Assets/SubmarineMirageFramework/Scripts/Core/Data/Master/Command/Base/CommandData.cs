//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 命令の情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public abstract class CommandData<TGroupKey, TCommand> : CSVData<int>, ICommandData
		where TCommand : struct, Enum
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override int _registerKey => _id;

		/// <summary>読込時の行数</summary>
		public int _id	{ get; private set; }
		/// <summary>節ごとにグループにした辞書への、登録鍵</summary>
		public TGroupKey _groupKey	{ get; protected set; }
		/// <summary>グループ辞書の、鍵変更命令</summary>
		TCommand[] _groupRegisterCommands;
		/// <summary>グループ鍵を設定したか？</summary>
		public bool _isSetGroupKey	{ get; private set; }
		/// <summary>命令</summary>
		public TCommand _command { get; protected set; } = default;
		/// <summary>命令の内容一覧</summary>
		public List<string> _commands { get; protected set; } = new List<string>();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected CommandData( params TCommand[] groupRegisterCommands ) {
			_groupRegisterCommands = groupRegisterCommands;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● グループ鍵を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public object GetGroupKey() {
			return _groupKey;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 命令を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Enum GetCommand() {
			return _command;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			// 仮想関数の為、継承したが、使用しない
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void Set( string fileName, int index, List<string> texts, object groupKey ) {
			_id = index;

			// 命令を設定
			_command = texts[0].ToEnumOrDefault<TCommand>();
			texts.RemoveAt( 0 );

			// 複数命令を設定
			_commands = texts;

			// グループ鍵を設定
			_groupKey = (TGroupKey)groupKey;
			// グループ鍵登録命令の場合、グループ鍵を変更
			// 複数鍵の場合も考慮
			var equalKey = _groupRegisterCommands.FirstOrDefault( c => _command.Equals( c ) );
			if ( !equalKey.Equals( default(TCommand) ) ) {
				_isSetGroupKey = true;
				SetGroupKey( equalKey );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● グループ鍵を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected abstract void SetGroupKey( TCommand groupKey );
	}
}