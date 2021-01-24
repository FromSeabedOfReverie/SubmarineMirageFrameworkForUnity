//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMLog
namespace SubmarineMirage.Debug {
	using UnityEngine;
	using Base;
	using Service;
	using Extension;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ記録のクラス
	///		UnityエディタのConsole窓から、クリックで飛べない為、DLL化した基盤クラスを継承する。
	/// </summary>
	///====================================================================================================
	public class SMLog : BaseSMLog<SMLogTag>, ISMLightBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>識別番号</summary>
		[SMShowLine] public uint _id	{ get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMLog() : base(
#if DEVELOP
				true,
#else
				false,
#endif
#if UNITY_EDITOR
				true
#else
				false
#endif
		) {
			_id = BaseSMManager.s_instance.GetNewID( this );

#if TestSMLog
			SMLog.Debug( SMLogTag.Task,				SMLogTag.Task );
			SMLog.Debug( SMLogTag.Singleton,		SMLogTag.Singleton );
			SMLog.Debug( SMLogTag.FSM,				SMLogTag.FSM );
			SMLog.Debug( SMLogTag.Build,			SMLogTag.Build );
			SMLog.Debug( SMLogTag.Server,			SMLogTag.Server );
			SMLog.Debug( SMLogTag.Advertisement,	SMLogTag.Advertisement );
			SMLog.Debug( SMLogTag.Purchase,			SMLogTag.Purchase );
			SMLog.Debug( SMLogTag.Data,				SMLogTag.Data );
			SMLog.Debug( SMLogTag.File,				SMLogTag.File );
			SMLog.Debug( SMLogTag.Scene,			SMLogTag.Scene );
			SMLog.Debug( SMLogTag.Audio,			SMLogTag.Audio );
			SMLog.Debug( SMLogTag.UI,				SMLogTag.UI );
			SMLog.Debug( SMLogTag.AI,				SMLogTag.AI );
			SMLog.Debug( SMLogTag.GameObject,		SMLogTag.GameObject );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● フォーマットを装飾
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override string DecorationFormat( string format, Color color )
			=> SMServiceLocator.Resolve<SMDecorationManager>()._uGUI.ByColor( format, color );
		///------------------------------------------------------------------------------------------------
		/// ● 文章に変換
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章に変換
		/// </summary>
		public override string ToString() => ToString( 0 );
		/// <summary>
		/// ● 文章に変換（インデントを整列）
		/// </summary>
		public virtual string ToString( int indent ) => this.ToShowString( indent );
		/// <summary>
		/// ● 1行文章に変換（インデントを整列）
		/// </summary>
		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}