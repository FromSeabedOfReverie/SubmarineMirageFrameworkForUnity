//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test.Process {
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using SubmarineMirageFramework.Process;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ コルーチン処理の試験クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TestCoroutineProcess : MonoBehaviourProcess {


		static int s_maxID;
		int _id;


		protected override void Constructor() {

			Application.targetFrameRate = 60;
			var coroutineProcess = new CoroutineProcess( Hoge( _id ), () => Log.Debug( "finish" ), false, true );
//			var processes = new List<CoroutineProcess>();


			InputManager.s_instance.GetPressedEvent( InputManager.Event.Decide ).Subscribe( _ => {
				coroutineProcess.Play();
/*
				CoroutineProcess c;
				if ( processes.Count > 0 ) {
					c = processes.LastOrDefault();
				} else {
					var id = _id = s_maxID++;
					c = new CoroutineProcess( Hoge( id ), () => Log.Debug( "finish" ), false, true );
					processes.Add( c );
				}
				c.Play();
*/
			} );

			InputManager.s_instance.GetPressedEvent( InputManager.Event.Quit ).Subscribe( _ => {
				coroutineProcess.Pause();
/*
				var c = processes.LastOrDefault();
				if ( c != null ) {
					c.Pause();
				}
//				processes.Remove( c );
*/
			} );

			base.Constructor();
		}


		IEnumerator Hoge( int id ) {
			Log.Debug( $"coroutine start : {id}" );
			yield return new WaitForSeconds( 5 );
			Log.Debug( $"coroutine waiting : {id}" );
			yield return new WaitForSeconds( 5 );
			Log.Debug( $"coroutine end : {id}" );
		}
	}
}