//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using Cysharp.Threading.Tasks;



	public abstract class BaseSMEventData : SMLightBase {
		[SMShowLine] public string _key	{ get; private set; }



		public BaseSMEventData( string key ) {
			_key = key;
		}



		public abstract UniTask Run( SMAsyncCanceler canceler );
	}
}