//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System.Collections;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using Service;
	using Scene;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ画面表示のクラス
	///----------------------------------------------------------------------------------------------------
	///		アプリケーション画面の最前面に、デバック文字を表示する。
	/// </summary>
	///====================================================================================================
	public class DebugDisplay : SMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>描画するか？</summary>
		public bool _isDraw = true;
		/// <summary>背景描画するか？</summary>
		public bool _isDrawBackground = true;
		/// <summary>描画スタイル</summary>
		GUIStyle _guiStyle = new GUIStyle();
		/// <summary>登録用、文章配列</summary>
		readonly ArrayList _texts = new ArrayList();
		/// <summary>描画用、文章配列</summary>
		ArrayList _drawTexts = new ArrayList();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public DebugDisplay() {
#if DEVELOP
			// 描画スタイルを設定
			_guiStyle.normal.background = Texture2D.whiteTexture;
			_guiStyle.normal.textColor = Color.white;

/*
			// デバッグ表示キー押下の場合
			InputManager.s_instance.GetPressedEvent( InputManager.Event.DebugView )
				.Subscribe( _ => {
					// 設定を保存
					var setting = AllDataManager.s_instance._save._setting;
					setting._data._isViewDebug = !setting._data._isViewDebug;
					setting.Save().Forget();

					// 描画切り替え
					_isDraw = setting._data._isViewDebug;
				} );
*/

			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			// ● 更新（遅延）
			//	※毎回、文章登録を初期化する為、全プログラムの一番最後に呼ぶべき。
			sceneManager._lateUpdateEvent.AddLast().Subscribe( _ => {
				// 毎フレーム登録を更新する
				_drawTexts = new ArrayList( _texts );
				_texts.Clear();
			} );


			// ● GUI描画
			sceneManager._onGUIEvent.AddLast().Subscribe( _ => {
				if ( !_isDraw )	{ return; }

				// 背景描画開始
				GUI.backgroundColor = new Color( 0, 0, 0, ( _isDrawBackground ? 0.3f : 0 ) );

				var screenSize = new Vector2( Screen.width, Screen.height );
				var fontSize = (int)Mathf.Round( screenSize.y / 30 );
				_guiStyle.fontSize = fontSize;
				var marginSize = fontSize / 2;
				var width = screenSize.x - marginSize * 2;
				var height = fontSize + marginSize / 2;
				var rect = new Rect( marginSize, marginSize, width, height );

				// 描画文章配列を走査し、画面に描画
				foreach ( var text in _drawTexts ) {
					// 色の場合
					if ( text is Color ) {
						_guiStyle.normal.textColor = (Color)text;

						// 文章の場合
					} else if ( text is string ) {
						GUI.Label( rect, (string)text, _guiStyle );
						rect.y += height;
					}
				}

				// 背景描画終了
				GUI.backgroundColor = new Color( 0, 0, 0, 0 );
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 追加（文章）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Add( string text ) {
#if DEVELOP
			_texts.Add( text );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 追加（色）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Add( Color color ) {
#if DEVELOP
			_texts.Add( color );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 追加（改行）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void AddLine() {
#if DEVELOP
			_texts.Add( "\n" );
#endif
		}
	}
}