//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#if DEVELOP
namespace SubmarineMirageFramework.Debug {
	using System;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using Process;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグの肝心情報のクラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class DebugMainInfo {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>現在の、1秒間の画面描画回数（FPS）</summary>
		public int _fps	{ get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public DebugMainInfo() {
			// FPS計測
			Observable
				.Interval( TimeSpan.FromSeconds( 1 ) )
				.SelectMany( _ => CoreProcessManager.s_instance._updateEvent
					.Select( __ => 1 )
					.Scan( ( total, current ) => total + current )
					.Take( TimeSpan.FromSeconds( 1 ) )
					.LastOrDefault()
				)
				.Subscribe( second => _fps = second );


			// デバッグ表示
			CoreProcessManager.s_instance._updateEvent.Subscribe( _ => {
				DebugDisplay.s_instance.Add( Color.blue );
				DebugDisplay.s_instance.Add( $"FPS : {_fps}" );
				DebugDisplay.s_instance.Add( $"GC : {GC.CollectionCount( 0 )}" );
//				DebugDisplay.s_instance.Add( $"Memory System : {SystemInfo.systemMemorySize} MB" );
//				DebugDisplay.s_instance.Add( $"Memory Graphics : {SystemInfo.graphicsMemorySize} MB" );
				DebugDisplay.s_instance.Add( Color.white );
			} );
		}
	}
}
#endif