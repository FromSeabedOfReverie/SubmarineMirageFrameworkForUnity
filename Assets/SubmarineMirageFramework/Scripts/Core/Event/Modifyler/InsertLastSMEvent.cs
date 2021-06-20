//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event.Modifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Extension;
	using Utility;
	using Debug;



	public class InsertLastSMEvent : SMEventModifyData {
		[SMShowLine] string _findKey		{ get; set; }
		[SMShowLine] BaseSMEventData _data	{ get; set; }



		public InsertLastSMEvent( string findKey, BaseSMEventData data ) {
			_findKey = findKey;
			_data = data;
		}

		protected override void Cancel() {
			base.Cancel();
			_data.Dispose();
		}



		public override async UniTask Run() {
			_target._events.AddAfter(
				_data,
				d => d._key == _findKey,
				() => throw new NotSupportedException( string.Join( "\n",
					$"{this.GetAboutName()}.{nameof( Run )} : 未登録 : {_findKey}",
					$"{_target._events}"
				) )
			);

			await UTask.DontWait();
		}
	}
}