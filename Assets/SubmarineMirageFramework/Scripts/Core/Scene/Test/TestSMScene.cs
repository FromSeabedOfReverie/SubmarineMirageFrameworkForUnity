//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Test {
	using UniRx;
	using Base;
	using Extension;
	using Utility;
	using Debug;



	public class TestSMScene : SMStandardBase {
		SMScene _scene;


		public TestSMScene( SMScene scene ) {
			_scene = scene;
		}


		public void SetEvent() {
			_scene._selfInitializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetAboutName()}.{nameof( _scene._selfInitializeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetAboutName()}.{nameof( _scene._selfInitializeEvent )}" );
			} );
			_scene._initializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetAboutName()}.{nameof( _scene._initializeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetAboutName()}.{nameof( _scene._initializeEvent )}" );
			} );
			_scene._finalizeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetAboutName()}.{nameof( _scene._finalizeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetAboutName()}.{nameof( _scene._finalizeEvent )}" );
			} );

			_scene._enableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetAboutName()}.{nameof( _scene._enableEvent )}" );
			} );
			_scene._disableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetAboutName()}.{nameof( _scene._disableEvent )}" );
			} );

			_scene._fixedUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetAboutName()}.{nameof( _scene._fixedUpdateEvent )}" );
			} );
			_scene._updateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetAboutName()}.{nameof( _scene._updateEvent )}" );
			} );
			_scene._lateUpdateEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( $"{_scene.GetAboutName()}.{nameof( _scene._lateUpdateEvent )}" );
			} );

			_scene._enterEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetAboutName()}.{nameof( _scene._enterEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetAboutName()}.{nameof( _scene._enterEvent )}" );
			} );
			_scene._updateAsyncEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetAboutName()}.{nameof( _scene._updateAsyncEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetAboutName()}.{nameof( _scene._updateAsyncEvent )}" );
			} );
			_scene._exitEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetAboutName()}.{nameof( _scene._exitEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetAboutName()}.{nameof( _scene._exitEvent )}" );
			} );

			_scene._createBehavioursEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {_scene.GetAboutName()}.{nameof( _scene._createBehavioursEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {_scene.GetAboutName()}.{nameof( _scene._createBehavioursEvent )}" );
			} );
		}
	}
}