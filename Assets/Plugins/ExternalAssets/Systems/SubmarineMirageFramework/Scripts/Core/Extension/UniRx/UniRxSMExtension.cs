//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using UniRx;



	public static class UniRxSMExtension {
		static MainThreadDispatcherSMExtension s_mainThreadDispatcher	{ get; set; }



		public static void AddLast( this CompositeDisposable self, IDisposable disposable )
			=> self.Add( disposable );

		public static void AddLast( this CompositeDisposable self, Action @event )
			=> AddLast( self, Disposable.Create( @event ) );



		public static Subject<Unit> EveryOnGUI() {
			if ( s_mainThreadDispatcher == null ) {
				s_mainThreadDispatcher = GameObjectSMUtility.Instantiate<MainThreadDispatcherSMExtension>();
				SMServiceLocator.Register( s_mainThreadDispatcher );
			}
			return s_mainThreadDispatcher._onGUIEvent;
		}
	}
}