//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System;
	using UniRx;
	using Service;
	using Utility;


	public static class UniRxSMExtension {
		static MainThreadDispatcherSMExtension s_mainThreadDispatcher	{ get; set; }


		public static void Add( this CompositeDisposable disposable, Action action )
			=> disposable.Add( Disposable.Create( action ) );


		public static Subject<Unit> EveryOnGUI() {
			if ( s_mainThreadDispatcher == null ) {
				s_mainThreadDispatcher = GameObjectSMUtility.Instantiate<MainThreadDispatcherSMExtension>();
				SMServiceLocator.Register( s_mainThreadDispatcher );
			}
			return s_mainThreadDispatcher._onGUIEvent;
		}
	}
}