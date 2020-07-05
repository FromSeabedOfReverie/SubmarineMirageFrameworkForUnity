//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Collections;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Extension;



/// <summary>
/// ■ プレイヤーのクラス
/// </summary>
public class Player : AI {
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
					new ActionPlayerState( this ),
					new DeathAIState( this ),
				}
			);
			_fsm._bullet = (GameObject)await Resources.LoadAsync<GameObject>( "Prefabs/PlayerBullet" );
			_fsm.Initialize();
		};
	}
}



/// <summary>
/// ■ プレイヤーの行動状態クラス
/// </summary>
public class ActionPlayerState : AIState {
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public ActionPlayerState( Player owner ) : base( owner ) {
	}
	/// <summary>
	/// ● 入口
	///		キー入力時、銃弾攻撃を設定。
	/// </summary>
	public override IEnumerator OnEnter() {
		InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide )
			.TakeWhile( _ => _fsm._state == this )
			.Subscribe( _ => Attack() );
		yield break;
	}
	/// <summary>
	/// ● 更新（微分）
	///		プレイヤーの回転、移動を実装。
	/// </summary>
	public override void OnUpdateDelta() {
		if ( CheckChangeDeathState() )	{ return; }

		var rotate = InputManager.s_instance.GetAxis( InputManager.Axis.Rotate );
		var rotateX = rotate.x * 200 * Time.deltaTime;
		_owner.transform.rotation *= Quaternion.Euler( 0, rotateX, 0 );

		var move = InputManager.s_instance.GetAxis( InputManager.Axis.Move );
		var velocity = _owner.transform.rotation
			* new Vector3( move.x, 0, move.y ) * 10 * Time.deltaTime;
		_fsm._rigidbody.AddForce( velocity, ForceMode.VelocityChange );
	}
}