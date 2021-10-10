//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	///====================================================================================================
	/// <summary>
	/// ■ フレームワーク標準のモノ動作クラス
	///		フレームワークの初期化待機の調整を行う。
	/// </summary>
	///====================================================================================================
	public abstract class SMStandardMonoBehaviour : MonoBehaviourSMExtension, ISMStandardBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>破棄の一覧</summary>
		public SMDisposable _disposables { get; private set; } = new SMDisposable();
		/// <summary>破棄済か？</summary>
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		/// <summary>デバッグ文字列の生成者</summary>
		public SMToStringer _toStringer { get; private set; }

		/// <summary>フレームワーク</summary>
		protected SubmarineMirageFramework _framework { get; private set; }
		/// <summary>フレームワークが初期化済か？</summary>
		protected bool _isFrameworkInitialized	{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 早期初期化（Unityコールバック）
		/// </summary>
		protected override void Awake() {
			base.Awake();

			_toStringer = new SMToStringer( this );
			SetToString();

			UTask.Void( async () => {
				_framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
				await _framework.WaitInitialize();
				if ( isActiveAndEnabled )	{ StartAfterInitialize(); }
				_isFrameworkInitialized = true;
			} );

			_disposables.AddFirst( () => {
				_toStringer.Dispose();
				gameObject.Destroy();
				_framework = null;
			} );
		}

		/// <summary>
		/// ● 廃棄
		/// </summary>
		public override void Dispose() => _disposables.Dispose();

		///------------------------------------------------------------------------------------------------
		/// ● 更新
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 物理更新（Unityコールバック）
		/// </summary>
		protected virtual void FixedUpdate() {
			if ( _isFrameworkInitialized )	{ FixedUpdateAfterInitialize(); }
		}

		/// <summary>
		/// ● 更新（Unityコールバック）
		/// </summary>
		protected virtual void Update() {
			if ( _isFrameworkInitialized )	{ UpdateAfterInitialize(); }
		}

		/// <summary>
		/// ● 後更新（Unityコールバック）
		/// </summary>
		protected virtual void LateUpdate() {
			if ( _isFrameworkInitialized )	{ LateUpdateAfterInitialize(); }
		}

		///------------------------------------------------------------------------------------------------
		/// ● 拡張Unityコールバック
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化
		///		フレームワーク初期化後の、Unityコールバック。
		/// </summary>
		protected virtual void StartAfterInitialize() {
		}

		/// <summary>
		/// ● 物理更新
		///		フレームワーク初期化後の、Unityコールバック。
		/// </summary>
		protected virtual void FixedUpdateAfterInitialize() {
		}

		/// <summary>
		/// ● 更新
		///		フレームワーク初期化後の、Unityコールバック。
		/// </summary>
		protected virtual void UpdateAfterInitialize() {
		}

		/// <summary>
		/// ● 後更新
		///		フレームワーク初期化後の、Unityコールバック。
		/// </summary>
		protected virtual void LateUpdateAfterInitialize() {
		}

		///------------------------------------------------------------------------------------------------
		/// ● エラー
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 廃棄済エラーを判定
		/// </summary>
		protected void CheckDisposeError( string name ) {
			if ( !_isDispose ) { return; }

			throw new ObjectDisposedException(
				$"{this}", $"既に解放済\n{this.GetAboutName()}.{name}" );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 文字列に変換
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列を設定
		/// </summary>
		public virtual void SetToString() {
		}

		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		public override string ToString( int indent, bool isUseHeadIndent = true )
			=> _toStringer.Run( indent, isUseHeadIndent );

		/// <summary>
		/// ● 1行文字列に変換
		/// </summary>
		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );
	}
}