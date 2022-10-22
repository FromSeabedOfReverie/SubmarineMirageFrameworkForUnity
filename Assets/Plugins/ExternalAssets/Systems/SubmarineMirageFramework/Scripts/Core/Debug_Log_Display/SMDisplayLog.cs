//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System.Collections;
	using UnityEngine;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ画面記録のクラス
	///		アプリケーション画面の最前面に、デバック文字を表示する。
	/// </summary>
	///====================================================================================================
	public class SMDisplayLog : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Dont;
		/// <summary>描画するか？</summary>
		public bool _isDraw { get; set; } = true;
		/// <summary>背景描画するか？</summary>
		public bool _isDrawBackground { get; set; } = true;
		/// <summary>描画スタイル</summary>
		readonly GUIStyle _guiStyle = new GUIStyle();
		/// <summary>登録用、文章配列</summary>
		readonly ArrayList _texts = new ArrayList();
		/// <summary>描画用、文章配列</summary>
		ArrayList _drawTexts { get; set; } = new ArrayList();
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			if ( !SMDebugManager.IS_DEVELOP )	{ return; }


			// 描画スタイルを設定
			_guiStyle.normal.background = Texture2D.whiteTexture;
			_guiStyle.normal.textColor = Color.white;


			// ● 更新（遅延）
			//	※毎回、文章登録を初期化する為、全プログラムの一番最後に呼ぶべき。
			_taskManager._lateUpdateEvent.AddLast( nameof( SMDisplayLog ) ).Subscribe( _ => {
				// 毎フレーム登録を更新する
				_drawTexts = new ArrayList( _texts );
				_texts.Clear();
			} );


			// ● GUI描画
			_taskManager._onGUIEvent.AddLast( nameof( SMDisplayLog ) ).Subscribe( _ => {
				if ( !_isDraw )	{ return; }

				// 背景描画開始
				GUI.backgroundColor = new Color( 0, 0, 0, ( _isDrawBackground ? 0.3f : 0 ) );

				var screenSize = new Vector2( Screen.width, Screen.height );
				var fontSize = ( int )Mathf.Round( screenSize.y / 30 );
				_guiStyle.fontSize = fontSize;
				var marginSize = fontSize / 2;
				var width = screenSize.x - marginSize * 2;
				var height = fontSize + marginSize / 2;
				var rect = new Rect( marginSize, marginSize, width, height );

				// 描画文章配列を走査し、画面に描画
				foreach ( var text in _drawTexts ) {
					switch ( text ) {
						// 色の場合
						case Color c:
							_guiStyle.normal.textColor = c;
							break;

						// 文章の場合
						case string s:
							GUI.Label( rect, s, _guiStyle );
							rect.y += height;
							break;
					}
				}

				// 背景描画終了
				GUI.backgroundColor = new Color( 0, 0, 0, 0 );
			} );


			// 破棄処理
			_disposables.AddFirst( () => {
				_taskManager._lateUpdateEvent.Remove( nameof( SMDisplayLog ) );
				_taskManager._onGUIEvent.Remove( nameof( SMDisplayLog ) );

				_texts.Clear();
				_drawTexts.Clear();
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 追加
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 追加（文章）
		/// </summary>
		public void Add( string text ) {
			if ( !SMDebugManager.IS_DEVELOP )	{ return; }

			_texts.Add( text );
		}
		/// <summary>
		/// ● 追加（色）
		/// </summary>
		public void Add( Color color ) {
			if ( !SMDebugManager.IS_DEVELOP )	{ return; }

			_texts.Add( color );
		}
		/// <summary>
		/// ● 追加（改行）
		/// </summary>
		public void AddLine() {
			if ( !SMDebugManager.IS_DEVELOP )	{ return; }

			_texts.Add( "\n" );
		}
	}
}