//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System.Threading;



	public static class CancellationTokenSMExtension {
		public static CancellationTokenSource Link( this CancellationToken self, CancellationToken add ) {
			return CancellationTokenSource.CreateLinkedTokenSource( self, add );
		}
	}
}