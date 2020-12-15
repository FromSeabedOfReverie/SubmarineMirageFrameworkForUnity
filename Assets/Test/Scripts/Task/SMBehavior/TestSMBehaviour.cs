//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
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



	public class TestSMBehaviour : SMStandardTest {
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
		public IEnumerator TestCreate() => From( () => _testBody.TestCreate<B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunErrorState() => From( () => _testBody.TestRunErrorState<B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestTaskType() => From( () => _testBody.TestTaskType<B4, B5, B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestStopActiveAsync() => From( () => _testBody.TestStopActiveAsync<B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( () => _testBody.TestDispose<B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDelete() => From( () => _testBody.TestDelete<B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( _testBody.TestManual<B6>() );
	}
}