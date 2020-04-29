//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System;


	// TODO : コメント追加、整頓


	public class MultiEvent : BaseMultiEvent<Action> {
		public override void OnRemove( Action function ) {}


		public void Run() {
			CheckDisposeError();
			_isLock = true;
			_events.ForEach( pair => pair.Value.Invoke() );
			_isLock = false;
		}
	}
}