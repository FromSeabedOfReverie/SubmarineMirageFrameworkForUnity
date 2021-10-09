//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Setting {
	using System.Linq;
	using UnityEngine;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ 層の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMUnityLayerManager : SMUnityName<SMUnityLayer> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		/// <summary>AIの視界遮断マスク</summary>
		public int _aiSightObstructMask	=> LayerMask.GetMask( Get( SMUnityLayer.Ground ) );
		/// <summary>カメラ遮断マスク</summary>
		public int _cameraObstructMask	=> LayerMask.GetMask(
											Get( SMUnityLayer.Ground ), Get( SMUnityLayer.Water ) );
		/// <summary>地面マスク</summary>
		public int _groundedMask		=> LayerMask.GetMask( Get( SMUnityLayer.Ground ) );
		/// <summary>水マスク</summary>
		public int _waterMask			=> LayerMask.GetMask( Get( SMUnityLayer.Water ) );

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● マスクを取得
		/// </summary>
		public int GetMask( params SMUnityLayer[] names )
			=> LayerMask.GetMask(
				names
					.Select( type => Get( type ) )
					.ToArray()
			);

		///------------------------------------------------------------------------------------------------
		/// ● 判定
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● レイヤーが等しいか？
		/// </summary>
		public bool IsEqual( int layer, SMUnityLayer name )
			=> layer == ToInt( name );

		///------------------------------------------------------------------------------------------------
		/// ● 変換
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● レイヤー名から、番号に変換
		/// </summary>
		public int ToInt( SMUnityLayer name )
			=> LayerMask.NameToLayer( Get( name ) );
	}
}