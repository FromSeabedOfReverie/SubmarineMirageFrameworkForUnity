//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Base {
	using System;


	// TODO : コメント追加、整頓


	public interface ISMBase : IDisposable {
		uint _id		{ get; set; }

		string ToString( int indent );
		string ToLineString( int indent = 0 );
	}
}