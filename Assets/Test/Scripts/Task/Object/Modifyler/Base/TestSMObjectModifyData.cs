//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask.Modifyler {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UnityEngine;
	using KoganeUnityLib;
	using Task;
	using Task.Behaviour;
	using Task.Object;
	using Task.Object.Modifyler;
	using Scene;
	using Utility;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestSMObjectModifyData : SMStandardTest {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				SMLog.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestRegisterAddChildObject ):	CreateTestRegisterAddChildObject();	break;
					case nameof( TestSetTopAllObjectData ):		CreateTestSetTopAllObjectData();	break;
					case nameof( TestUnlinkObject ):			CreateTestUnlinkObject();			break;
				}
				SMLog.Debug( $"end {nameof( Create )}{_testName}" );

				await UTask.DontWait();
			} );
		}



/*
		・登録追加テスト
		RegisterObject、AddObject、AddChildObject、を確認
*/
		void CreateTestRegisterAddChildObject() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRegisterAddChildObject() => From( async () => {
			SMLog.Debug( $"{nameof( TestRegisterAddChildObject )}" );


			SMLog.Debug( $"・RegisterObjectのテスト" );
			3.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				o._type = SMTaskType.DontWork;
				o._scene = SMSceneManager.s_instance._fsm._scene;
				new LinkData( o ).TestRegisterObject();
			} );


			SMLog.Debug( $"・AddObjectのテスト" );
			var last = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			2.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new LinkData( null ).TestAddObject( last, o );
			} );
			{
				var p = last._next;
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				SMObjectModifyData.AddChildObject( p, o );
				var c = p._child;
				2.Times( () => {
					b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
					o = new SMObject( b.gameObject, new [] { b }, null, true );
					new LinkData( null ).TestAddObject( c, o );
				} );
			}

			SMLog.Debug( $"・{nameof( SMObjectModifyData.AddChildObject )}のテスト" );
			var parent = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			3.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				SMObjectModifyData.AddChildObject( parent, o );
			} );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );


			SMLog.Debug( $"・RegisterObjectのエラーテスト" );
			try {
				new LinkData( null ).TestRegisterObject();
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new LinkData( o ).TestRegisterObject();
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				o._type = SMTaskType.DontWork;
				new LinkData( o ).TestRegisterObject();
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }


			SMLog.Debug( $"・AddObjectのエラーテスト" );
			try {
				new LinkData( null ).TestAddObject( null, null );
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }
			try {
				new LinkData( null ).TestAddObject( last, null );
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new LinkData( null ).TestAddObject( null, o );
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }


			SMLog.Debug( $"・{nameof( SMObjectModifyData.AddChildObject )}のエラーテスト" );
			try {
				SMObjectModifyData.AddChildObject( null, null );
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }
			try {
				SMObjectModifyData.AddChildObject( parent, null );
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				SMObjectModifyData.AddChildObject( null, o );
			} catch ( Exception e )	{ SMLog.Error( e, SMLogTag.Task ); }


			await UTask.Never( _asyncCanceler );
		} );



/*
		・頂上設定、全データ設定テスト
		SetTopObject、SetAllObjectData、を確認
*/
		void CreateTestSetTopAllObjectData() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSetTopAllObjectData() => From( async () => {
			SMLog.Debug( $"{nameof( TestSetTopAllObjectData )}" );


			SMLog.Debug( $"・{nameof( SMObjectModifyData.SetTopObject )}のテスト" );
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetTopObject( p );
			}
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetTopObject( p._parent );
			}
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetTopObject( p._parent._parent );
			}


			SMLog.Debug( $"・{nameof( SMObjectModifyData.SetAllObjectData )}のテスト" );
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				var top = p._parent._parent;
				SMObjectModifyData.SetAllObjectData( top );
				SMObjectModifyData.SetAllObjectData( top );
				{
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M2" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
					SMObjectModifyData.SetAllObjectData( top );
				}
				{
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M3" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
					SMObjectModifyData.SetAllObjectData( top );
				}
				{
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M4" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
					SMObjectModifyData.SetAllObjectData( top );
				}
				p.Dispose();
				SMObjectModifyData.SetAllObjectData( top );
			}


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );


			SMLog.Debug( "・エラーテスト" );
			try {
				SMObjectModifyData.SetTopObject( null );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			try {
				SMObjectModifyData.SetAllObjectData( null );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			try {
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourSMUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetAllObjectData( p );
			} catch ( Exception e )	{ SMLog.Error( e ); }


			await UTask.Never( _asyncCanceler );
		} );



/*
		・リンク解除テスト
		UnlinkObject、を確認
*/
		void CreateTestUnlinkObject() => TestSMBehaviourSMUtility.CreateBehaviours( @"
			M1,
			M1,
			M1,
				M1,
				M1,
				M1,
			M1,
				M1,
					M1,
			M1,
				M1,
					M1,
		" );

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestUnlinkObject() => From( async () => {
			SMLog.Debug( $"{nameof( TestUnlinkObject )}" );

			SMLog.Debug( "・兄弟解除テスト" );
			var o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._next;
			SMObjectModifyData.UnlinkObject( o );

			SMLog.Debug( "・登録最初解除テスト" );
			o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			SMObjectModifyData.UnlinkObject( o );

			SMLog.Debug( $"・子供兄弟解除テスト" );
			o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._child._next;
			SMObjectModifyData.UnlinkObject( o );

			SMLog.Debug( $"・子供最初解除テスト" );
			o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._child;
			SMObjectModifyData.UnlinkObject( o );

			SMLog.Debug( $"・親解除テスト" );
			o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			SMObjectModifyData.UnlinkObject( o );

			SMLog.Debug( $"・中間子解除テスト" );
			o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._child;
			SMObjectModifyData.UnlinkObject( o );

			SMLog.Debug( $"・孫解除テスト" );
			o = SMSceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._next._child._child;
			SMObjectModifyData.UnlinkObject( o );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );

			SMLog.Debug( $"・エラーテスト" );
			try {
				SMObjectModifyData.UnlinkObject( null );
			} catch ( Exception e )	{ SMLog.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}