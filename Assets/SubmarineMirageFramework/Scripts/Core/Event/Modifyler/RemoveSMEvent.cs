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



	public class RemoveSMEvent : SMEventModifyData {
		[SMShowLine] string _findKey	{ get; set; }



		public RemoveSMEvent( string findKey )
			=> _findKey = findKey;



		public override async UniTask Run() {
			_target._events.RemoveAll(
				data => data._key == _findKey,
				data => data.Dispose()
			);

			await UTask.DontWait();
		}
	}
}