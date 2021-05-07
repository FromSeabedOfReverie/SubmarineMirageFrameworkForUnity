//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Test {
	using UnityEngine;
	using UniRx;
	using Base;
	using Service;
	using Debug;



	public class TestOpenWorldCenter : SMStandardBase {
		OpenWorldCenter _owner	{ get; set; }


		public TestOpenWorldCenter( OpenWorldCenter owner, SMScene scene ) {
			_owner = owner;


			var fps = 0;
			var count = 0;
			var nextSecond = 0f;
			DebugDisplay debugDisplay = null;
			Application.targetFrameRate = 60;
			scene._owner._updateEvent.AddLast().Subscribe( _ => {
				if ( nextSecond > 1 ) {
					nextSecond = 0;
					fps = count;
					count = 0;
				}
				count++;
				nextSecond += Time.deltaTime;
				if ( debugDisplay == null ) {
					debugDisplay = SMServiceLocator.Resolve<DebugDisplay>();
				}
				debugDisplay.Add( $"FPS : {fps}" );
			} );

			
			var angles = Vector3.zero;
			scene._owner._updateEvent.AddLast().Subscribe( _ => {
				var input = new Vector3(
					Input.GetAxis( "MoveAxisX" ),
					Input.GetAxis( "DebugAxisY" ),
					Input.GetAxis( "MoveAxisY" )
				);
				if ( input.magnitude > 1 )	{ input.Normalize(); }

				_owner.transform.rotation = Quaternion.Euler(
					0,
					angles.y,
					0
				);
				_owner.transform.position += _owner.transform.rotation * input * 100 * Time.deltaTime;
			} );


			var distance = 30f;
			scene._owner._updateEvent.AddLast().Subscribe( _ => {
				var input = -Input.GetAxis( "Scroll" );
				distance += input * 10000 * Time.deltaTime;
				distance = Mathf.Clamp( distance, 0, 3000 );

				var scale = Mathf.Max( distance / 50, 1 );
				_owner.transform.localScale = Vector3.one * scale;
			} );


			var camera = Camera.main.transform;
			scene._owner._updateEvent.AddLast().Subscribe( _ => {
				var input = new Vector3(
					-Input.GetAxis( "RotateAxisY" ),
					Input.GetAxis( "RotateAxisX" ),
					0
				);
				if ( input.magnitude > 1 )	{ input.Normalize(); }

				angles += input * 100 * Time.deltaTime;
				angles.x = Mathf.Clamp( angles.x, -90, 90 );
				angles.y = Mathf.Repeat( angles.y, 360 );

				camera.rotation = Quaternion.Slerp(
					camera.rotation,
					Quaternion.Euler( angles ),
					100
				);
				camera.position = _owner.transform.position + -camera.forward * distance;
			} );
		}
	}
}