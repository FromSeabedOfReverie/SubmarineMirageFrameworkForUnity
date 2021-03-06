//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 層の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class LayerManager : UnityName<LayerManager, LayerManager.Name> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>層名</summary>
		public enum Name {
			/// <summary>通常</summary>
			Default,
			/// <summary>未設定</summary>
			None,
			/// <summary>地面</summary>
			Ground,
			/// <summary>水</summary>
			Water,
			/// <summary>AI</summary>
			AI,
			/// <summary>プレイヤー</summary>
			Player,
			/// <summary>物理衝突</summary>
			Collider,
			/// <summary>遠景</summary>
			DistantView,
		}

		/// <summary>登録するか？</summary>
		public override bool _isRegister => false;
		/// <summary>AIの視界遮断マスク</summary>
		public int _aiSightObstructMask	=> LayerMask.GetMask( Get( Name.Ground ) );
		/// <summary>カメラ遮断マスク</summary>
		public int _cameraObstructMask	=> LayerMask.GetMask( Get( Name.Ground ), Get( Name.Water ) );
		/// <summary>地面マスク</summary>
		public int _groundedMask		=> LayerMask.GetMask( Get( Name.Ground ) );
		/// <summary>水マスク</summary>
		public int _waterMask			=> LayerMask.GetMask( Get( Name.Water ) );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public LayerManager() {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● レイヤー番号を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public int ToInt( Name name ) {
			return LayerMask.NameToLayer( Get( name ) );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● レイヤーが等しいか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsEqual( int layer, Name name ) {
			return layer == ToInt( name );
		}
	}
}