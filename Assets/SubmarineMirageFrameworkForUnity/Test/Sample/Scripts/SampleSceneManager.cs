//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Sample {
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UniRx;
	using Singleton;
	using Test.Audio;
	using FSM;



	/// <summary>
	/// ■ サンプル場面の管理クラス
	/// </summary>
	public class SampleSceneManager : Singleton<SampleSceneManager>, IFiniteStateMachineOwner<GameManager> {
		/// <summary>ゲーム進行管理の有限状態機械</summary>
		public GameManager _fsm	{ get; private set; }
		/// <summary>
		/// ● コンストラクタ
		///		有限状態機械を初期化し、読込負荷削減の為に音書類の事前読込を行う。
		/// </summary>
		public SampleSceneManager() {
			_fsm = new GameManager( this );
			_fsm.Initialize();
			// ● 初期化
			_initializeEvent += async () => {
				await TestAudioManager.s_instance._bgm.Load( TestAudioManager.BGM.TestBattle );
				await TestAudioManager.s_instance._bgs.Load( TestAudioManager.BGS.TestWind );
				await TestAudioManager.s_instance._jingle.Load( TestAudioManager.Jingle.TestGameOver );
				await TestAudioManager.s_instance._jingle.Load( TestAudioManager.Jingle.TestGameClear );
			};
		}
	}



	/// <summary>
	/// ■ ゲーム進行管理の有限状態機械のクラス
	/// </summary>
	public class GameManager : FiniteStateMachine<SampleSceneManager, GameManager> {
		/// <summary>人工知能の一覧</summary>
		public List<AI> _ais = new List<AI>();
		/// <summary>
		/// ● コンストラクタ
		///		有限状態機械を初期化。
		/// </summary>
		public GameManager( SampleSceneManager owner ) : base(
			owner,
			new GameState[] {
				new NormalGameState( owner ),
				new BattleGameState( owner ),
				new OverGameState( owner ),
				new ClearGameState( owner ),
			}
		) {
		}
	}



	/// <summary>
	/// ■ ゲーム進行の状態クラス
	/// </summary>
	public abstract class GameState : State<SampleSceneManager, GameManager> {
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public GameState( SampleSceneManager owner ) : base( owner ) {
		}
		/// <summary>
		/// ● ゲーム失敗状態へ遷移するか？
		///		全プレイヤーが死亡の場合、ゲーム失敗状態へ。
		/// </summary>
		/// <returns></returns>
		protected bool CheckChangeOverState() {
			if ( _fsm._state is OverGameState )	{ return false; }
			var isOver = _fsm._ais
				.Where( ai => ai is Player )
				.All( p => p._fsm._isDeath );
			if ( isOver ) {
				_fsm.ChangeState<OverGameState>();
			}
			return isOver;
		}
		/// <summary>
		/// ● ゲーム成功状態へ遷移するか？
		///		全敵が死亡の場合、ゲーム成功状態へ。
		/// </summary>
		/// <returns></returns>
		protected bool CheckChangeClearState() {
			if ( _fsm._state is ClearGameState )	{ return false; }
			var isClear = _fsm._ais
				.Where( ai => ai is Enemy )
				.All( e => e._fsm._isDeath );
			if ( isClear ) {
				_fsm.ChangeState<ClearGameState>();
			}
			return isClear;
		}
	}



	/// <summary>
	/// ■ ゲーム進行の通常状態のクラス
	/// </summary>
	public class NormalGameState : GameState {
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public NormalGameState( SampleSceneManager owner ) : base( owner ) {
		}
		/// <summary>
		/// ● 入口
		///		フィールド環境音を再生。
		/// </summary>
		public override IEnumerator OnEnter() {
			TestAudioManager.s_instance._bgs.Play( TestAudioManager.BGS.TestWind );
			yield break;
		}
		/// <summary>
		/// ● 更新（微分）
		///		ゲーム失敗、ゲーム成功、戦闘状態への遷移を判定。
		/// </summary>
		public override void OnUpdateDelta() {
			if ( CheckChangeOverState() )	{ return; }
			if ( CheckChangeClearState() )	{ return; }
			if ( _fsm._ais.Any( ai => ai._fsm._isBattle ) ) {
				_fsm.ChangeState<BattleGameState>();
			}
		}
		/// <summary>
		/// ● 出口
		///		フィールド環境音を停止。
		/// </summary>
		/// <returns></returns>
		public override IEnumerator OnExit() {
			TestAudioManager.s_instance._bgs.Stop();
			yield break;
		}
	}



	/// <summary>
	/// ■ ゲーム進行の戦闘状態のクラス
	/// </summary>
	public class BattleGameState : GameState {
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public BattleGameState( SampleSceneManager owner ) : base( owner ) {
		}
		/// <summary>
		/// ● 入口
		///		背景音楽を再生。
		/// </summary>
		public override IEnumerator OnEnter() {
			TestAudioManager.s_instance._bgm.Play( TestAudioManager.BGM.TestBattle );
			yield break;
		}
		/// <summary>
		/// ● 更新（微分）
		///		ゲーム失敗、ゲーム成功、通常状態への遷移を判定。
		/// </summary>
		public override void OnUpdateDelta() {
			if ( CheckChangeOverState() )	{ return; }
			if ( CheckChangeClearState() )	{ return; }
			if ( _fsm._ais.All( ai => !ai._fsm._isBattle ) ) {
				_fsm.ChangeState<NormalGameState>();
			}
		}
		/// <summary>
		/// ● 出口
		///		背景音楽を停止。
		/// </summary>
		public override IEnumerator OnExit() {
			TestAudioManager.s_instance._bgm.Stop();
			yield break;
		}
	}



	/// <summary>
	/// ■ ゲーム進行の失敗状態のクラス
	/// </summary>
	public class OverGameState : GameState {
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public OverGameState( SampleSceneManager owner ) : base( owner ) {
		}
		/// <summary>
		/// ● 入口
		///		ジングル音を再生。
		/// </summary>
		public override IEnumerator OnEnter() {
			TestAudioManager.s_instance._jingle.Play( TestAudioManager.Jingle.TestGameOver );
			yield break;
		}
	}



	/// <summary>
	/// ■ ゲーム進行の成功状態のクラス
	/// </summary>
	public class ClearGameState : GameState {
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public ClearGameState( SampleSceneManager owner ) : base( owner ) {
		}
		/// <summary>
		/// ● 入口
		///		ジングル音を再生。
		/// </summary>
		/// <returns></returns>
		public override IEnumerator OnEnter() {
			TestAudioManager.s_instance._jingle.Play( TestAudioManager.Jingle.TestGameClear );
			yield break;
		}
	}
}