//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestLog
namespace SubmarineMirage {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ デバッグ記録のクラス
	///		UnityエディタのConsole窓から、クリックで飛べない為、DLL化した基盤クラスを継承する。
	/// </summary>
	///====================================================================================================
	public class SMLog : BaseSMLog<SMLogTag>, ISMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShowLine] public uint _id	{ get; private set; }
		public SMDisposable _disposables	{ get; private set; } = new SMDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		public SMToStringer _toStringer	{ get; private set; }
		SMDecorationManager _decorationManager	{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMLog() : base( SMDebugManager.IS_UNITY_EDITOR ) {
			_id = SMIDCounter.GetNewID( this );
			_toStringer = new SMToStringer( this );
			SetToString();

			s_isEnable = SMDebugManager.IS_DEVELOP;

			_disposables.AddFirst( () => {
				_toStringer.Dispose();
				base.Dispose();
			} );

#if !TestLog
			return;
#endif
			SMLog.Debug( SMLogTag.Task,				SMLogTag.Task );
			SMLog.Debug( SMLogTag.Service,			SMLogTag.Service );
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
		}

		/// <summary>
		/// ● 廃棄
		/// </summary>
		public override void Dispose()
			=> _disposables.Dispose();

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
			_toStringer.Add( nameof( s_isEnable ), i => _toStringer.DefaultValue( s_isEnable, i, false ) );
			_toStringer.Add( nameof( _isEditor ), i => _toStringer.DefaultValue( _isEditor, i, false ) );
			_toStringer.Add( nameof( _tagFormats ), i => _toStringer.DefaultValue( _tagFormats, i, false ) );
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