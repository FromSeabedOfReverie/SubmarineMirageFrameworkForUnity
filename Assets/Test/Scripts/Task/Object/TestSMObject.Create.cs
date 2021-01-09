//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using KoganeUnityLib;
	using Task.Behaviour;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public partial class TestSMObject : SMStandardTest {
		Text _text	{ get; set; }
		ISMBehaviour _behaviour	{ get; set; }


		protected override void Create() {
			Application.targetFrameRate = 30;

			Resources.Load<GameObject>( "TestCamera" ).Instantiate();
			var go = Resources.Load<GameObject>( "TestCanvas" ).Instantiate();
			go.DontDestroyOnLoad();
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast(Observable.EveryLateUpdate().Subscribe( (System.Action<long>)(_ => {
				if (_behaviour?._object == null ) {
					_text.text = string.Empty;
					return;
				}
				var b = _behaviour._object._behaviour;
				_text.text = string.Join( "\n",
					$"{_behaviour._object}",
					$"{b.GetAboutName()}(",
					$"    {nameof( b._isInitialized )} : {b._isInitialized}",
					$"    {nameof( b._isActive )} : {b._isActive}",
					$"    {nameof( b._body._ranState )} : {b._body._ranState}",
					$"    {nameof( b._body._isActive )} : {b._body._isActive}",
					$"    {nameof( b._body._isRunInitialActive )} : {b._body._isRunInitialActive}",
					")"
				);
			}) ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestCreate ):						CreateTestCreate();					break;
					case nameof( TestCreateBehaviours ):			CreateTestCreateBehaviours();		break;
					case nameof( TestCreateObjects1 ):				CreateTestCreateObjects1();			break;
					case nameof( TestCreateObjects2 ):				CreateTestCreateObjects2();			break;

					case nameof( TestGetFirstLastChild ):			CreateTestGetFirstLastChild();		break;
					case nameof( TestGetBrothersChildren ):			CreateTestGetBrothersChildren();	break;
					case nameof( TestGetAllParentsChildren ):		CreateTestGetAllParentsChildren();	break;
					case nameof( TestGetBehavioursLast ):			CreateTestGetBehavioursLast();		break;
					case nameof( TestGetBehavioursParentChildren ):	CreateTestGetBehavioursParentChildren();break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}
	}
}