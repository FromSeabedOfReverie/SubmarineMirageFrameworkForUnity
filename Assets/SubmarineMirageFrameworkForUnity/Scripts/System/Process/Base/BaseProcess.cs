//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process {
	using System;
	using UniRx;
	using UniRx.Async;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 処理の基盤クラス
	///----------------------------------------------------------------------------------------------------
	///		継承して登録したクラスは、自動更新される。
	///		インターフェース関数を、オーバーライドさせるのが面倒なので作成。
	/// </summary>
	///====================================================================================================
	public abstract class BaseProcess : IProcessUpdater, IProcessLoader {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行済処理の状態</summary>
		public CoreProcessManager.ExecutedState _executedState	{ get; set; }
		/// <summary>登録するか？</summary>
		public virtual bool _isRegister => true;
		/// <summary>場面内だけ存在するか？</summary>
		public virtual bool _isInSceneOnly => true;
		/// <summary>中心処理初期化後まで待機するか？</summary>
		public virtual bool _isWaitInitializedCoreProcesses => true;
		/// <summary>初期化済か？</summary>
		public bool _isInitialized	{ get; private set; }
		/// <summary>読込時のイベント</summary>
		public Func<UniTask> _loadEvent			{ get; protected set; } = async () => await UniTask.Delay( 0 );
		/// <summary>初期化時のイベント</summary>
		public Func<UniTask> _initializeEvent	{ get; protected set; } = async () => await UniTask.Delay( 0 );
		/// <summary>物理更新時のイベント</summary>
		public Subject<Unit> _fixedUpdateEvent	{ get; protected set; } = new Subject<Unit>();
		/// <summary>更新時のイベント</summary>
		public Subject<Unit> _updateEvent		{ get; protected set; } = new Subject<Unit>();
		/// <summary>遅更新時のイベント</summary>
		public Subject<Unit> _lateUpdateEvent	{ get; protected set; } = new Subject<Unit>();
		/// <summary>終了時のイベント</summary>
		public Func<UniTask> _finalizeEvent		{ get; protected set; } = async () => await UniTask.Delay( 0 );
		///------------------------------------------------------------------------------------------------
		/// ● コンストラクタ
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		protected BaseProcess() {
			BaseProcessSub().Forget();
		}
		/// <summary>
		/// ● コンストラクタ（補助）
		/// </summary>
		async UniTask BaseProcessSub() {
			if ( !_isRegister )	{ return; }	// 未登録の場合、未処理

			// 継承先のコンストラクタ終了後に、登録したいので、少し待機
			await UniTask.Delay( 1 );

			// 中心処理初期化後まで待機の場合、待機
			if ( _isWaitInitializedCoreProcesses ) {
				await UniTask.WaitUntil( () => CoreProcessManager.s_instance._isInitialized );
			}

			await CoreProcessManager.s_instance.Register( this );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		///		サーバー情報やアセット等を読みたい時、予めここで要求する。
		///		（読込要求しっぱなしで、完了待機しないで良い。）
		///		初期化の前に呼ばれる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual async UniTask Load() {
			await _loadEvent();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化
		///		自身の要求読込が完了後、場面全体の読込後に、呼ばれる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual async UniTask Initialize() {
			await _initializeEvent();
			_isInitialized = true;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新（物理）
		///		物理エンジン関連処理に使用する。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void FixedUpdate() {
			_fixedUpdateEvent.OnNext( Unit.Default );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void Update() {
			_updateEvent.OnNext( Unit.Default );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新（遅）
		///		更新タイミングを遅らせたい場合に使用する。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual void LateUpdate() {
			_lateUpdateEvent.OnNext( Unit.Default );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		///		Finalize関数は、デストラクタで定義されているらしいので、別名を定義。
		///		破棄直前に、デストラクタより早く呼ばれる。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public virtual async UniTask Finalize() {
			await _finalizeEvent();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ToString() {
			return this.ToDeepString();
		}
	}
}