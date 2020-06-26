//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestProcess {
	using System;
	using System.Threading;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine.TestTools;
	using KoganeUnityLib;
	using Scene;
	using Extension;
	using Debug;
	using Test;



	// TODO : コメント追加、整頓



	public partial class TestProcessHierarchy : Test {
/*
		・階層取得テスト
		GetHierarchiesInParent、GetHierarchiesInChildren、動作確認
*/
		void CreateTestGetHierarchies() => TestProcessUtility.CreateMonoBehaviourProcess( @"
			M1, M1, M2,
				null,
					M2,
					M1,
						M3,
							null,
								M4,
									M5,
							null,
								M4, M4,
				null,
					M1,
						M6,
							M1, M1
				M1,
		" );

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetHierarchies() => From( TestGetHierarchiesSub() );
		IEnumerator TestGetHierarchiesSub() {
			Log.Debug( "TestGetHierarchiesSub" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			var center = SceneManager.s_instance.GetBehaviour<M3>()._object;
			var bottom = SceneManager.s_instance.GetBehaviour<M5>()._object;
			TestProcessUtility.LogHierarchy( "top", top );
			TestProcessUtility.LogHierarchy( "center", center );
			TestProcessUtility.LogHierarchy( "bottom", bottom );

			TestProcessUtility.LogHierarchies( "top.GetHierarchiesInChildren",		top.GetAllChildren() );
			TestProcessUtility.LogHierarchies( "center.GetHierarchiesInChildren",	center.GetAllChildren() );
			TestProcessUtility.LogHierarchies( "bottom.GetHierarchiesInChildren",	bottom.GetAllChildren() );

			TestProcessUtility.LogHierarchies( "top.GetHierarchiesInParent",	top.GetAllParents() );
			TestProcessUtility.LogHierarchies( "center.GetHierarchiesInParent",	center.GetAllParents() );
			TestProcessUtility.LogHierarchies( "bottom.GetHierarchiesInParent",	bottom.GetAllParents() );

			while ( true )	{ yield return null; }
		}


/*
		・取得テスト
		GetProcess<T>、GetProcess(type)、GetProcesses<T>、GetProcesses(type)、動作確認
*/
		void CreateTestGetProcess()
			=> CreateTestGetHierarchies();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetProcess() => From( TestGetProcessSub() );
		IEnumerator TestGetProcessSub() {
			Log.Debug( "TestGetProcessSub" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			TestProcessUtility.LogHierarchy( "top", top );

			TestProcessUtility.LogProcess( "top.GetProcess<M1>",	top.GetBehaviour<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M1 )",	top.GetBehaviour( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M2>",	top.GetBehaviour<M2>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M2 )",	top.GetBehaviour( typeof( M2 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M3>",	top.GetBehaviour<M3>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M3 )",	top.GetBehaviour( typeof( M3 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcesses<M1>",	top.GetBehaviours<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M1 )",	top.GetBehaviours( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M2>",	top.GetBehaviours<M2>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M2 )",	top.GetBehaviours( typeof( M2 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M3>",	top.GetBehaviours<M3>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M3 )",	top.GetBehaviours( typeof( M3 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・取得テスト
		GetProcessInParent<T>、GetProcessInParent(type)
		GetProcessesInParent<T>、GetProcessesInParent(type)
		GetProcessInChildren<T>、GetProcessInChildren(type)
		GetProcessesInChildren<T>、GetProcessesInChildren(type)
