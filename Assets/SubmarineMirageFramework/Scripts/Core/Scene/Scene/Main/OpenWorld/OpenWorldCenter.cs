//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using Task;
	using Debug;



	public class OpenWorldCenter : SMBehaviour {
		public override void Create() {
			var scene = ( OpenWorldSMScene )_object._body._groupBody._managerBody._scene;
			scene.Setup( this );

			var test = new Test.TestOpenWorldCenter( this );
			_disposables.AddLast( test );
		}
	}
}