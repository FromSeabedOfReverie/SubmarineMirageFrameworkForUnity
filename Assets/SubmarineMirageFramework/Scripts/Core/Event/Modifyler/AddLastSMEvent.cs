//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;



	public class AddLastSMEvent : SMEventModifyData {
		[SMShowLine] BaseSMEventData _data { get; set; }



		public AddLastSMEvent( BaseSMEventData data ) {
			_data = data;
		}

		protected override void Cancel() {
			base.Cancel();
			_data.Dispose();
		}



		public override async UniTask Run() {
			_target._events.AddLast( _data );

			await UTask.DontWait();
		}
	}
}