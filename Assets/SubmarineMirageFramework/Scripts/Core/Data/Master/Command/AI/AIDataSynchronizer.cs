//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------

// まだ、リファクタリング中
#if false
namespace SubmarineMirageFramework.Data {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using KoganeUnityLib;
	using State_Position = UITalk.StatePosition;
	using UniRx;
	using UniRx.Async;
	using Process;
	using Audio;
	using Debug;
	using Command = AIData.Command;
	///====================================================================================================
	/// <summary>
	/// ■ AI情報の同期クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class AIDataSynchronizer : MonoBehaviourProcess {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		// 全てのAI同期クラス
		public static readonly List<AIDataSynchronizer> s_allSynchronizer = new List<AIDataSynchronizer>();

		int _index;										// 文章配列の参照位置
		public string _fileName;						// ファイル名
		public string _sceneName	{get; private set; }	// シーン名
		[HideInInspector] public string _viewName;		// 表示する名前
		[HideInInspector] public string _imageName;		// 画像名
		[HideInInspector] public string _presentText;	// 現在表示文章
		[HideInInspector] public AIData _presentAICommand;	// 現在のＡＩ文章クラス
		string _voicePath;								// 声音の階層
		[HideInInspector] public float _waitSecond;		// 待機時間（秒）
		static int s_maxID;								// 最大のオブジェクト番号（デバッグ用）
		int _id;											// オブジェクト番号（デバッグ用）
		[HideInInspector] public bool _isAuto;			// 自動実行されるか？
		bool _isNextCommand;							// 次の文章へ遷移するか？
		bool _isInterruptBan;							// 割り込みを禁止するか？
		AllAIDataManager _aiData;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AIDataSynchronizer() {
			// ● 初期化
			_initializeEvent += async () => {
				_aiData = AllDataManager.s_instance.Get<AllAIDataManager>();

				// オブジェクト番号を設定
				_id = ++s_maxID;

				_voicePath = "";
				Dispose();

				SetAIFile( _fileName );
				s_allSynchronizer.Add( this );
				await UniTask.Delay( 0 );
			};


			// ● 更新
			_updateEvent.Subscribe( _ => {
				// 待機時間が設定されている場合、待機
				if ( _waitSecond > 0 ) {
					_waitSecond -= Time.deltaTime;
		
				// 自動実行中か、次の文章へ遷移する場合
				} else if ( _isAuto || _isNextCommand ) {
					_isNextCommand = false;

					// 文章が存在し、声音が未再生の場合
					if ( !IsFinish() && !AudioManager.s_instance.voice.is_playing() ) {
						// 最後まで再生した場合、削除
						if ( _index >= _aiData.Get( _fileName, _sceneName ).Count ) {
							UITalk.get_instance().stop( this );
							Dispose();

						// それ以外の場合、文章命令を実行
						} else {
							ExecutionText();
						}
					}
				}
#if DEVELOP
				DebugDisplay.s_instance.Add( Color.blue );
				DebugDisplay.s_instance.Add( $"{gameObject.name} : {_sceneName}" );
#endif
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読み込むAIファイルを設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetAIFile( string fileName ) {
			_fileName = fileName;

			// 顔設定を検索
			var commands = _aiData.Get( _fileName, "Setting" );

			var data = commands.First( c => c._command == Command.Name );
			if ( data != null ) {
				_viewName = data._commands.Any() ? data._commands[0] : string.Empty;
			}
			data = commands.First( c => c._command == Command.Image );
			if ( data != null ) {
				_imageName = data._commands.Any() ? data._commands[0] : string.Empty;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章命令を実行
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void ExecutionText() {
			if ( _sceneName == "" )	{ return; }
		
			_presentText = "";	// 現在文章を初期化


			// AI 文章を取得
			_presentAICommand = _aiData.Get( _fileName, _sceneName )[_index];
			_index++;	// 文章配列位置を更新


			// 文章タグで分岐
			switch ( _presentAICommand._command ) {
				// 待機命令の場合、待機時間を設定
				case Command.Wait:
					_waitSecond = _presentAICommand._commands[0].ToFloat();
					_isNextCommand = true;
					break;

				// 自動実行命令の場合、文章の自動送りを設定
				case Command.Auto:
					_isAuto = _presentAICommand._commands[0].ToBoolean();
					_isNextCommand = true;
					Update();
					break;

				// 割り込み禁止命令の場合、割り込み禁止を設定
				case Command.InterruptBan:
					_isInterruptBan = _presentAICommand._commands[0].ToBoolean();
					_isNextCommand = true;
					Update();
					break;

				// 位置変更命令の場合、文章位置を設定
				case Command.Position:
					State_Position pos = State_Position.LOWER;
					switch ( _presentAICommand._commands[0] ) {
						case "Upper":	pos = State_Position.UPPER;		break;
						case "Middle":	pos = State_Position.MIDDLE;	break;
						case "Lower":	pos = State_Position.LOWER;		break;
					}
					UITalk.get_instance().change_state_position( pos );
					_isNextCommand = true;
					Update();
					break;

				// シーン変更命令の場合
				case Command.Next:
					// 自身のシーン遷移を行う場合
					if ( _presentAICommand._commands.Count == 1 )
						StartScene( _presentAICommand._commands[0] );
					// 他者のシーン遷移を行う場合
					else {
						// 全 AI 同期クラスを、表示名で検索
						var name = _presentAICommand._commands[1];
						if ( name == "NoName" )	{ name = ""; }
						var target = s_allSynchronizer.First( ai => ai != null && ai._viewName == name );
						target?.StartScene( _presentAICommand._commands[0] );
					}
					break;

				// ランダム命令の場合
				case Command.Random:
					// 現在文章をランダム設定
					Play( _presentAICommand.GetRandomCommand() );
					break;

				// それ以外の場合
				default:
					// 文章を再生
					Play( _presentAICommand._commands[0] );
					break;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章を再生
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Play( string text ) {
			_presentText = text;
			// 声音、アニメ用に改行を削除
			text = text.Replace( "\r", "" ).Replace( "\n", "" );

			UITalk.get_instance().play( this );
		
//			PlayVoice( ref text );	// 声音を再生
//			PlayUnderVoice();		// CSV 下列の声音を再生

			// 自動実行中の場合、少し待機する
			if ( _isAuto ) {
				_waitSecond = Mathf.Max( text.Length / 8.0f, 1 );
//				Log.Debug( _waitSecond, Log.Tag.Data );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章を再度再生
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Replay() {
			// 現在声音データの場合、2 つ戻る
			var subValue = _presentAICommand._command == Command.Talk ? 2 : 1;
			_index = Mathf.Max( _index - subValue, 0 );	// 0 以下にならないように補正
			ExecutionText();	// 文章命令を実行
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● CSV下列の声音を再生
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void PlayUnderVoice() {
			// 声音の参照位置を設定
			// ランダム文章の場合、要素番号、通常文章の場合、先頭番号を設定
			var voiceIndex = _presentAICommand._command == Command.Random ?
				_presentAICommand._lastChoiceIndex : 0;

			// AI 文章を取得
			_presentAICommand = (AIData)_aiData.Get( _fileName, _sceneName )[_index];
			_index++;	// 文章配列位置を更新

			// 声音を再生
			PlayVoice( _presentAICommand._commands[voiceIndex] );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 声音を再生
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void PlayVoice( string text, string subPath = null ) {
			if ( subPath == null )	{ subPath = _voicePath; }
			AudioManager.s_instance._voice.play( text, subPath );	// 声音を再生
		
			// 声音が無い場合、文字数を考慮して、待機
			if ( !AudioManager.s_instance._voice.isLoading( ref text ) )
				_waitSecond = Mathf.Max( text.Length / 6.0f, 2 );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章が終了したか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsFinish() {
			return _sceneName.IsNullOrEmpty();	// シーン名が未設定か？
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● シーンを開始
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void StartScene( string name ) {
			Dispose();		// 前回分を消去

			// シーン名が存在する場合
			if ( _aiData.IsScene( _fileName, name ) ) {
//				_voicePath = $"{name}/";	// 声音フォルダ階層を設定
				_sceneName = name;			// シーン名を設定
				ExecutionText();			// 文章命令を実行
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Dispose() {
			// 文章が終了していない場合
			if ( !IsFinish() ) {
				AudioManager.s_instance._voice.clear();		// 声音を消去
				_index = 0;									// 参照位置を初期化
				_sceneName = "";							// シーン名を初期化
				_presentText = "";							// 現在表示文章を初期化
				_waitSecond = 0;							// 待機時間（秒）を初期化
				_presentAICommand = new AIData();			// ＡＩ文章を初期化
				_isAuto = false;							// 自動実行するか？を初期化
				_isNextCommand = false;						// 次の文章へ遷移するか？を初期化
				_isInterruptBan = false;					// 割り込み禁止するか？を初期化
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 全解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static void DisposeAll() {
			AudioManager.s_instance.voice.clear();	// 声音を消去
			s_allSynchronizer.Clear();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ボタンが押された場合
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void OnClickButton( string buttonName ) {
			if ( _waitSecond > 0 )	{ return; }
			_waitSecond = 0.3f;
			_isNextCommand = true;
		
			switch ( buttonName ) {
				case "Next":
					break;

				case "Yes":
					if ( _presentAICommand._commands.Count > 1 )
						StartScene( _presentAICommand._commands[1] );
					break;

				case "No":
					if ( _presentAICommand._commands.Count > 2 )
						StartScene( _presentAICommand._commands[2] );
					break;

				case "Ignore":
					// 選択肢文章の場合
					if ( _presentAICommand._command == Command.Select ) {
						// 無視の選択肢が、存在する場合
						if ( _presentAICommand._commands.Count > 3 )
							StartScene( _presentAICommand._commands[3] );
						// 否定の選択肢が、存在する場合
						else if ( _presentAICommand._commands.Count > 2 )
							StartScene( _presentAICommand._commands[2] );

					// ＡＩファイルに共通無視項目が、存在する場合
					} else if ( _aiData.IsScene( _fileName, "Ignore" ) )
						StartScene( "Ignore" );

					// ＡＩファイルに何も書かれていない場合、会話を終了
					else {
						UITalk.get_instance().stop( this );
						Dispose();
					}
					break;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 停止
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool Stop( ref AIDataSynchronizer other ) {
			// 割り込み禁止の場合、停止しない
			if ( _isInterruptBan )	{ return false; }
		
			if ( _aiData.IsScene( _fileName, "Stop" ) ) {
				StartScene( "Stop" );
			}
			return true;
		}
	}
}
#endif