*/
		void CreateTestGetHierarchyProcess()
			=> CreateTestGetHierarchies();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetHierarchyProcess() => From( TestGetHierarchyProcessSub() );
		IEnumerator TestGetHierarchyProcessSub() {
			Log.Debug( "TestGetHierarchyProcessSub" );

			var top = SceneManager.s_instance.GetBehaviour<M1>()._object;
			var center = SceneManager.s_instance.GetBehaviour<M3>()._object;
			var bottom = SceneManager.s_instance.GetBehaviour<M5>()._object;
			TestProcessUtility.LogHierarchy( "top", top );
			TestProcessUtility.LogHierarchy( "center", center );
			TestProcessUtility.LogHierarchy( "bottom", bottom );

			TestProcessUtility.LogProcess( "top.GetProcessInParent<M1>",		top.GetBehaviourInParent<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( M1 )",		top.GetBehaviourInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInParent<M1>",		center.GetBehaviourInParent<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M1 )",	center.GetBehaviourInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent<M1>",		bottom.GetBehaviourInParent<M1>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent( M1 )",	bottom.GetBehaviourInParent( typeof( M1 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<M1>",		top.GetBehavioursInParent<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( M1 )",		top.GetBehavioursInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M1>",		center.GetBehavioursInParent<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M1 )",	center.GetBehavioursInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent<M1>",		bottom.GetBehavioursInParent<M1>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent( M1 )",	bottom.GetBehavioursInParent( typeof( M1 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInChildren<M1>",			top.GetBehaviourInChildren<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( M1 )",		top.GetBehaviourInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M1>",		center.GetBehaviourInChildren<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M1 )",		center.GetBehaviourInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren<M1>",		bottom.GetBehaviourInChildren<M1>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren( M1 )",		bottom.GetBehaviourInChildren( typeof( M1 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<M1>",		top.GetBehavioursInChildren<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( M1 )",	top.GetBehavioursInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M1>",	center.GetBehavioursInChildren<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M1 )",	center.GetBehavioursInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren<M1>",	bottom.GetBehavioursInChildren<M1>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren( M1 )",	bottom.GetBehavioursInChildren( typeof( M1 ) ) );


			TestProcessUtility.LogProcess( "top.GetProcessInParent<M4>",		top.GetBehaviourInParent<M4>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( M4 )",		top.GetBehaviourInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInParent<M4>",		center.GetBehaviourInParent<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M4 )",	center.GetBehaviourInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent<M4>",		bottom.GetBehaviourInParent<M4>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent( M4 )",	bottom.GetBehaviourInParent( typeof( M4 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<M4>",		top.GetBehavioursInParent<M4>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( M4 )",		top.GetBehavioursInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M4>",		center.GetBehavioursInParent<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M4 )",	center.GetBehavioursInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent<M4>",		bottom.GetBehavioursInParent<M4>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent( M4 )",	bottom.GetBehavioursInParent( typeof( M4 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInChildren<M4>",		top.GetBehaviourInChildren<M4>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( M4 )",	top.GetBehaviourInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M4>",	center.GetBehaviourInChildren<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M4 )",	center.GetBehaviourInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren<M4>",	bottom.GetBehaviourInChildren<M4>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren( M4 )",	bottom.GetBehaviourInChildren( typeof( M4 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<M4>",		top.GetBehavioursInChildren<M4>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( M4 )",	top.GetBehavioursInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M4>",	center.GetBehavioursInChildren<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M4 )",	center.GetBehavioursInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren<M4>",	bottom.GetBehavioursInChildren<M4>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren( M4 )",	bottom.GetBehavioursInChildren( typeof( M4 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・Process内の取得テスト
