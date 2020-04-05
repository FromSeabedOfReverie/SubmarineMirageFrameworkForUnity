//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {


	// TODO : コメント追加、整頓


	public class ReverseEventModifyData<T> : EventModifyData<T> {

		public ReverseEventModifyData() {}

		public override void Run() => _owner._events.Reverse();
	}
}