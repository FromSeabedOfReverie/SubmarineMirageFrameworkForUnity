//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using System;
	using KoganeUnityLib;
	using Event.Base;
	using Extension;



	// TODO : コメント追加、整頓



	public class SMEvent : BaseSMEvent<Action> {
		public override void OnRemove( Action function ) {}


		public void Run() {
			CheckDisposeError();

			var temp = _events.Copy();
			temp.ForEach( pair => pair.Value.Invoke() );
		}
	}
}