*/
		void CreateTestGetInMonoBehaviourProcess()
			=> CreateTestGetHierarchies();

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetInMonoBehaviourProcess() => From( TestGetInMonoBehaviourProcessSub() );
		IEnumerator TestGetInMonoBehaviourProcessSub() {
			Log.Debug( "TestGetInMonoBehaviourProcessSub" );

			var top = SceneManager.s_instance.GetBehaviour<M1>();
			var center = SceneManager.s_instance.GetBehaviour<M3>();
			TestProcessUtility.LogHierarchy( "top", top._object );
			TestProcessUtility.LogHierarchy( "center", center._object );

			TestProcessUtility.LogProcess( "top.GetProcess<M1>",	top.GetBehaviour<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M1 )",	top.GetBehaviour( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M2>",	top.GetBehaviour<M2>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M2 )",	top.GetBehaviour( typeof( M2 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M3>",	top.GetBehaviour<M3>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M3 )",	top.GetBehaviour( typeof( M3 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcesses<M1>",	top.GetBehaviours<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M1 )",	top.GetBehaviours( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M2>",	top.GetBehaviours<M2>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M2 )",	top.GetBehaviours( typeof( M2 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M3>",	top.GetBehaviours<M3>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M3 )",	top.GetBehaviours( typeof( M3 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInParent<M1>",			center.GetBehaviourInParent<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M1 )",		center.GetBehaviourInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M1>",		center.GetBehavioursInParent<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M1 )",	center.GetBehavioursInParent( typeof( M1 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M1>",		center.GetBehaviourInChildren<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M1 )",		center.GetBehaviourInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M1>",	center.GetBehavioursInChildren<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M1 )",	center.GetBehavioursInChildren( typeof( M1 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInParent<M4>",			center.GetBehaviourInParent<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M4 )",		center.GetBehaviourInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M4>",		center.GetBehavioursInParent<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M4 )",	center.GetBehavioursInParent( typeof( M4 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M4>",		center.GetBehaviourInChildren<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M4 )",		center.GetBehaviourInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M4>",	center.GetBehavioursInChildren<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M4 )",	center.GetBehavioursInChildren( typeof( M4 ) ) );

			while ( true )	{ yield return null; }
		}


/*
		・BaseProcessの取得テスト
*/
		void CreateTestGetBaseProcess() {
			new Type[] { typeof( B1 ), typeof( B2 ), typeof( B3 ), typeof( B4 ), typeof( B5 ), typeof( B6 ) }
				.ForEach( t => t.Create() );
		}

		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestGetBaseProcess() => From( TestGetBaseProcessSub() );
		IEnumerator TestGetBaseProcessSub() {
			Log.Debug( "TestGetBaseProcessSub" );

			var top = SceneManager.s_instance.GetBehaviour<B1>()._object;
			TestProcessUtility.LogHierarchy( "top", top );

			TestProcessUtility.LogHierarchies( "top.GetHierarchiesInChildren",	top.GetAllChildren() );
			TestProcessUtility.LogHierarchies( "top.GetHierarchiesInParent",	top.GetAllParents() );
			
			TestProcessUtility.LogProcess( "top.GetProcess<B1>",	top.GetBehaviour<B1>() );
			TestProcessUtility.LogProcess( "top.GetProcess( B1 )",	top.GetBehaviour( typeof( B1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<B2>",	top.GetBehaviour<B2>() );
			TestProcessUtility.LogProcess( "top.GetProcess( B2 )",	top.GetBehaviour( typeof( B2 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcesses<B1>",	top.GetBehaviours<B1>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( B1 )",	top.GetBehaviours( typeof( B1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<B2>",	top.GetBehaviours<B2>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( B2 )",	top.GetBehaviours( typeof( B2 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInParent<B1>",	top.GetBehaviourInParent<B1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( B1 )",	top.GetBehaviourInParent( typeof( B1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcessInParent<B2>",	top.GetBehaviourInParent<B2>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( B2 )",	top.GetBehaviourInParent( typeof( B2 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<B1>",	top.GetBehavioursInParent<B1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( B1 )",	top.GetBehavioursInParent( typeof( B1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<B2>",	top.GetBehavioursInParent<B2>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( B2 )",	top.GetBehavioursInParent( typeof( B2 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInChildren<B1>",		top.GetBehaviourInChildren<B1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( B1 )",	top.GetBehaviourInChildren( typeof( B1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren<B2>",		top.GetBehaviourInChildren<B2>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( B2 )",	top.GetBehaviourInChildren( typeof( B2 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<B1>",		top.GetBehavioursInChildren<B1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( B1 )",	top.GetBehavioursInChildren( typeof( B1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<B2>",		top.GetBehavioursInChildren<B2>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( B2 )",	top.GetBehavioursInChildren( typeof( B2 ) ) );

			while ( true )	{ yield return null; }
		}
	}
}