//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
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
					$"    {nameof( b._body._isInitialActive )} : {b._body._isInitialActive}",
					")"
				);
			}) ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
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
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}
	}
}