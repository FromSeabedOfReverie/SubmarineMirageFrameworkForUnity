//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event {
	using Cysharp.Threading.Tasks;
	using Base;
	using Utility;
	using Debug;



	public abstract class BaseSMEventData : SMLightBase {
		protected BaseSMEvent _owner { get; private set; }
		[SMShowLine] public string _key	{ get; private set; }



		public BaseSMEventData( string key ) {
			_key = key;
		}

		public virtual void Set( BaseSMEvent owner ) {
			_owner = owner;
		}



		public abstract UniTask Run( SMAsyncCanceler canceler );
	}
}