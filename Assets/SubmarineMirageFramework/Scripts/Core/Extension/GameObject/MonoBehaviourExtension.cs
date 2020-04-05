//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ モノ動作の拡張クラス
	///----------------------------------------------------------------------------------------------------
	///		MonoBehaviourを使うときは、必ずこれを継承する。
	/// </summary>
	///====================================================================================================
	public class MonoBehaviourExtension : MonoBehaviour {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>変形のキャッシュ</summary>
		Transform _transform;
		/// <summary>変形のキャッシュを取得</summary>
		public new Transform transform {
			get {
				if ( _transform == null )	{ _transform = base.transform; }
				return _transform;
			}
		}
		/// <summary>ゲーム物のキャッシュ</summary>
		GameObject _gameObject;
		/// <summary>ゲーム物のキャッシュを取得</summary>
		public new GameObject gameObject {
			get {
				if ( _gameObject == null )	{ _gameObject = base.gameObject; }
				return _gameObject;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ToString() {
			return this.ToDeepString();
		}
	}
}