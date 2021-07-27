//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestCoroutine
namespace SubmarineMirage.Utility {
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;
	using Service;
	using Task;
	using Debug;
	using Extension;


// TODO : コルーチンをデバッグする


	///====================================================================================================
	/// <summary>
	/// ■ コルーチンの管理クラス
	///		コルーチンを一括管理し、監視する。
	/// </summary>
	///====================================================================================================
	public class SMCoroutineManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Dont;

		/// <summary>全コルーチン処理一覧</summary>
		[SMShow] readonly LinkedList<SMCoroutine> _coroutines = new LinkedList<SMCoroutine>();
#region ToString
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章変換を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _coroutines ), i => _toStringer.DefaultValue( _coroutines, i, true ) );
		}
#endregion
		///------------------------------------------------------------------------------------------------
		/// 生成
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMCoroutineManager() {
			_disposables.AddFirst( () => {
				_coroutines.ForEach( c => c.Dispose() );
				_coroutines.Clear();
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
#if TestCoroutine
			// デバッグ表示を設定
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();

			_taskManager._updateEvent.AddLast().Subscribe( _ => {
				displayLog.Add( $"{this.GetAboutName()}" );
				_coroutines.ForEach( c => displayLog.Add( $"\t{c}" ) );
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// 登録
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public void Register( SMCoroutine task )
			=> _coroutines.AddLast( task );

		/// <summary>
		/// ● 登録解除
		/// </summary>
		public void Unregister( SMCoroutine task )
			=> _coroutines.Remove( task );
	}
}