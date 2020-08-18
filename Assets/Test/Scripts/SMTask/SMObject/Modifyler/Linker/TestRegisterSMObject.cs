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



	public class TestRegisterSMObject : Test {
		protected override void Create() {
			Application.targetFrameRate = 30;

			_createEvent.AddLast( async canceler => {
				Log.Debug( $"start {nameof( Create )}{_testName}" );
				switch ( _testName ) {
					case nameof( TestRegisterAddChildObject ):	CreateTestRegisterAddChildObject();	break;
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

			await UTask.Never( _asyncCanceler );
		} );
	}
}