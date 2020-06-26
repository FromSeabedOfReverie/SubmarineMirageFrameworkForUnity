//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestProcess {
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using SMTask;
	using Extension;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public partial class TestProcessHierarchy : Test {
		Text _text;
		ISMBehavior _process;


		protected override void Create() {
			Application.targetFrameRate = 30;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _process._object == null ) {
					_text.text = string.Empty;
					return;
				}
				var p = _process._object._processes.FirstOrDefault();
				_text.text =
					$"{_process._object}\n"
					+ $"{p.GetAboutName()}(\n"
					+ $"    _isInitialized : {p._isInitialized}\n"
					+ $"    _isActive : {p._isActive}\n"
					+ $"    _ranState : {p._body._ranState}\n"
					+ $"    _activeState : {p._body._activeState}\n"
					+ $"    next : {p._body._nextActiveState}\n"
					+ $")\n";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async cancel => {
				Log.Debug( $"start Create{_testName}" );
				switch ( _testName ) {
					case nameof( TestVariable ):					CreateTestVariable();					break;
					case nameof( TestMultiVariable ):				CreateTestMultiVariable();				break;
					case nameof( TestHierarchy ):					CreateTestHierarchy();					break;

					case nameof( TestGetHierarchies ):				CreateTestGetHierarchies();				break;
					case nameof( TestGetProcess ):					CreateTestGetProcess();					break;
					case nameof( TestGetHierarchyProcess ):			CreateTestGetHierarchyProcess();		break;
					case nameof( TestGetInMonoBehaviourProcess ):	CreateTestGetInMonoBehaviourProcess();	break;
					case nameof( TestGetBaseProcess ):				CreateTestGetBaseProcess();				break;

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
			} );
		}
	}
}