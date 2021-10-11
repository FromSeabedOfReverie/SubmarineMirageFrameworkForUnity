//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using UniRx;



	public static class DisposableSMExtension {
		public static void AddFirst( this IDisposable self, SMDisposable disposable )
			=> disposable.AddFirst( self );

		public static void AddLast( this IDisposable self, SMDisposable disposable )
			=> disposable.AddLast( self );

		public static void AddLast( this IDisposable self, CompositeDisposable disposable )
			=> disposable.AddLast( self );



		public static void AddFirst( this IDisposable self, ISMStandardBase @base )
			=> AddFirst( self, @base._disposables );

		public static void AddLast( this IDisposable self, ISMStandardBase @base )
			=> AddLast( self, @base._disposables );



		public static void AddLast( this IDisposable self, ISMRawBase @base )
			=> AddLast( self, @base._disposables );
	}
}