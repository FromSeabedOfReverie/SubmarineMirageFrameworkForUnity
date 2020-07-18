//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestMultiEvent {
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.TestTools;
	using UniRx;
	using MultiEvent;
	using UTask;
	using Debug;
	using Test;


	public class TestEventModifyler : Test {
		MultiEvent _events = new MultiEvent();


		protected override void Create() {
			Application.targetFrameRate = 10;
			_disposables.AddLast( _events );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestChangeWhileRunning() => From( async () => {
			Log.Debug( "・生成テスト" );
			Log.Debug( _events );

			Log.Debug( "・追加テスト" );
			_events.AddLast( "b", () => Log.Debug( "b" ) );
			Log.Debug( _events );

			Log.Debug( "・追加中に変更を登録" );
			_events.AddLast( "start", () => {
				_events.AddLast( "c", () => Log.Debug( "c" ) );
				_events.AddFirst( "a", () => Log.Debug( "a" ) );
				_events.InsertLast( "b", "b.5", () => Log.Debug( "b.5" ) );
				_events.InsertFirst( "b", "a.5", () => Log.Debug( "a.5" ) );
				_events.Reverse();
				_events.Remove( "start" );
				Log.Debug( _events );
			} );
			Log.Debug( _events );

			Log.Debug( "・実行1" );
			_events.Run();
			Log.Debug( _events );

			Log.Debug( "・実行2" );
			_events.Run();
			Log.Debug( _events );

			await UTask.DontWait();
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() => From( async () => {
			_disposables.AddLast( TestMultiEventUtility.SetKey(
				_events,
				i => _events.AddLast( $"{i}", () => Log.Debug( $"{i}" ) )
			) );

			await UTask.Never( _asyncCanceler );
		} );
	}
}