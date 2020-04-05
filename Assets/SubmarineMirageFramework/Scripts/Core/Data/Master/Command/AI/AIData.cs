//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System;
	using System.Collections.Generic;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ AIの情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class AIData : CommandData<string, AIData.Command> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>命令の型</summary>
		public enum Command {
			/// <summary>会話</summary>
			Talk,
			/// <summary>節</summary>
			Node,
			/// <summary>名前</summary>
			Name,
			/// <summary>画像名</summary>
			Image,
			/// <summary>待機</summary>
			Wait,
			/// <summary>自動実行</summary>
			Auto,
			/// <summary>割込み禁止</summary>
			InterruptBan,
			/// <summary>会話位置変更</summary>
			Position,
			/// <summary>ランダム会話</summary>
			Random,
			/// <summary>選択会話</summary>
			Select,
			/// <summary>ランダム節遷移</summary>
			RandomNode,
			/// <summary>節遷移</summary>
			Next,
		}

		/// <summary>AI命令情報</summary>
		public AICommandData _aiCommand	{ get; private set; }
		/// <summary>最後に選択した要素番号</summary>
		public int _lastChoiceIndex	{ get; protected set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AIData() : base( Command.Node ) {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts, object groupKey ) {
			base.Set( fileName, index, texts, groupKey );

			_aiCommand = new Func<AICommandData>( () => {
				switch ( _command ) {
					case Command.Node:				return new NodeAICommandData( _commands );
					case Command.Name:				return new NameAICommandData( _commands );
					case Command.Image:				return new ImageAICommandData( _commands );
					case Command.Wait:				return new WaitAICommandData( _commands );
					case Command.Auto:				return new AutoAICommandData( _commands );
					case Command.InterruptBan:		return new InterruptBanAICommandData( _commands );
					case Command.Position:			return new PositionAICommandData( _commands );
					case Command.Random:			return new RandomAICommandData( _commands );
					case Command.Select:			return new SelectAICommandData( _commands );
					case Command.RandomNode:		return new RandomNodeAICommandData( _commands );
					case Command.Next:				return new NextAICommandData( _commands );
					case Command.Talk:	default:	return new TalkAICommandData( _commands );
				}
			} )();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● グループ鍵を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void SetGroupKey( Command groupKey ) {
			_groupKey = _commands[0];
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ランダムに命令を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string GetRandomCommand() {
			// 要素が１つしか無い場合、そのまま返す
			if ( _commands.Count == 1 )	{ return _commands[0]; }

			// 違う要素になるまで、繰り返しランダム選択
			while ( true ) {
				var i = UnityEngine.Random.Range( 0, _commands.Count );
				if ( i != _lastChoiceIndex ) {
					_lastChoiceIndex = i;
					return _commands[_lastChoiceIndex];
				}
			}
		}
	}
}