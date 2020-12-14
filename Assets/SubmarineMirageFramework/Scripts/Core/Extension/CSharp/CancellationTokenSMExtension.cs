//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System.Threading;


	// TODO : コメント追加、整頓


	public static class CancellationTokenSMExtension {
		public static CancellationTokenSource Link( this CancellationToken self, CancellationToken add ) {
			return CancellationTokenSource.CreateLinkedTokenSource( self, add );
		}
	}
}