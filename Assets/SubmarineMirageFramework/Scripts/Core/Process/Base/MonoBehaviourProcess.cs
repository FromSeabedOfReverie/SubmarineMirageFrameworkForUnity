//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process {
	using System;
	using UniRx;
	using UniRx.Async;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ モノ動作処理の基盤クラス
	///----------------------------------------------------------------------------------------------------
	///		継承して登録したクラスは、自動読込される。
	///		インターフェース関数を、オーバーライドさせるのが面倒なので作成。
	/// </summary>
	///====================================================================================================
	public abstract class MonoBehaviourProcess : MonoBehaviourExtension, IProcessLoader {
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
		protected Subject<Unit> _fixedUpdateEvent	= new Subject<Unit>();
		/// <summary>更新時のイベント</summary>
		protected Subject<Unit> _updateEvent		= new Subject<Unit>();
		/// <summary>遅更新時のイベント</summary>
		protected Subject<Unit> _lateUpdateEvent	= new Subject<Unit>();
		/// <summary>終了時のイベント</summary>
		public Func<UniTask> _finalizeEvent		{ get; protected set; } = async () => await UniTask.Delay( 0 );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（疑似的）
		///		オブジェクト作成時に、活動中の場合のみ呼ばれる。
		///		各種イベント関数の登録等を、記述する。
		///		※Load()、Initailize()前に登録したい、一瞬で終わる処理のみ記述する。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected virtual void Constructor() {
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
		protected virtual void FixedUpdateProcess() {
			_fixedUpdateEvent.OnNext( Unit.Default );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected virtual void UpdateProcess() {
			_updateEvent.OnNext( Unit.Default );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 更新（遅）
		///		更新タイミングを遅らせたい場合に使用する。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected virtual void LateUpdateProcess() {
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
		/// ● Unity呼戻関数
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化（Unity呼戻）
		///		非活動中でも、初期化時に1番目に呼ばれる。
		///		継承先では使用せず、Load()を使用し、自身の情報を設定する。
		/// </summary>
		protected async void Awake() {	// async UniTaskだと、エラーが出なくなる
			if ( !isActiveAndEnabled )	{ return; }	// ゲーム物、部品が非活動中の場合、未処理
			if ( !_isRegister )			{ return; }	// 未登録の場合、未処理


			// 中心処理初期化後まで待機の場合、待機
			if ( _isWaitInitializedCoreProcesses ) {
				await UniTask.WaitUntil( () => CoreProcessManager.s_instance._isInitialized );
			}

			Constructor();	// 疑似コンストラクタ実行

			// 自身を登録
			await CoreProcessManager.s_instance.Register( this );
			// 物理更新時イベントを登録
			CoreProcessManager.s_instance._fixedUpdateEvent.Subscribe( _ => {
				if ( CoreProcessManager.s_instance.ChangeExecutedState(
						this, CoreProcessManager.ExecutedState.FixedUpdate, true )
				) {
					FixedUpdateProcess();
				}
			} );
			// 更新時イベントを登録
			CoreProcessManager.s_instance._updateEvent.Subscribe( _ => {
				if ( CoreProcessManager.s_instance.ChangeExecutedState(
						this, CoreProcessManager.ExecutedState.Update, true )
				) {
					UpdateProcess();
				}
			} );
			// 遅延更新時イベントを登録
			CoreProcessManager.s_instance._lateUpdateEvent.Subscribe( _ => {
				if ( CoreProcessManager.s_instance.ChangeExecutedState(
						this, CoreProcessManager.ExecutedState.LateUpdate, true )
				) {
					LateUpdateProcess();
				}
			} );
		}

// 継承先で、間違えて記述しないように、基盤クラスで定義
// こうすると継承先で、未継承の警告が出る
#if DEVELOP
		/// <summary>
		/// ● 初期化（Unity呼戻）
		///		活動中のみ、初期化時に2番目に呼ばれる。
		///		継承先では使用せず、Initialize()を使用する。
		/// </summary>
		protected void Start() {}
		/// <summary>
		/// ● 物理更新（Unity呼戻）
		///		物理エンジン更新時に、呼ばれる。
		///		継承先では使用せず、FixedUpdateProcess()を使用する。
		/// </summary>
		protected void FixedUpdate() {}
		/// <summary>
		/// ● 更新（Unity呼戻）
		///		継承先では使用せず、UpdateProcess()を使用する。
		/// </summary>
		protected void Update() {}
		/// <summary>
		/// ● 遅延更新（Unity呼戻）
		///		Update()後に呼ばれる。
		///		継承先では使用せず、LateUpdateProcess()を使用する。
		/// </summary>
		protected void LateUpdate() {}
#endif
	}
}