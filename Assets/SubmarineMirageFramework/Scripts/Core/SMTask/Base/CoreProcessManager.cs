//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Singleton;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 中心処理の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		自動更新が必要なものは、ここに登録する。
	///		こうすると、不要ゲーム物（MonoBehaviourSingleton）を作らずに済む。
	///		
	///		Linq記述方法もあるが、async/awaitの処理順を保障する為、田舎臭い記述方法を採用。
	/// </summary>
	///====================================================================================================
	public class CoreProcessManager : MonoBehaviourSingleton<CoreProcessManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行済処理の状態（各種処理が、何処まで実行されたか）</summary>
		public enum ExecutedState {
			/// <summary>作成、処理済</summary>
			Create,
			/// <summary>読込、処理済</summary>
			Load,
			/// <summary>初期化、処理済</summary>
			Initialize,
			/// <summary>定期更新、処理済</summary>
			FixedUpdate,
			/// <summary>更新、処理済</summary>
			Update,
			/// <summary>遅延更新、処理済</summary>
			LateUpdate,
			/// <summary>終了、処理済</summary>
			Finalize,
		}

		/// <summary>登録するか？</summary>
		public override bool _isRegister => false;
		/// <summary>処理一覧</summary>
		List<IProcess> _processes = new List<IProcess>();
		/// <summary>削除する処理一覧</summary>
		List<IProcess> _deleteProcesses = new List<IProcess>();
		/// <summary>処理を削除中か？</summary>
		bool _isProcessDeleting;

		/// <summary>読込時のイベント</summary>
		public new Func<UniTask> _loadEvent {
			get { return base._loadEvent; }
			set { base._loadEvent = value; }
		}
		/// <summary>初期化時のイベント</summary>
		public new Func<UniTask> _initializeEvent {
			get { return base._initializeEvent; }
			set { base._initializeEvent = value; }
		}
		/// <summary>物理更新時のイベント</summary>
		public new Subject<Unit> _fixedUpdateEvent {
			get { return base._fixedUpdateEvent; }
			set { base._fixedUpdateEvent = value; }
		}
		/// <summary>更新時のイベント</summary>
		public new Subject<Unit> _updateEvent {
			get { return base._updateEvent; }
			set { base._updateEvent = value; }
		}
		/// <summary>遅更新時のイベント</summary>
		public new Subject<Unit> _lateUpdateEvent {
			get { return base._lateUpdateEvent; }
			set { base._lateUpdateEvent = value; }
		}
#if DEVELOP
		/// <summary>GUI描画時のイベント</summary>
		public Subject<Unit> _onGUIEvent = new Subject<Unit>();
