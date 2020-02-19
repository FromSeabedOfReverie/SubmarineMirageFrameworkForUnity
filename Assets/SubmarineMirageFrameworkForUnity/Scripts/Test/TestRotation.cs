//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using UnityEngine;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 回転の試験クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TestRotation {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>回転角度（度数）</summary>
		public Vector3 _angle;
		/// <summary>回転角度（4元数）</summary>
		public Quaternion _rotation;
		///------------------------------------------------------------------------------------------------
		/// ● コンストラクタ
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（引数無し）
		/// </summary>
		public TestRotation() {
		}
		/// <summary>
		/// ● コンストラクタ（成分）
		/// </summary>
		public TestRotation( float x, float y, float z ) {
			Initialize( x, y, z );
		}
		/// <summary>
		/// ● コンストラクタ（ベクトル）
		/// </summary>
		public TestRotation( Vector3 angle ) {
			Initialize( angle );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 初期化
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化（成分）
		/// </summary>
		public void Initialize( float x, float y, float z ) {
			Initialize( new Vector3( x, y, z ) );
		}
		/// <summary>
		/// ● 初期化（ベクトル）
		/// </summary>
		public void Initialize( Vector3 angle ) {
			_angle = angle;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Update() {
			// X軸を入力により回転
			_angle.x = Mathf.Repeat(
				_angle.x +
					( Input.GetKey( KeyCode.Keypad1 ) ? 1 : 0 ) +
					( Input.GetKey( KeyCode.Keypad4 ) ? -1 : 0 ),
				360
			);
			// Y軸入力により回転
			_angle.y = Mathf.Repeat(
				_angle.y +
					( Input.GetKey( KeyCode.Keypad2 ) ? 1 : 0 ) +
					( Input.GetKey( KeyCode.Keypad5 ) ? -1 : 0 ),
				360
			);
			// Z軸入力により回転
			_angle.z = Mathf.Repeat(
				_angle.z +
					( Input.GetKey( KeyCode.Keypad3 ) ? 1 : 0 ) +
					( Input.GetKey( KeyCode.Keypad6 ) ? -1 : 0 ),
				360
			);

			_rotation = Quaternion.Euler( _angle );	// 回転角度を4元数に変換
			Log.Debug( _angle );					// 角度を表示
		}
	}
}