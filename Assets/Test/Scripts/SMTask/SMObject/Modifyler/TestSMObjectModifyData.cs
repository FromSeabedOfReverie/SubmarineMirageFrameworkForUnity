//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UTask;
	using SMTask;
	using SMTask.Modifyler;
	using Scene;
	using Extension;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;



	// TODO : コメント追加、整頓



	public class TestSMObjectModifyData : Test {
		Text _text;
		ISMBehaviour _behaviour;


		protected override void Create() {
			Application.targetFrameRate = 30;

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			UnityObject.DontDestroyOnLoad( go );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _behaviour?._object == null ) {
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
					$"    {nameof( b._body._activeState )} : {b._body._activeState}",
					$"    next : {b._body._nextActiveState}",
					")"
				);
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestRegisterAddChildObject ):	CreateTestRegisterAddChildObject();	break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}



		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestHoge() => From( async () => {
			await UTask.Never( _asyncCanceler );
		} );



/*
		・登録追加テスト
		RegisterObject、AddObject、AddChildObject
*/
		void CreateTestRegisterAddChildObject() => TestSMBehaviourUtility.CreateBehaviours( @"
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegisterAddChildObject() => From( async () => {
			Log.Debug( $"{nameof( TestRegisterAddChildObject )}" );


			Log.Debug( $"・RegisterObjectのテスト" );
			3.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				o._type = SMTaskType.DontWork;
				o._scene = SceneManager.s_instance._fsm._scene;
				new LinkData( o ).TestRegisterObject();
			} );


			Log.Debug( $"・AddObjectのテスト" );
			var last = SceneManager.s_instance.GetBehaviour<M1>()._object;
			2.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				new LinkData( null ).TestAddObject( last, o );
			} );

			Log.Debug( $"・{nameof( SMObjectModifyData.AddChildObject )}のテスト" );
			var parent = SceneManager.s_instance.GetBehaviour<M1>()._object;
			3.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				SMObjectModifyData.AddChildObject( parent, o );
			} );


			Log.Debug( $"・RegisterObjectのエラーテスト" );
			try {
				new LinkData( null ).TestRegisterObject();
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				new LinkData( o ).TestRegisterObject();
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				o._scene = SceneManager.s_instance._fsm._scene;
				new LinkData( o ).TestRegisterObject();
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				o._type = SMTaskType.DontWork;
				new LinkData( o ).TestRegisterObject();
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }


			Log.Debug( $"・AddObjectのエラーテスト" );
			try {
				new LinkData( null ).TestAddObject( null, null );
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				new LinkData( null ).TestAddObject( last, null );
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				new LinkData( null ).TestAddObject( null, o );
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }


			Log.Debug( $"・{nameof( SMObjectModifyData.AddChildObject )}のエラーテスト" );
			try {
				SMObjectModifyData.AddChildObject( null, null );
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				SMObjectModifyData.AddChildObject( parent, null );
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, false );
				SMObjectModifyData.AddChildObject( null, o );
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }


			Log.Debug( $"{nameof( TestRegisterAddChildObject )} : end" );
			await UTask.Never( _asyncCanceler );
			Log.Debug( $"{nameof( TestRegisterAddChildObject )} : end" );
		} );
	}
}