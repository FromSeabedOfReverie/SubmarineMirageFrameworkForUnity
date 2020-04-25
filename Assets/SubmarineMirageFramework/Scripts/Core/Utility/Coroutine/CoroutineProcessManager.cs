//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Utility {
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;
	using Process.New;
	using Singleton.New;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチン処理の管理クラス
	///		コルーチン処理を一括管理し、監視する。
	/// </summary>
	///====================================================================================================
	public class CoroutineProcessManager : Singleton<CoroutineProcessManager> {
		///------------------------------------------------------------------------------------------------
		/// 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>処理の型</summary>
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		/// <summary>全コルーチン処理一覧</summary>
		readonly List<CoroutineProcess> _coroutines = new List<CoroutineProcess>();
		///------------------------------------------------------------------------------------------------
		/// 生成
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public CoroutineProcessManager() {
			_disposables.AddLast( _coroutines );
			_disposables.AddLast( () => _coroutines.Clear() );
		}
		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
#if DEVELOP && false
			// デバッグ表示を設定
			ProcessRunner.s_instance._updateEvent.AddLast().Subscribe( _ => {
				DebugDisplay.s_instance.Add( $"{this.GetAboutName()}" );
				_coroutines.ForEach( p => DebugDisplay.s_instance.Add( $"\t{p.GetAboutName()}" ) );
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// 登録
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public void Register( CoroutineProcess process ) => _coroutines.Add( process );
		/// <summary>
		/// ● 登録解除
		/// </summary>
		public void UnRegister( CoroutineProcess process ) => _coroutines.Remove( process );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    count {_coroutines.Count}\n";
			_coroutines.ForEach( ( c, i ) => result += $"    {i} : {c}\n" );
			result += $")";
			return result;
		}
	}
}