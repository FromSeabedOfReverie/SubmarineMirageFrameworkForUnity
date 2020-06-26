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
	using SMTask;
	using Singleton.New;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチン仕事の管理クラス
	///		コルーチン仕事を一括管理し、監視する。
	/// </summary>
	///====================================================================================================
	public class CoroutineTaskManager : Singleton<CoroutineTaskManager> {
		///------------------------------------------------------------------------------------------------
		/// 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>処理の型</summary>
		public override SMTaskType _type => SMTaskType.DontWork;
		/// <summary>全コルーチン処理一覧</summary>
		readonly List<CoroutineTask> _coroutines = new List<CoroutineTask>();
		///------------------------------------------------------------------------------------------------
		/// 生成
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public CoroutineTaskManager() {
			_disposables.AddLast( _coroutines );
			_disposables.AddLast( () => _coroutines.Clear() );
		}
		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
#if DEVELOP && false
			// デバッグ表示を設定
			SMTaskRunner.s_instance._updateEvent.AddLast().Subscribe( _ => {
				DebugDisplay.s_instance.Add( $"{this.GetAboutName()}" );
				_coroutines.ForEach( t => DebugDisplay.s_instance.Add( $"\t{t.GetAboutName()}" ) );
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// 登録
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public void Register( CoroutineTask task ) => _coroutines.Add( task );
		/// <summary>
		/// ● 登録解除
		/// </summary>
		public void UnRegister( CoroutineTask task ) => _coroutines.Remove( task );
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