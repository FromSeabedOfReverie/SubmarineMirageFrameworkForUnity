//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask.Modifyler {
	using System;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using UnityEngine;
	using KoganeUnityLib;
	using UTask;
	using SMTask;
	using SMTask.Modifyler;
	using Scene;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public class TestSMObjectModifyData : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestRegisterAddChildObject ):	CreateTestRegisterAddChildObject();	break;
					case nameof( TestSetTopAllObjectData ):		CreateTestSetTopAllObjectData();	break;
					case nameof( TestUnLinkObject ):			CreateTestUnLinkObject();			break;
				}
				Log.Debug( $"end {nameof( Create )}{_testName}" );

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
			Log.Debug( $"{nameof( TestRegisterAddChildObject )}" );


			Log.Debug( $"・RegisterObjectのテスト" );
			3.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				o._type = SMTaskType.DontWork;
				o._scene = SceneManager.s_instance._fsm._scene;
				new LinkData( o ).TestRegisterObject();
			} );


			Log.Debug( $"・AddObjectのテスト" );
			var last = SceneManager.s_instance.GetBehaviour<M1>()._object;
			2.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new LinkData( null ).TestAddObject( last, o );
			} );
			{
				var p = last._next;
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				SMObjectModifyData.AddChildObject( p, o );
				var c = p._child;
				2.Times( () => {
					b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
					o = new SMObject( b.gameObject, new [] { b }, null, true );
					new LinkData( null ).TestAddObject( c, o );
				} );
			}

			Log.Debug( $"・{nameof( SMObjectModifyData.AddChildObject )}のテスト" );
			var parent = SceneManager.s_instance.GetBehaviour<M1>()._object;
			3.Times( () => {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				SMObjectModifyData.AddChildObject( parent, o );
			} );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );


			Log.Debug( $"・RegisterObjectのエラーテスト" );
			try {
				new LinkData( null ).TestRegisterObject();
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				new LinkData( o ).TestRegisterObject();
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }
			try {
				var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
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
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
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
				var o = new SMObject( b.gameObject, new [] { b }, null, true );
				SMObjectModifyData.AddChildObject( null, o );
			} catch ( Exception e )	{ Log.Error( e, Log.Tag.SMTask ); }


			await UTask.Never( _asyncCanceler );
		} );



/*
		・頂上設定、全データ設定テスト
		SetTopObject、SetAllObjectData、を確認
*/
		void CreateTestSetTopAllObjectData() {}

		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestSetTopAllObjectData() => From( async () => {
			Log.Debug( $"{nameof( TestSetTopAllObjectData )}" );


			Log.Debug( $"・{nameof( SMObjectModifyData.SetTopObject )}のテスト" );
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetTopObject( p );
			}
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetTopObject( p._parent );
			}
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetTopObject( p._parent._parent );
			}


			Log.Debug( $"・{nameof( SMObjectModifyData.SetAllObjectData )}のテスト" );
			{
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				var top = p._parent._parent;
				SMObjectModifyData.SetAllObjectData( top );
				SMObjectModifyData.SetAllObjectData( top );
				{
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M2" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
					SMObjectModifyData.SetAllObjectData( top );
				}
				{
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M3" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
					SMObjectModifyData.SetAllObjectData( top );
				}
				{
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M4" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
					SMObjectModifyData.SetAllObjectData( top );
				}
				p.Dispose();
				SMObjectModifyData.SetAllObjectData( top );
			}


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );


			Log.Debug( "・エラーテスト" );
			try {
				SMObjectModifyData.SetTopObject( null );
			} catch ( Exception e )	{ Log.Error( e ); }

			try {
				SMObjectModifyData.SetAllObjectData( null );
			} catch ( Exception e )	{ Log.Error( e ); }

			try {
				SMObject p = null;
				3.Times( () => {
					var b = (SMMonoBehaviour)TestSMBehaviourUtility.CreateBehaviours( "M1" );
					p = new SMObject( b.gameObject, new [] { b }, p, true );
				} );
				SMObjectModifyData.SetAllObjectData( p );
			} catch ( Exception e )	{ Log.Error( e ); }


			await UTask.Never( _asyncCanceler );
		} );



/*
		・リンク解除テスト
		UnLinkObject、を確認
*/
		void CreateTestUnLinkObject() => TestSMBehaviourUtility.CreateBehaviours( @"
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
		public IEnumerator TestUnLinkObject() => From( async () => {
			Log.Debug( $"{nameof( TestUnLinkObject )}" );

			Log.Debug( "・兄弟解除テスト" );
			var o = SceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._next;
			SMObjectModifyData.UnLinkObject( o );

			Log.Debug( "・登録最初解除テスト" );
			o = SceneManager.s_instance.GetBehaviour<M1>()._object;
			SMObjectModifyData.UnLinkObject( o );

			Log.Debug( $"・子供兄弟解除テスト" );
			o = SceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._child._next;
			SMObjectModifyData.UnLinkObject( o );

			Log.Debug( $"・子供最初解除テスト" );
			o = SceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._child;
			SMObjectModifyData.UnLinkObject( o );

			Log.Debug( $"・親解除テスト" );
			o = SceneManager.s_instance.GetBehaviour<M1>()._object;
			SMObjectModifyData.UnLinkObject( o );

			Log.Debug( $"・中間子解除テスト" );
			o = SceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._child;
			SMObjectModifyData.UnLinkObject( o );

			Log.Debug( $"・孫解除テスト" );
			o = SceneManager.s_instance.GetBehaviour<M1>()._object;
			o = o._next._child._child;
			SMObjectModifyData.UnLinkObject( o );


			await UTask.WaitWhile( _asyncCanceler, () => !Input.GetKey( KeyCode.Return ) );

			Log.Debug( $"・エラーテスト" );
			try {
				SMObjectModifyData.UnLinkObject( null );
			} catch ( Exception e )	{ Log.Error( e ); }

			await UTask.Never( _asyncCanceler );
		} );
	}
}