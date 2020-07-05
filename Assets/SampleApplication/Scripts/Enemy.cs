//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Linq;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using KoganeUnityLib;



/// <summary>
/// ■ 敵のクラス
/// </summary>
public class Enemy : AI {
	/// <summary>
	/// ● コンストラクタ（疑似的）
	///		読込時に、有限状態機械を初期化、銃弾を読み込む。
	/// </summary>
	protected override void Constructor() {
		base.Constructor();
		// ● 読込
		_loadEvent += async () => {
			_fsm = new AIStateMachine( this,
				new AIState[] {
					new WaitEnemyState( this ),
					new BattleEnemyState( this ),
					new AttackEnemyState( this ),
					new DeathAIState( this ),
				}
			);
			_fsm._bullet = (GameObject)await Resources.LoadAsync<GameObject>( "Prefabs/EnemyBullet" );
			_fsm.Initialize();
		};
	}
}



/// <summary>
/// ■ 敵の待機状態のクラス
/// </summary>
public class WaitEnemyState : AIState {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public WaitEnemyState( Enemy owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 被ダメージ
	///		被ダメージ処理、戦闘状態への遷移を実装。
	/// </summary>
	public override void Damage() {
		base.Damage();
		if ( _fsm._isDeath )	{ return; }
		if ( _fsm._state is WaitEnemyState ) {
			_fsm.ChangeState<BattleEnemyState>();
		}
	}
	/// <summary>
	/// ● 更新（微分）
	///		敵が視界の範囲内に存在する場合、戦闘状態へ遷移。
	/// </summary>
	public override void OnUpdateDelta() {
		if ( CheckChangeDeathState() )	{ return; }

		var isBattle = _fsm._enemies
			.Any( ai => {
				var delta = ai.transform.position - _owner.transform.position;
				var deltaAngle = Vector3.Angle( _owner.transform.forward, delta );
				return delta.magnitude < 20 && deltaAngle < 90;
			} );
		if ( isBattle ) {
			_fsm.ChangeState<BattleEnemyState>();
			return;
		}
	}
}



/// <summary>
/// ■ 敵の戦闘状態のクラス
/// </summary>
public class BattleEnemyState : AIState {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public BattleEnemyState( Enemy owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 更新（微分）
	///		一番近い敵に向け回転と接近を行い、目前の場合銃弾攻撃状態へ、遠く離れた場合待機状態へ遷移。
	/// </summary>
	public override void OnUpdateDelta() {
		if ( CheckChangeDeathState() )	{ return; }

		var targetPair = _fsm._enemies.Select( enemy => {
			var delta = enemy.transform.position - _owner.transform.position;
			return new { delta, enemy };
		} )
		.MinBy( a => a.delta.magnitude );

		_owner.transform.rotation = Quaternion.RotateTowards(
			_owner.transform.rotation,
			Quaternion.LookRotation( targetPair.delta ),
			300 * Time.deltaTime
		);
		_fsm._rigidbody.AddForce(
			_owner.transform.forward * 8 * Time.deltaTime,
			ForceMode.VelocityChange
		);

		var distance = targetPair.delta.magnitude;
		var deltaAngle = Vector3.Angle( _owner.transform.forward, targetPair.delta );
		if ( distance < 15 && deltaAngle < 30 ) {
			_fsm.ChangeState<AttackEnemyState>();
			return;
		}
		if ( distance > 30 ) {
			_fsm.ChangeState<WaitEnemyState>();
			return;
		}
	}
}



/// <summary>
/// ■ 敵の攻撃状態
/// </summary>
public class AttackEnemyState : AIState {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public AttackEnemyState( Enemy owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 入口
	///		攻撃処理を行う。
	/// </summary>
	public override IEnumerator OnEnter() {
		Attack();
		yield break;
	}
	/// <summary>
	/// ● 更新
	///		一定時間経過後、戦闘状態へ遷移。
	/// </summary>
	public override IEnumerator OnUpdate() {
		yield return new WaitForSeconds( 0.5f );
		_fsm.ChangeState<BattleEnemyState>();
	}
	/// <summary>
	/// ● 更新（微分）
	///		一番近い敵の方を向く。
	/// </summary>
	public override void OnUpdateDelta() {
		if ( CheckChangeDeathState() )	{ return; }

		var targetPair = _fsm._enemies.Select( enemy => {
			var delta = enemy.transform.position - _owner.transform.position;
			return new { delta, enemy };
		} )
		.MinBy( a => a.delta.magnitude );

		_owner.transform.rotation = Quaternion.RotateTowards(
			_owner.transform.rotation,
			Quaternion.LookRotation( targetPair.delta ),
			300 * Time.deltaTime
		);
	}
}