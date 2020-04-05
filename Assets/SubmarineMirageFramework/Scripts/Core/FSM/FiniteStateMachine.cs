//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;
	using Debug;
	using Extension;
	using Process;
	///====================================================================================================
	/// <summary>
	/// ■ 有限状態機械のクラス
	///----------------------------------------------------------------------------------------------------
	///		State要素に渡す際、ジェネリッククラスだと安定しない為、基盤インタフェースを継承。
	///		TODO : async/awaitに対応する
	/// </summary>
	///====================================================================================================
	public class FiniteStateMachine<TOwner, TFSM> : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
		where TFSM : IFiniteStateMachine
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行中状態の型</summary>
		enum RunState {
			/// <summary>遷移開始</summary>
			OnEnter,
			/// <summary>更新</summary>
			OnUpdate,
			/// <summary>遷移終了</summary>
			OnExit,
		}

		/// <summary>運用者</summary>
		protected TOwner _owner	{ get; private set; }
		/// <summary>状態一覧</summary>
		readonly Dictionary< Type, State<TOwner, TFSM> > _states = new Dictionary< Type, State<TOwner, TFSM> >();
		/// <summary>現在の状態</summary>
		public State<TOwner, TFSM> _state	{ get; private set; }
		/// <summary>次の遷移先状態</summary>
		protected State<TOwner, TFSM> _nextState;
		/// <summary>実行中状態</summary>
		RunState _runState;

		/// <summary>状態遷移中の処理</summary>
		IDisposable _changeStateProcess;
		/// <summary>更新中の処理</summary>
		IDisposable _stateUpdateProcess;
		/// <summary>微分更新中の処理</summary>
		IDisposable _stateUpdateDeltaProcess;
#if DEVELOP
		/// <summary>デバッグ用更新処理</summary>
		IDisposable _debugUpdateProcess;
#endif
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public FiniteStateMachine( TOwner owner, State<TOwner, TFSM>[] states ) {
			_owner = owner;

			foreach ( var state in states ) {
				var type = state.GetType();
				_states[type] = state;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 初期化
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Initialize() {
			_states.ForEach( pair => pair.Value.Initialize() );
			ChangeState( _states.First().Key );

#if DEVELOP && false
			// デバッグ設定
			_debugUpdateProcess = CoreProcessManager.s_instance._updateEvent
				.TakeWhile( _ => this != null )
				.Subscribe( _ => {
				DebugDisplay.s_instance.Add(
					$"{_owner.GetAboutName()}.{this.GetAboutName()} : " +
					$"{_state.GetAboutName()}.{_runState}"
				);
			} );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// ● 状態遷移
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 状態遷移（ジェネリック）
		/// </summary>
		public void ChangeState<TState>() {
			ChangeState( typeof( TState ) );
		}
		/// <summary>
		/// ● 状態遷移（型）
		/// </summary>
		public void ChangeState( Type stateType ) {
			if ( !_states.ContainsKey( stateType ) ) {
				Log.Error( $"未定義状態遷移 : {stateType}" );
			}
			var changeState = _states[stateType];
			if ( !IsPossibleChangeState( changeState ) )	{ return; }
			_nextState = changeState;

			if ( _changeStateProcess != null )	{ return; }
			_changeStateProcess = Observable
				.FromCoroutine( () => ChangeStateSub() )
				.TakeWhile( _ => this != null )
				.Subscribe();
		}
		/// <summary>
		/// ● 状態遷移（補助）
		///		連打すると、途中の遷移先状態が無かった事になる。
		/// </summary>
		IEnumerator ChangeStateSub() {
			_stateUpdateProcess?.Dispose();
			_stateUpdateDeltaProcess?.Dispose();

			// 出口中は、次の状態を変更できる
			_runState = RunState.OnExit;
			// _state?.だと、初期化時に1フレーム遅延設定されるので駄目
			if ( _state != null )	{ yield return _state.OnExit(); }

			_state = _nextState;
			_nextState = null;

			// 入口中に、次の状態に変更された場合、入口後に遷移する
			_runState = RunState.OnEnter;
			yield return _state.OnEnter();

			// 次の状態に変更されていない場合、更新処理
			if ( _nextState == null ) {
				_runState = RunState.OnUpdate;
				_stateUpdateProcess = Observable
					.FromCoroutine( () => _state.OnUpdate() )
					.TakeWhile( _ => this != null )
					.Subscribe();
				_stateUpdateDeltaProcess = CoreProcessManager.s_instance._updateEvent
					.Subscribe( _ => _state.OnUpdateDelta() );
				_changeStateProcess.Dispose();
				_changeStateProcess = null;

			// 次の状態に変更された場合、再度状態遷移
			} else {
				_changeStateProcess = Observable
					.FromCoroutine( () => ChangeStateSub() )
					.TakeWhile( _ => this != null )
					.Subscribe();
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 状態遷移出来るか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected virtual bool IsPossibleChangeState( State<TOwner, TFSM> changeState ) {
			// _state, _nextState等も使って継承先で判定する
			// 引数が多くなり過ぎるので、あえてメンバで取れる物は入れていない
			return true;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		~FiniteStateMachine() {
			// コルーチン全停止
			_changeStateProcess?.Dispose();
			_stateUpdateProcess?.Dispose();
			_stateUpdateDeltaProcess?.Dispose();
#if DEVELOP
			_debugUpdateProcess?.Dispose();
#endif
		}
	}
}