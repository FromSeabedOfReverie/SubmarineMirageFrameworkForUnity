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

			var top = SceneManager.s_instance.GetProcess<M1>()._hierarchy;
			var center = SceneManager.s_instance.GetProcess<M3>()._hierarchy;
			var bottom = SceneManager.s_instance.GetProcess<M5>()._hierarchy;
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

			var top = SceneManager.s_instance.GetProcess<M1>()._hierarchy;
			TestProcessUtility.LogHierarchy( "top", top );

			TestProcessUtility.LogProcess( "top.GetProcess<M1>",	top.GetProcess<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M1 )",	top.GetProcess( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M2>",	top.GetProcess<M2>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M2 )",	top.GetProcess( typeof( M2 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M3>",	top.GetProcess<M3>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M3 )",	top.GetProcess( typeof( M3 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcesses<M1>",	top.GetProcesses<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M1 )",	top.GetProcesses( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M2>",	top.GetProcesses<M2>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M2 )",	top.GetProcesses( typeof( M2 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M3>",	top.GetProcesses<M3>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M3 )",	top.GetProcesses( typeof( M3 ) ) );

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

			var top = SceneManager.s_instance.GetProcess<M1>()._hierarchy;
			var center = SceneManager.s_instance.GetProcess<M3>()._hierarchy;
			var bottom = SceneManager.s_instance.GetProcess<M5>()._hierarchy;
			TestProcessUtility.LogHierarchy( "top", top );
			TestProcessUtility.LogHierarchy( "center", center );
			TestProcessUtility.LogHierarchy( "bottom", bottom );

			TestProcessUtility.LogProcess( "top.GetProcessInParent<M1>",		top.GetProcessInParent<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( M1 )",		top.GetProcessInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInParent<M1>",		center.GetProcessInParent<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M1 )",	center.GetProcessInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent<M1>",		bottom.GetProcessInParent<M1>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent( M1 )",	bottom.GetProcessInParent( typeof( M1 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<M1>",		top.GetProcessesInParent<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( M1 )",		top.GetProcessesInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M1>",		center.GetProcessesInParent<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M1 )",	center.GetProcessesInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent<M1>",		bottom.GetProcessesInParent<M1>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent( M1 )",	bottom.GetProcessesInParent( typeof( M1 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInChildren<M1>",			top.GetProcessInChildren<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( M1 )",		top.GetProcessInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M1>",		center.GetProcessInChildren<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M1 )",		center.GetProcessInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren<M1>",		bottom.GetProcessInChildren<M1>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren( M1 )",		bottom.GetProcessInChildren( typeof( M1 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<M1>",		top.GetProcessesInChildren<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( M1 )",	top.GetProcessesInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M1>",	center.GetProcessesInChildren<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M1 )",	center.GetProcessesInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren<M1>",	bottom.GetProcessesInChildren<M1>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren( M1 )",	bottom.GetProcessesInChildren( typeof( M1 ) ) );


			TestProcessUtility.LogProcess( "top.GetProcessInParent<M4>",		top.GetProcessInParent<M4>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( M4 )",		top.GetProcessInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInParent<M4>",		center.GetProcessInParent<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M4 )",	center.GetProcessInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent<M4>",		bottom.GetProcessInParent<M4>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInParent( M4 )",	bottom.GetProcessInParent( typeof( M4 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<M4>",		top.GetProcessesInParent<M4>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( M4 )",		top.GetProcessesInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M4>",		center.GetProcessesInParent<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M4 )",	center.GetProcessesInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent<M4>",		bottom.GetProcessesInParent<M4>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInParent( M4 )",	bottom.GetProcessesInParent( typeof( M4 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInChildren<M4>",		top.GetProcessInChildren<M4>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( M4 )",	top.GetProcessInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M4>",	center.GetProcessInChildren<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M4 )",	center.GetProcessInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren<M4>",	bottom.GetProcessInChildren<M4>() );
			TestProcessUtility.LogProcess( "bottom.GetProcessInChildren( M4 )",	bottom.GetProcessInChildren( typeof( M4 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<M4>",		top.GetProcessesInChildren<M4>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( M4 )",	top.GetProcessesInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M4>",	center.GetProcessesInChildren<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M4 )",	center.GetProcessesInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren<M4>",	bottom.GetProcessesInChildren<M4>() );
			TestProcessUtility.LogProcesses( "bottom.GetProcessesInChildren( M4 )",	bottom.GetProcessesInChildren( typeof( M4 ) ) );

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

			var top = SceneManager.s_instance.GetProcess<M1>();
			var center = SceneManager.s_instance.GetProcess<M3>();
			TestProcessUtility.LogHierarchy( "top", top._hierarchy );
			TestProcessUtility.LogHierarchy( "center", center._hierarchy );

			TestProcessUtility.LogProcess( "top.GetProcess<M1>",	top.GetProcess<M1>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M1 )",	top.GetProcess( typeof( M1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M2>",	top.GetProcess<M2>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M2 )",	top.GetProcess( typeof( M2 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<M3>",	top.GetProcess<M3>() );
			TestProcessUtility.LogProcess( "top.GetProcess( M3 )",	top.GetProcess( typeof( M3 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcesses<M1>",	top.GetProcesses<M1>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M1 )",	top.GetProcesses( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M2>",	top.GetProcesses<M2>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M2 )",	top.GetProcesses( typeof( M2 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<M3>",	top.GetProcesses<M3>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( M3 )",	top.GetProcesses( typeof( M3 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInParent<M1>",			center.GetProcessInParent<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M1 )",		center.GetProcessInParent( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M1>",		center.GetProcessesInParent<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M1 )",	center.GetProcessesInParent( typeof( M1 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M1>",		center.GetProcessInChildren<M1>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M1 )",		center.GetProcessInChildren( typeof( M1 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M1>",	center.GetProcessesInChildren<M1>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M1 )",	center.GetProcessesInChildren( typeof( M1 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInParent<M4>",			center.GetProcessInParent<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInParent( M4 )",		center.GetProcessInParent( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent<M4>",		center.GetProcessesInParent<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInParent( M4 )",	center.GetProcessesInParent( typeof( M4 ) ) );

			TestProcessUtility.LogProcess( "center.GetProcessInChildren<M4>",		center.GetProcessInChildren<M4>() );
			TestProcessUtility.LogProcess( "center.GetProcessInChildren( M4 )",		center.GetProcessInChildren( typeof( M4 ) ) );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren<M4>",	center.GetProcessesInChildren<M4>() );
			TestProcessUtility.LogProcesses( "center.GetProcessesInChildren( M4 )",	center.GetProcessesInChildren( typeof( M4 ) ) );

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

			var top = SceneManager.s_instance.GetProcess<B1>()._hierarchy;
			TestProcessUtility.LogHierarchy( "top", top );

			TestProcessUtility.LogHierarchies( "top.GetHierarchiesInChildren",	top.GetAllChildren() );
			TestProcessUtility.LogHierarchies( "top.GetHierarchiesInParent",	top.GetAllParents() );
			
			TestProcessUtility.LogProcess( "top.GetProcess<B1>",	top.GetProcess<B1>() );
			TestProcessUtility.LogProcess( "top.GetProcess( B1 )",	top.GetProcess( typeof( B1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcess<B2>",	top.GetProcess<B2>() );
			TestProcessUtility.LogProcess( "top.GetProcess( B2 )",	top.GetProcess( typeof( B2 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcesses<B1>",	top.GetProcesses<B1>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( B1 )",	top.GetProcesses( typeof( B1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcesses<B2>",	top.GetProcesses<B2>() );
			TestProcessUtility.LogProcesses( "top.GetProcesses( B2 )",	top.GetProcesses( typeof( B2 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInParent<B1>",	top.GetProcessInParent<B1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( B1 )",	top.GetProcessInParent( typeof( B1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcessInParent<B2>",	top.GetProcessInParent<B2>() );
			TestProcessUtility.LogProcess( "top.GetProcessInParent( B2 )",	top.GetProcessInParent( typeof( B2 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<B1>",	top.GetProcessesInParent<B1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( B1 )",	top.GetProcessesInParent( typeof( B1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent<B2>",	top.GetProcessesInParent<B2>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInParent( B2 )",	top.GetProcessesInParent( typeof( B2 ) ) );

			TestProcessUtility.LogProcess( "top.GetProcessInChildren<B1>",		top.GetProcessInChildren<B1>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( B1 )",	top.GetProcessInChildren( typeof( B1 ) ) );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren<B2>",		top.GetProcessInChildren<B2>() );
			TestProcessUtility.LogProcess( "top.GetProcessInChildren( B2 )",	top.GetProcessInChildren( typeof( B2 ) ) );

			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<B1>",		top.GetProcessesInChildren<B1>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( B1 )",	top.GetProcessesInChildren( typeof( B1 ) ) );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren<B2>",		top.GetProcessesInChildren<B2>() );
			TestProcessUtility.LogProcesses( "top.GetProcessesInChildren( B2 )",	top.GetProcessesInChildren( typeof( B2 ) ) );

			while ( true )	{ yield return null; }
		}
	}
}