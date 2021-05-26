//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using UniRx;
	using Task;
	using Service;
	using Extension;
	using Utility;
	using Debug;


	public class TestB : SMBehaviour {
		public override void Create() {
			UTask.Void( async () => {
				var debugDisplay = SMServiceLocator.Resolve<DebugDisplay>();
				while ( true ) {
					debugDisplay.Add( $"{this.GetAboutName()} : { _body._ranState }" );
					await UTask.NextFrame( _asyncCancelerOnDispose );
				}
			} );


//			return;


			_selfInitializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {nameof( _selfInitializeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {nameof( _selfInitializeEvent )}" );
			} );
			_initializeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {nameof( _initializeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {nameof( _initializeEvent )}" );
			} );
			_finalizeEvent.AddLast( async canceler => {
				SMLog.Debug( $"start : {nameof( _finalizeEvent )}" );
				await UTask.Delay( canceler, 1000 );
				SMLog.Debug( $"end : {nameof( _finalizeEvent )}" );
			} );

			_enableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( nameof( _enableEvent ) );
			} );
			_disableEvent.AddLast().Subscribe( _ => {
				SMLog.Debug( nameof( _disableEvent ) );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
//				SMLog.Debug( nameof( _fixedUpdateEvent ) );
			} );
			_updateEvent.AddLast().Subscribe( _ => {
//				SMLog.Debug( nameof( _updateEvent ) );
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
//				SMLog.Debug( nameof( _lateUpdateEvent ) );
			} );
		}
	}
}