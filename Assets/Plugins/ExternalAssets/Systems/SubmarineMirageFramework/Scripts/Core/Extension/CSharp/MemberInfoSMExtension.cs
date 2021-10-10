//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.Reflection;


	public static class MemberInfoSMExtension {
		public static object GetValue( this MemberInfo self, object instance ) {
			switch ( self ) {
				case FieldInfo f:		return f.GetValue( instance );
				case PropertyInfo p:	return p.GetValue( instance );
				default:				return null;
			}
		}
	}
}