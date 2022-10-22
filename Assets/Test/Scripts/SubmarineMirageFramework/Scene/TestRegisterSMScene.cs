//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Test {
	using UniRx;



	public class TestRegisterSMScene : SMStandardBase {
		SMScene _scene;


		public TestRegisterSMScene( SMScene scene ) {
			_scene = scene;
		}


		public void SetEvent() {
			_scene._enterEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetName()}.{nameof( _scene._enterEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetName()}.{nameof( _scene._enterEvent )}" );
			} );
			_scene._asyncUpdateEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetName()}.{nameof( _scene._asyncUpdateEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetName()}.{nameof( _scene._asyncUpdateEvent )}" );
			} );
			_scene._exitEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetName()}.{nameof( _scene._exitEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetName()}.{nameof( _scene._exitEvent )}" );
			} );

			_scene._createBehavioursEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetName()}.{nameof( _scene._createBehavioursEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetName()}.{nameof( _scene._createBehavioursEvent )}" );
			} );

			return;

			_scene._fixedUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetName()}.{nameof( _scene._fixedUpdateEvent )}" );
			} );
			_scene._updateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetName()}.{nameof( _scene._updateEvent )}" );
			} );
			_scene._lateUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetName()}.{nameof( _scene._lateUpdateEvent )}" );
			} );
		}
	}
}