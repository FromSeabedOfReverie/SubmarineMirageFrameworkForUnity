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


	public static class UniRxSMExtension {
		public static void Add( this CompositeDisposable disposable, Action action )
			=> disposable.Add( Disposable.Create( action ) );


		public static Subject<Unit> EveryOnGUI() {
			var mtd = SMServiceLocator.Resolve<MainThreadDispatcherSMExtension>();
			return mtd._onGUIEvent;
		}
	}
}