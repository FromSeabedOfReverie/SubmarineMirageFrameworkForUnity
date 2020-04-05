//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Utility {
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using Process;
	///====================================================================================================
	/// <summary>
	/// ■ 看板のクラス
	///----------------------------------------------------------------------------------------------------
	///		3次元空間に配置された、2次元画像を、必ずカメラ方向に表を向けるように、制御する。
	/// </summary>
	///====================================================================================================
	public class Billboard : MonoBehaviourProcess {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Billboard() {
			// ● 初期化
			_initializeEvent += async () => {
				await UniTask.Delay( 0 );
			};

			// ● 更新
			_updateEvent.Subscribe(
				_ => {
					transform.rotation =
						Quaternion.LookRotation( Camera.main.transform.position - transform.position )
						* Quaternion.Euler(0, 180, 0);
				}
			);
		}
	}
}