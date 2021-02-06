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
	using MultiEvent;
	using Utility;
	using Debug.ToString;



	// TODO : コメント追加、整頓



	///====================================================================================================
	/// <summary>
	/// ■ デバッグ記録のクラス
	///		UnityエディタのConsole窓から、クリックで飛べない為、DLL化した基盤クラスを継承する。
	/// </summary>
	///====================================================================================================
	public class SMLog : BaseSMLog<SMLogTag>, ISMStandardBase, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShowLine] public uint _id	{ get; private set; }
		[SMHide] public SMMultiDisposable _disposables	{ get; private set; } = new SMMultiDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		[SMHide] public SMToStringer _toStringer	{ get; private set; }
		[SMHide] SMDecorationManager _decorationManager	{ get; set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMLog() : base( DebugSetter.s_isUnityEditor ) {
			_id = SMIDCounter.GetNewID( this );

			_toStringer = new SMToStringer( this );
			SetToString();

			_disposables.AddLast( () => {
				base.Dispose();
				_toStringer.Dispose();
			} );

#if TestSMLog
			SMLog.Debug( SMLogTag.Task,				SMLogTag.Task );
			SMLog.Debug( SMLogTag.Service,			SMLogTag.Service );
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

		/// <summary>
		/// ● 廃棄
		/// </summary>
		public override void Dispose() => _disposables.Dispose();

		///------------------------------------------------------------------------------------------------
		/// ● 登録
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● フォーマットを装飾
		/// </summary>
		protected override string DecorationFormat( string format, Color color ) {
			if ( _decorationManager == null ) {
				_decorationManager = SMServiceLocator.Resolve<SMDecorationManager>();
			}
			return _decorationManager._uGUI.ByColor( format, color );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 文章変換
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章変換を設定
		/// </summary>
		public virtual void SetToString() {
			_toStringer.Hide( nameof( s_instance ) );
		}

		/// <summary>
		/// ● 文章に変換
		/// </summary>
		public override string ToString() => ToString( 0 );

		/// <summary>
		/// ● 文章に変換（インデントを整列）
		/// </summary>
		public virtual string ToString( int indent, bool isUseHeadIndent = true )
			=> _toStringer.Run( indent, isUseHeadIndent );

		/// <summary>
		/// ● 1行文章に変換（インデントを整列）
		/// </summary>
		public virtual string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}