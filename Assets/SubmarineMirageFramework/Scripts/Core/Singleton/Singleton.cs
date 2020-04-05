//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton {
	using UniRx.Async;
	using Extension;
	using Debug;
	using Process;
	///====================================================================================================
	/// <summary>
	/// ■ シングルトンのクラス
	///----------------------------------------------------------------------------------------------------
	///		シングルトンで更新が必要な物の更新をサポートする。
	/// </summary>
	///====================================================================================================
	public class Singleton<T> : BaseProcess where T : new() {
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
		protected static void Create() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = new T();
			Log.Debug( $"作成 : { s_instanceObject.GetAboutName() }", Log.Tag.Singleton );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成登録を待機
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static async UniTask WaitForCreation() {
			// 既に作成済の場合を考慮
			// そもそも作成済の場合は、この関数を呼ばなくても良い
			var i = s_instance;

			// BaseProcess内部の登録時に、Delay(1)をForget()で行っており、登録順がランダムになる為、
			// ここで念の為、もう1ミリ秒待機させる
			await UniTask.Delay( 1 );
		}
	}
}