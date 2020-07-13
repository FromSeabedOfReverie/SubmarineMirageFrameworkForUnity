//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SubmarineMirage.UTask;
using SubmarineMirage.Process;
using SubmarineMirage.FSM;
using SubmarineMirage.Audio;



/// <summary>
/// ■ 人工知能のクラス
/// </summary>
public abstract class AI : MonoBehaviourProcess, IFiniteStateMachineOwner<AIStateMachine> {
	/// <summary>人工知能の有限状態機械</summary>
	public AIStateMachine _fsm	{ get; protected set; }
	/// <summary>
	/// ● コンストラクタ（疑似的）
	///		読込時に場面管理クラス登録と音読込、初期化時に登録された敵を全取得する。
	/// </summary>
	protected override void Constructor() {
		// ● 読込
		_loadEvent += async () => {
			SceneManager.s_instance._fsm._ais.Add( this );
			await GameAudioManager.s_instance._voice.Load( GameAudioManager.Voice.TestRidicule );
			await GameAudioManager.s_instance._voice.Load( GameAudioManager.Voice.TestScream );
			await UTask.DontWait();
		};
		// ● 初期化
		_initializeEvent += async () => {
			_fsm._enemies = SceneManager.s_instance._fsm._ais
				.Where( ai => ai.GetType() != GetType() )
				.ToList();
			await UTask.DontWait();
		};
	}
}



/// <summary>
/// ■ 人工知能の有限状態機械のクラス
/// </summary>
public class AIStateMachine : FiniteStateMachine<AI, AIStateMachine> {
	/// <summary>敵一覧</summary>
	public List<AI> _enemies = new List<AI>();
	/// <summary>剛体</summary>
	public Rigidbody _rigidbody;
	/// <summary>銃弾のゲーム物</summary>
	public GameObject _bullet;
	/// <summary>被ダメージ量</summary>
	public float _damage;
	/// <summary>死亡中か？</summary>
	public bool _isDeath	=> _state is DeathAIState;
	/// <summary>戦闘中か？</summary>
	public bool _isBattle	=> _state is BattleEnemyState || _state is AttackEnemyState;
	/// <summary>
	/// ● コンストラクタ
	///		剛体を取得する。
	/// </summary>
	public AIStateMachine( AI owner, AIState[] states )
		: base( owner, states )
	{
		_rigidbody = _owner.GetComponent<Rigidbody>();
	}
}



/// <summary>
/// ■ 人工知能の状態クラス
/// </summary>
public abstract class AIState : State<AI, AIStateMachine> {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public AIState( AI owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 死亡状態へ遷移するか？
	///		被ダメージを沢山受けた、奈落へ落下した場合、死亡状態へ。
	/// </summary>
	protected bool CheckChangeDeathState() {
		if ( _fsm._isDeath )	{ return false; }
		if ( _fsm._damage > 100 || _owner.transform.position.y < -5 ) {
			_fsm.ChangeState<DeathAIState>();
			return true;
		}
		return false;
	}
	/// <summary>
	/// ● 攻撃
	///		前方に銃弾の生成を行う。
	/// </summary>
	protected void Attack() {
		var bullet = Object.Instantiate(
			_fsm._bullet,
			_owner.transform.position + _owner.transform.forward * 1.5f,
			_owner.transform.rotation
		);
	}
	/// <summary>
	/// ● 被ダメージ
	///		敵からの攻撃時、一定ダメージを受け、死亡判定し、ダメージ音を再生する。
	/// </summary>
	public virtual void Damage() {
		if ( _fsm._isDeath )	{ return; }
		_fsm._damage += 10;
		CheckChangeDeathState();
		GameAudioManager.s_instance._voice.Play( GameAudioManager.Voice.TestRidicule );
	}
}



/// <summary>
/// ■ 人工知能の死亡状態クラス
/// </summary>
public class DeathAIState : AIState {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public DeathAIState( AI owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 入口
	///		声音の再生、剛体を自由回転させる。
	/// </summary>
	public override IEnumerator OnEnter() {
		GameAudioManager.s_instance._voice.Play( GameAudioManager.Voice.TestScream );
		_fsm._rigidbody.constraints = RigidbodyConstraints.None;
		yield break;
	}
}