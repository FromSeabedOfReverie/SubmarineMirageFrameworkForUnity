//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Collections;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;
using SubmarineMirage.Process;
using SubmarineMirage.FSM;
using SubmarineMirage.Extension;
using SubmarineMirage.Audio;



/// <summary>
/// ■ 銃弾のクラス
/// </summary>
public class Bullet : MonoBehaviourProcess, IFiniteStateMachineOwner<BulletStateMachine> {
	/// <summary>銃弾の有限状態機械</summary>
	public BulletStateMachine _fsm	{ get; private set; }
	/// <summary>
	/// ● コンストラクタ（疑似的）
	///		読込時に音読込と有限状態機械を初期化し、終了時にゲーム物を破棄する。
	/// </summary>
	protected override void Constructor() {
		// ● 読込
		_loadEvent += async () => {
			await GameAudioManager.s_instance._se.Load( GameAudioManager.SE.TestGun );
			_fsm = new BulletStateMachine( this );
			_fsm.Initialize();
		};
		// ● 終了
		_finalizeEvent += async () => {
			await UniTask.Delay( 0 );
			Destroy( gameObject );
		};
	}
}



/// <summary>
/// ■ 銃弾の有限状態機械のクラス
/// </summary>
public class BulletStateMachine : FiniteStateMachine<Bullet, BulletStateMachine> {
	/// <summary>剛体</summary>
	public Rigidbody _rigidbody;
	/// <summary>
	/// ● コンストラクタ
	///		各種状態の設定、剛体の取得を行う。
	/// </summary>
	public BulletStateMachine( Bullet owner ) : base(
		owner,
		new BulletState[] {
			new AttackBulletState( owner ),
			new DeathBulletState( owner ),
		}
	) {
		_rigidbody = _owner.GetComponent<Rigidbody>();
	}
}



/// <summary>
/// ■ 銃弾の状態クラス
/// </summary>
public abstract class BulletState : State<Bullet, BulletStateMachine> {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public BulletState( Bullet owner ) : base( owner ) {
	}
}



/// <summary>
/// ■ 銃弾の攻撃状態クラス
/// </summary>
public class AttackBulletState : BulletState {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public AttackBulletState( Bullet owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 入口
	///		攻撃音を再生し、剛体に初速を与え、衝突時のダメージ呼び出し、衝突時の死亡処理を実装。
	/// </summary>
	public override IEnumerator OnEnter() {
		GameAudioManager.s_instance._se.Play( GameAudioManager.SE.TestGun );
		_fsm._rigidbody.AddForce( _owner.transform.forward * 20, ForceMode.Impulse );

		_owner.OnCollisionEnterAsObservable()
			.TakeWhile( _ => _fsm._state == this )
			.Select( collision => collision.gameObject )
			.Where( go => !LayerManager.s_instance.IsEqual( go.layer, LayerManager.Name.Ground ) )
			.Subscribe( go => {
				var ai = go.GetComponent<AI>();
				if ( ai != null ) {
					( (AIState)ai._fsm._state ).Damage();
				}
				_fsm.ChangeState<DeathBulletState>();
			} );
		yield break;
	}
	/// <summary>
	/// ● 更新
	///		銃弾が5秒後に死亡する。
	/// </summary>
	public override IEnumerator OnUpdate() {
		yield return new WaitForSeconds( 5 );
		_fsm.ChangeState<DeathBulletState>();
	}
}



/// <summary>
/// ■ 銃弾の死亡状態クラス
/// </summary>
public class DeathBulletState : BulletState {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public DeathBulletState( Bullet owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 入口
	///		Process管理クラスの登録を解除し、終了処理を呼び出して貰う。
	/// </summary>
	public override IEnumerator OnEnter() {
		CoreProcessManager.s_instance.UnRegister( _owner );
		yield break;
	}
}