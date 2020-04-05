//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process {
	using System.Collections.Generic;
	using UniRx;
	using Singleton;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチン処理の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		コルーチン処理を一括管理し、監視する。
	/// </summary>
	///====================================================================================================
	public class CoroutineProcessManager : Singleton<CoroutineProcessManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>管理クラスに登録するか？</summary>
		public override bool _isRegister => false;

		/// <summary>全コルーチン処理一覧</summary>
		readonly List<CoroutineProcess> _processes = new List<CoroutineProcess>();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CoroutineProcessManager() {
#if DEVELOP && false
			// デバッグ表示を設定
			CoreProcessManager.s_instance._updateEvent.Subscribe( _ => {
				DebugDisplay.s_instance.Add( $"{this.GetAboutName()}" );
				_processes.ForEach( p => DebugDisplay.s_instance.Add( $"\t{p.GetAboutName()}" ) );
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Register( CoroutineProcess process ) {
			_processes.Add( process );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録解除
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void UnRegister( CoroutineProcess process ) {
			_processes.Remove( process );
		}
	}
}