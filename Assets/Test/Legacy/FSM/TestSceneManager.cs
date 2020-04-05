//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using System.Collections;
	using UnityEngine;
	using FSM;
	using Singleton;



	// 有限状態機械のテスト



	public class TestSceneManager : MonoBehaviourSingleton<TestSceneManager>,
								IFiniteStateMachineOwner< GeneralStateMachine<TestSceneManager> >
	{

		public GeneralStateMachine<TestSceneManager> _fsm { get; private set; }

		protected override void Constructor() {
			base.Constructor();

			var states = new State< TestSceneManager, GeneralStateMachine<TestSceneManager> >[] {
				new TitleScene( this ),
			};
			_fsm = new GeneralStateMachine<TestSceneManager>( this, states );
			_fsm.Initialize();
		}

		public void ChangeScene() {
		}
	}


	public class TitleScene : State< TestSceneManager, GeneralStateMachine<TestSceneManager> > {

		TestCat _cat;
		TestDog _dog;

		public TitleScene( TestSceneManager owner ) : base( owner ) {
		}

		public override IEnumerator OnEnter() {
			_cat = Object.FindObjectOfType<TestCat>();
			_dog = Object.FindObjectOfType<TestDog>();
//			_cat._fsm.ChangeState< UIFade<TestCat> >();
			_dog._fsm.ChangeState< RunState<TestDog> >();
			_owner.ChangeScene();
			yield break;
		}
	}
}