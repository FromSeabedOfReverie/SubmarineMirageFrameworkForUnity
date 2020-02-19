//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 線の試験クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TestLine {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>最後の線の識別番号（線の個数を表す）</summary>
		static int s_maxID = 0;
		/// <summary>線の識別番号</summary>
		int _id;
		/// <summary>線の描画者</summary>
		LineRenderer _line;
		/// <summary>ゲーム物</summary>
		GameObject _gameObject;
		///------------------------------------------------------------------------------------------------
		/// ● アクセサ
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 色を設定
		/// </summary>
		public void SetColor( Color color ) {
			_line.material.color = color;
		}
		///------------------------------------------------------------------------------------------------
		/// ● コンストラクタ
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（引数無し）
		/// </summary>
		public TestLine() {
//			Initialize();
		}
		/// <summary>
		/// ● コンストラクタ（色）
		/// </summary>
		public TestLine( Color color ) {
//			Initialize(color);
		}
		///------------------------------------------------------------------------------------------------
		/// ● 初期化
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化（引数無し）
		/// </summary>
		public void Initialize() {
			_id = s_maxID;
			_gameObject = new GameObject( "TestLine" + _id );
			_line = _gameObject.gameObject.AddComponent<LineRenderer>();
			_line.SetWidth( 0.1f, 0.1f );
			_line.SetVertexCount( 2 );
			s_maxID++;
		}
		/// <summary>
		/// ● 初期化（色）
		/// </summary>
		public void Initialize( Color color ) {
			Initialize();
			SetColor( color );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Update( Vector3 position, Vector3 direction ) {
			_line.SetPosition( 0, position );
			_line.SetPosition( 1, position + direction );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		~TestLine() {
//			Object.Destroy( _gameObject );
		}
	}
}