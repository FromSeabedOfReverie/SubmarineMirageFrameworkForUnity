//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMMonoBehaviour : Test {
		TestSMBehaviourBody _testBody;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_testBody = new TestSMBehaviourBody();
			_disposables.AddLast( _testBody );

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ =>
				_text.text = _testBody._viewText
			) );
			_disposables.AddLast( () => _text.text = string.Empty );
		}


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreateMono() => From( () => _testBody.TestCreateMono<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunErrorState() => From( () => _testBody.TestRunErrorState<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestTaskType() => From( () => _testBody.TestTaskType<M4, M5, M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestStopActiveAsync() => From( () => _testBody.TestStopActiveAsync<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( () => _testBody.TestDispose<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDelete() => From( () => _testBody.TestDelete<M6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManualAtActive() => From( _testBody.TestManual<M6>( true ) );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManualAtInActive() => From( _testBody.TestManual<M6>( false ) );
	}
}