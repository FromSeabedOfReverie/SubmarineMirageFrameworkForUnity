//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using UnityEngine;
	using Task;
	///====================================================================================================
	/// <summary>
	/// ■ 層の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMLayerManager : SMUnityName<SMLayerManager.Name> {
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

		/// <summary>処理の型</summary>
		public override SMTaskType _type => SMTaskType.DontWork;

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
		/// ● 作成
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Create() {}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● レイヤー番号を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public int ToInt( Name name ) => LayerMask.NameToLayer( Get( name ) );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● レイヤーが等しいか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsEqual( int layer, Name name ) => layer == ToInt( name );
	}
}