#endif
		/// <summary>終了時のイベント</summary>
		public new Func<UniTask> _finalizeEvent {
			get { return base._finalizeEvent; }
			set { base._finalizeEvent = value; }
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 疑似コンストラクタ（Main関数呼戻）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask Constructor( Func<UniTask> initializePlugin, Func<UniTask> registerProcesses ) {
			await initializePlugin();				// 外部プラグインを初期化
			RegisterEvents( registerProcesses );	// イベント関数を登録
			base.Constructor();

			await Load();
			await Initialize();

			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.SMTask );

/*
			_updateEvent
				.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
				.DistinctUntilChanged()
				.Subscribe( _ => CheckDeleteProcessesForScene().Forget() );
*/
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● イベント関数を登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void RegisterEvents( Func<UniTask> registerProcesses ) {
			// 読込処理を登録
			_loadEvent += async () => {
				ChangeExecutedState( this, ExecutedState.Load, true );
				await registerProcesses();	// 処理を初期登録

				// 登録処理を読込
				foreach ( var p in _processes ) {
					if ( ChangeExecutedState( p, ExecutedState.Load ) ) {
						await ( (IProcessLoader)p ).Load();
					}
				}
			};


			// 初期化処理を登録
			_initializeEvent += async () => {
				ChangeExecutedState( this, ExecutedState.Initialize, true );

				// 登録処理を初期化
				foreach ( var p in _processes ) {
					if ( ChangeExecutedState( p, ExecutedState.Initialize ) ) {
						await p.Initialize();
					}
				}
			};


			// 物理更新処理を登録
			_fixedUpdateEvent.Subscribe(
				_ => {
					// 登録処理を物理更新
					foreach ( var p in _processes ) {
						if ( ChangeExecutedState( p, ExecutedState.FixedUpdate ) ) {
							( (IProcessUpdater)p ).FixedUpdate();
						}
					}
				}
			);


			// 更新処理を登録
			_updateEvent.Subscribe(
				_ => {
					// 登録処理を更新
					foreach ( var p in _processes ) {
						if ( ChangeExecutedState( p, ExecutedState.Update ) ) {
							( (IProcessUpdater)p ).Update();
						}
					}
				}
			);


			// 遅延更新処理を登録
			_lateUpdateEvent.Subscribe(
				_ => {
					// 登録処理を遅延更新
					foreach ( var p in _processes ) {
						if ( ChangeExecutedState( p, ExecutedState.LateUpdate ) ) {
							( (IProcessUpdater)p ).LateUpdate();
						}
					}
					// 処理削除を判定
					CheckDeleteProcesses().Forget();
#if DEVELOP
					// 実行中処理をデバッグ表示
					DebugDisplay.s_instance.Add( Color.cyan );
					DebugDisplay.s_instance.Add( $"● {this.GetAboutName()}" );
					DebugDisplay.s_instance.Add( Color.white );
					foreach ( var p in _processes ) {
						DebugDisplay.s_instance.Add( $"\t{p.GetAboutName()}" );
					}
#endif
				}
			);


			// 終了処理を登録
			_finalizeEvent += async () => {
				ChangeExecutedState( this, ExecutedState.Finalize, true );

				// 登録処理を終了
				foreach ( var p in _processes ) {
					Unregister( p );
				}
				await CheckDeleteProcesses();
			};


			// アプリケーション終了処理を登録
			// TODO : asyncで待機出来ず、実行前に終了してる為、FinalizeScene等から呼ぶようにする
			Observable.OnceApplicationQuit().Subscribe(
				async _ => await Finalize()
			);


			// Unity物理更新時イベントを登録
			Observable.EveryFixedUpdate().Subscribe( _ => {
				if ( ChangeExecutedState( this, ExecutedState.FixedUpdate, true ) ) {
					FixedUpdateProcess();
				}
			} );
			// Unity更新時イベントを登録
			Observable.EveryUpdate().Subscribe( _ => {
				if ( ChangeExecutedState( this, ExecutedState.Update, true ) ) {
					UpdateProcess();
				}
			} );
			// Unity遅延更新時イベントを登録
			Observable.EveryLateUpdate().Subscribe( _ => {
				if ( ChangeExecutedState( this, ExecutedState.LateUpdate, true ) ) {
					LateUpdateProcess();
				}
			} );
		}
#if DEVELOP
		/// <summary>
		/// ● GUI描画（Unity呼戻）
		/// </summary>
		void OnGUI() {
			// GUI描画イベントを発行
			if ( _executedState == ExecutedState.LateUpdate ) {
				_onGUIEvent.OnNext( Unit.Default );
			}
		}
#endif
		///------------------------------------------------------------------------------------------------
		/// ● 登録
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public async UniTask Register( IProcess process ) {
			// 未登録の場合
			if ( !_processes.Contains( process ) ) {
				_processes.Add( process );	// 登録

				// 全体処理が既に初期化済の場合、即初期化
				if ( _isInitialized ) {
					// 読込
					if ( ChangeExecutedState( process, ExecutedState.Load ) ) {
						await ( (IProcessLoader)process ).Load();
					}
					// 初期化
					if ( ChangeExecutedState( process, ExecutedState.Initialize ) ) {
						await process.Initialize();
					}
				}
			}
		}
		/// <summary>
		/// ● 登録解除
		/// </summary>
		public void Unregister( IProcess process ) {
			_deleteProcesses.Add( process );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 削除処理
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 削除処理を判定
		/// </summary>
		async UniTask CheckDeleteProcesses() {
			// 削除処理が未登録の場合、未処理
			if ( _deleteProcesses.Count == 0 )	{ return; }
			// 削除中は、未処理
			if ( _isProcessDeleting )			{ return; }
			_isProcessDeleting = true;	// 削除中ロックを掛ける

			
			// 登録処理の重複を除き、コピーし、新規登録に備えリセットする
			var dps = _deleteProcesses.Distinct().ToList();
			_deleteProcesses.Clear();
			// 一気待機用の、仕事配列
			var tasks = new List<UniTask>();

			// 全削除処理を走査
			foreach ( var p in dps ) {
				// 登録削除に成功した場合
				if ( _processes.Remove( p ) ) {
					// 即実行匿名関数を仕事登録
					tasks.Add(
						new Func<UniTask>(
							async () => {
								// 終了可能な場合、終了
								if ( ChangeExecutedState( p, ExecutedState.Finalize ) ) {
									await p.Finalize();
								}
							}
						)()
					);
				}
			}


			// 全終了まで待機
			await UniTask.WhenAll( tasks );
			_isProcessDeleting = false;	// 削除中ロックを解除
		}
		/// <summary>
		/// ● 削除処理を判定（場面遷移）
		///		TODO : シーン遷移の直前に呼び出す
		/// </summary>
		async UniTask CheckDeleteProcessesForScene() {
			foreach ( var p in _processes ) {
				if ( p._isInSceneOnly ) {
					Unregister( p );
				}
			}
			await UniTask.WaitUntil( () => _deleteProcesses.Count == 0 );
			await UniTask.WaitUntil( () => !_isProcessDeleting );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 実行済処理の状態遷移
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool ChangeExecutedState( IProcess process, ExecutedState newState,
											bool isCheckMonoBehaviour = false
		) {
			var isChange = false;		// 状態遷移可能か？
			var isProcessable = false;	// 関数実行可能か？

			// 読込処理か？
			var isLoader = process is IProcessLoader;
			// 更新処理か？
			var isUpdater =
				process is IProcessUpdater ||
				( isCheckMonoBehaviour && process is MonoBehaviour );

			var currentState = process._executedState;	// 現在の状態
			var delta = currentState - newState;		// 新規状態から見た、状態の遷移差


			// 繊維先状態で分岐
			switch ( newState ) {
				// 生成は、如何なる状態からも遷移不可能
				case ExecutedState.Create:
					isChange = false;
					isProcessable = isChange;
					break;

				// 読込は、前から順番に遷移可能
				case ExecutedState.Load:
					isChange =
						isLoader &&
						delta == -1;
					isProcessable = isChange;
					break;

				// 初期化は、前から順番か、読込を飛ばして遷移可能
				case ExecutedState.Initialize:
					isChange = delta == ( isLoader ? -1 : -2 );
					isProcessable = isChange;
					break;

				// 更新は、前から順番に遷移可能で、前の更新状態は好きに実行できる
				case ExecutedState.FixedUpdate:
				case ExecutedState.Update:
				case ExecutedState.LateUpdate:
					isChange =
						isUpdater &&
						process._isInitialized &&
						delta == -1;
					isProcessable =
						isUpdater &&
						process._isInitialized &&
						-1 <= delta &&
						currentState <= ExecutedState.LateUpdate;
					break;

				// 破棄は、同状態以外なら、何処からでも遷移可能
				case ExecutedState.Finalize:
					isChange = delta != 0;
					isProcessable = isChange;
					break;
			}


			// 遷移可能な場合、状態遷移
			if ( isChange )	{ process._executedState = newState; }
			// 関数実行可能か？を返す
			return isProcessable;
		}
	}
}