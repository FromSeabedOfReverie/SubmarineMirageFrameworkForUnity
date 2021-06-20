//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event.Modifyler {
	using Cysharp.Threading.Tasks;
	using Extension;
	using Utility;
	using Debug;



	public class ReverseSMEvent : SMEventModifyData {



		public override async UniTask Run() {
			_target._events = _target._events.Reverse();

			await UTask.DontWait();
		}
	}
}