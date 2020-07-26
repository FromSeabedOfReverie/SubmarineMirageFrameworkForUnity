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
	using Extension;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMBehaviour : Test {
		TestSMBehaviourBody _testBody;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			var behaviour = new B6();
			_testBody = new TestSMBehaviourBody( behaviour );

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( behaviour == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text = string.Join( "\n",
					$"{behaviour.GetAboutName()}(",
					$"    {nameof( behaviour._id )} : {behaviour._id}",
					$"    {nameof( behaviour._type )} : {behaviour._type}",
					$"    {nameof( behaviour._object._owner )} : "
						+ $"{behaviour._object._owner}( {behaviour._object._id} )",
					$"    {nameof( behaviour._body._ranState )} : {behaviour._body._ranState}",
					$"    {nameof( behaviour._body._activeState )} : {behaviour._body._activeState}",
					$"    {nameof( behaviour._body._nextActiveState )} : {behaviour._body._nextActiveState}",
					$"    {nameof( behaviour._isInitialized )} : {behaviour._isInitialized}",
					$"    {nameof( behaviour._isActive )} : {behaviour._isActive}",
					")"
				);
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );
			_disposables.AddLast( behaviour );
		}


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestCreate() => From( () => _testBody.TestCreate() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRunErrorState() => From( () => _testBody.TestRunErrorState() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestEventRun() => From( () => _testBody.TestEventRun( _testBody._behaviour ) );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestTaskType() => From( () => _testBody.TestTaskType<B4, B5, B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestStopActiveAsync() => From( () => _testBody.TestStopActiveAsync() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDispose() => From( () => _testBody.TestDispose<B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestDelete() => From( () => _testBody.TestDelete<B6>() );


		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( _testBody.TestManual() );
	}
}