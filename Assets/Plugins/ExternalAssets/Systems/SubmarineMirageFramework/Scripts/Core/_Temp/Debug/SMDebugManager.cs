//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using UnityEngine;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグの管理クラス
	/// </summary>
	///====================================================================================================
	public class SMDebugManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShow] public const bool IS_DEVELOP =
#if DEVELOP
				true;
#else
				false;
#endif
		[SMShow] public const bool IS_UNITY_EDITOR =
#if UNITY_EDITOR
				true;
#else
				false;
#endif

		[SMShow] public static bool s_isPlayTest { get; set; }

		/// <summary>実行型</summary>
		public override SMTaskRunType _type => SMTaskRunType.Sequential;
		/// <summary>デバッグ記録</summary>
		readonly SMLog _log = new SMLog();
		/// <summary>現在の、1秒間の画面描画回数（FPS）</summary>
		public int _fps { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMDebugManager() {
			_disposables.AddFirst( () => {
				_log.Dispose();
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			// FPS計測
			var checkSeconds = 0f;
			var count = 0;
			_updateEvent.AddLast().Subscribe( _ => {
				if ( checkSeconds < Time.time ) {
					checkSeconds = Time.time + 1;
					_fps = count;
					count = 0;
				}
				count++;
			} );


			if ( !IS_DEVELOP )	{ return; }

			// デバッグ表示
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
			_updateEvent.AddLast().Subscribe( _ => {
				displayLog.Add( Color.cyan );
				displayLog.Add( $"FPS : {_fps}" );
				displayLog.Add( $"GC : {GC.CollectionCount( 0 )}" );
				displayLog.Add( $"Memory System : {SystemInfo.systemMemorySize} MB" );
				displayLog.Add( $"Memory Graphics : {SystemInfo.graphicsMemorySize} MB" );
				displayLog.Add( Color.white );
			} );
		}
	}
}