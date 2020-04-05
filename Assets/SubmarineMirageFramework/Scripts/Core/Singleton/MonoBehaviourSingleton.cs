//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton {
	using Process;
	///====================================================================================================
	/// <summary>
	/// ■ MonoBehaviour利用のシングルトンのクラス
	///----------------------------------------------------------------------------------------------------
	///		場面遷移後も存在し続ける。
	/// </summary>
	///====================================================================================================
	public class MonoBehaviourSingleton<T> : MonoBehaviourProcess where T : MonoBehaviourProcess {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>シングルトン</summary>
		static T s_instanceObject;

		/// <summary>作成済か？</summary>
		public static bool s_isCreated => s_instanceObject != null;
		/// <summary>場面内だけ存在するか？</summary>
		public override bool _isInSceneOnly => false;
		/// <summary>中心処理初期化後まで待機するか？</summary>
		public override bool _isWaitInitializedCoreProcesses => false;
		///------------------------------------------------------------------------------------------------
		/// ● アクセサ
		///------------------------------------------------------------------------------------------------
		/// <summary>シングルトン取得</summary>
		public static T s_instance {
			get {
				if ( !s_isCreated )	{ Create(); }
				return s_instanceObject;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static void Create() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<T>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = MonoBehaviourSingletonManager.s_instance.CreateComponent<T>();
		}
	}
}