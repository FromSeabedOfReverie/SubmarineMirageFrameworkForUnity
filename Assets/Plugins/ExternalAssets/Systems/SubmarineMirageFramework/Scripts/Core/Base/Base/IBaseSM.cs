//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;



	public interface IBaseSM : IDisposable {
		uint _id	{ get; }

		string ToString( int indent, bool isUseHeadIndent = true );
		string ToLineString( int indent = 0 );
	}
}