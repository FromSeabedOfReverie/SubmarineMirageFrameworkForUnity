//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using KoganeUnityLib;
	using Extension;



	public class SMEvent<T> : BaseSMEvent< Action<T> > {
		public override void OnRemove( Action<T> function ) {}


		public void Run( T t ) {
			CheckDisposeError();

			var temp = _events.Copy();
			temp.ForEach( pair => pair.Value.Invoke( t ) );
		}
	}
}