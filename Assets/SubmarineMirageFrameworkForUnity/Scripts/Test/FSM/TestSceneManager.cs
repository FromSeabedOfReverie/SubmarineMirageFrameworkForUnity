//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test.FSM {
	using System.Collections;
	using UnityEngine;
	using SubmarineMirageFramework.FSM;
	using Singleton;



	// 有限状態機械のテスト



	public class TestSceneManager : MonoBehaviourSingleton<TestSceneManager>,
								IFiniteStateMachineOwner< GeneralFiniteStateMachine<TestSceneManager> >
	{

		public GeneralFiniteStateMachine<TestSceneManager> _fsm { get; private set; }

		protected override void Constructor() {
			base.Constructor();

			var states = new State< TestSceneManager, GeneralFiniteStateMachine<TestSceneManager> >[] {
				new TitleScene( this ),
			};
			_fsm = new GeneralFiniteStateMachine<TestSceneManager>( this, states );
		}

		public void ChangeScene() {
		}
	}


	public class TitleScene : State< TestSceneManager, GeneralFiniteStateMachine<TestSceneManager> > {

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