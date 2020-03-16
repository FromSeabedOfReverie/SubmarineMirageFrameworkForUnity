//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using System.Threading;


	// TODO : コメント追加、整頓


	public static class CancellationTokenExtension {
		public static CancellationTokenSource Add( this CancellationToken a, CancellationToken b ) {
			return CancellationTokenSource.CreateLinkedTokenSource( a, b );
		}
	}
}