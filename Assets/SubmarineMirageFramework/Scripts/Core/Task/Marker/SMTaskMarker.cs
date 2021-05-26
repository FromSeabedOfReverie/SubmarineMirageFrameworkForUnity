//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Marker {



	public class SMTaskMarker : SMTask {
		[SMShowLine] public override SMTaskRunType _type => _internalType;
		SMTaskRunType _internalType;
		[SMShowLine] public SMTaskMarkerType _markerType { get; private set; }
		[SMShowLine] public string _name { get; private set; }



		public SMTaskMarker( string name, SMTaskRunType type, SMTaskMarkerType markerType ) {
			_name = name;
			_internalType = type;
			_markerType = markerType;
		}
	}
}