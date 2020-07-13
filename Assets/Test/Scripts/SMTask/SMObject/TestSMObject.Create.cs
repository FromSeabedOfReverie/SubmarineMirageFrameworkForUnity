//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using UTask;
	using SMTask;
	using Extension;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public partial class TestSMObject : Test {
		Text _text;
		ISMBehaviour _behaviour;


		protected override void Create() {
			Application.targetFrameRate = 30;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _behaviour._object == null ) {
					_text.text = string.Empty;
					return;
				}
				var b = _behaviour._object._behaviour;
				_text.text =
					$"{_behaviour._object}\n"
					+ $"{b.GetAboutName()}(\n"
					+ $"    {nameof( b._isInitialized )} : {b._isInitialized}\n"
					+ $"    {nameof( b._isActive )} : {b._isActive}\n"
					+ $"    {nameof( b._body._ranState )} : {b._body._ranState}\n"
					+ $"    {nameof( b._body._activeState )} : {b._body._activeState}\n"
					+ $"    next : {b._body._nextActiveState}\n"
					+ $")\n";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async cancel => {
				Log.Debug( $"start Create{_testName}" );
				switch ( _testName ) {
					case nameof( TestVariable ):					CreateTestVariable();					break;
					case nameof( TestMultiVariable ):				CreateTestMultiVariable();				break;
					case nameof( TestObject ):						CreateTestObject();						break;

					case nameof( TestGetObjects ):				CreateTestGetObjects();				break;
					case nameof( TestGetBehaviour ):					CreateTestGetBehaviour();					break;
					case nameof( TestGetObjectBehaviour ):			CreateTestGetObjectBehaviour();		break;
					case nameof( TestGetInSMMonoBehaviour ):	CreateTestGetInSMMonoBehaviour();	break;
					case nameof( TestGetSMBehaviour ):				CreateTestGetSMBehaviour();				break;

					case nameof( TestRunB1 ):						CreateTestRunB1();						break;
					case nameof( TestRunB2 ):						CreateTestRunB2();						break;
					case nameof( TestRunB3 ):						CreateTestRunB3();						break;
					case nameof( TestRunM1 ):						CreateTestRunM1();						break;
					case nameof( TestRunM2 ):						CreateTestRunM2();						break;
					case nameof( TestRunM3 ):						CreateTestRunM3();						break;

					case nameof( TestRunBrothers1 ):				CreateTestRunBrothers1();				break;
					case nameof( TestRunBrothers2 ):				CreateTestRunBrothers2();				break;
					case nameof( TestRunBrothers3 ):				CreateTestRunBrothers3();				break;

					case nameof( TestRunChildren1 ):				CreateTestRunChildren1();				break;
					case nameof( TestRunChildren2 ):				CreateTestRunChildren2();				break;
					case nameof( TestRunChildren3 ):				CreateTestRunChildren3();				break;

					case nameof( TestRunByActiveState1 ):			CreateTestRunByActiveState1();			break;
					case nameof( TestRunByActiveState2 ):			CreateTestRunByActiveState2();			break;
					case nameof( TestRunByActiveState3 ):			CreateTestRunByActiveState3();			break;
				}
				Log.Debug( $"end Create{_testName}" );

				await UTask.DontWait();
			} );
		}
	}
}