//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestCSharp {
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using KoganeUnityLib;
	using Extension;
	using Utility;
	using Debug;
	using TestBase;



	public partial class TestSMLinkedList : SMStandardTest {
		LinkedList<float> _list	{ get; set; } = new LinkedList<float>( new float[] { 0, 1, 2, 3, 4 } );


		protected override void Create() {
			Application.targetFrameRate = 30;
		}

		void Reset() => _list = new LinkedList<float>( new float[] { 0, 1, 2, 3, 4 } );


/*
		・キュー挿入取得テスト
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestEnqueueDequeue() => From( async () => {
			_list.Enqueue( 10 );
			SMLog.Warning( string.Join( ", ", _list ) );
			SMLog.Warning( _list.Dequeue() );
			SMLog.Warning( string.Join( ", ", _list ) );

			await UTask.DontWait();
		} );


/*
		・スタック挿入取得テスト
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestPushPop() => From( async () => {
			_list.Push( 10 );
			SMLog.Warning( string.Join( ", ", _list ) );
			SMLog.Warning( _list.Pop() );
			SMLog.Warning( string.Join( ", ", _list ) );

			await UTask.DontWait();
		} );


/*
		・ノード取得テスト
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestGetNodes() => From( async () => {
			SMLog.Warning( string.Join( ", ", _list.GetNodes().Select( n => n.Value ) ) );
			SMLog.Warning( string.Join( ", ", _list.GetNodes( true ).Select( n => n.Value ) ) );
			SMLog.Warning( string.Join( ", ", _list.GetNodes().Reverse().Select( n => n.Value ) ) );

			await UTask.DontWait();
		} );


/*
		・ノード発見テスト
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestFindNode() => From( async () => {
			SMLog.Warning( _list.FindNode( d => d == 0 ).Value );
			SMLog.Warning( _list.FindNode( d => d == 4, true ).Value );
			SMLog.Warning( _list.GetNodes().LastOrDefault( d => d.Value == 4 ).Value );

			SMLog.Warning( _list.FindNode( d => d == 4 ).Value );
			SMLog.Warning( _list.FindNode( d => d == 0, true ).Value );
			SMLog.Warning( _list.GetNodes().LastOrDefault( d => d.Value == 0 ).Value );

			SMLog.Warning( _list.FindNode( d => d == 10 )?.Value );
			SMLog.Warning( _list.FindNode( d => d == 10, true )?.Value );
			SMLog.Warning( _list.GetNodes().LastOrDefault( d => d.Value == 10 )?.Value );

			await UTask.DontWait();
		} );


/*
		・追加テスト
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestAdd() => From( async () => {
			_list.AddBefore( 3.1f, d => d == 4 );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddBefore( 3.2f, d => d == 4, () => _list.Enqueue( 3.2f ) );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddBefore( 3.3f, d => d == 4, () => _list.Enqueue( 3.3f ), true );
			SMLog.Warning( string.Join( ", ", _list ) );

			_list.AddBefore( 9.1f, d => d == 10 );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddBefore( 9.2f, d => d == 10, () => _list.Enqueue( 9.2f ) );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddBefore( 9.3f, d => d == 10, () => _list.Enqueue( 9.3f ), true );
			SMLog.Warning( string.Join( ", ", _list ) );

			_list.AddAfter( 4.3f, d => d == 4 );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddAfter( 4.2f, d => d == 4, () => _list.Enqueue( 4.2f ) );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddAfter( 4.1f, d => d == 4, () => _list.Enqueue( 4.1f ), true );
			SMLog.Warning( string.Join( ", ", _list ) );

			_list.AddAfter( 11.3f, d => d == 10 );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddAfter( 11.2f, d => d == 10, () => _list.Enqueue( 11.2f ) );
			SMLog.Warning( string.Join( ", ", _list ) );
			_list.AddAfter( 11.1f, d => d == 10, () => _list.Enqueue( 11.1f ), true );
			SMLog.Warning( string.Join( ", ", _list ) );

			await UTask.DontWait();
		} );


/*
		・全削除テスト
*/
		[UnityTest] [Timeout( int.MaxValue )]
		public IEnumerator TestRemoveAll() => From( async () => {
			_list.RemoveAll( d => d % 2 == 0 );
			SMLog.Warning( string.Join( ", ", _list ) );
			Reset();
			_list.RemoveAll( d => d % 2 == 0, d => SMLog.Warning( d ) );
			SMLog.Warning( string.Join( ", ", _list ) );
			Reset();

			_list.RemoveAll( d => d == 10 );
			SMLog.Warning( string.Join( ", ", _list ) );
			Reset();
			_list.RemoveAll( d => d == 10, d => SMLog.Warning( d ) );
			SMLog.Warning( string.Join( ", ", _list ) );
			Reset();

			await UTask.DontWait();
		} );
	}
}