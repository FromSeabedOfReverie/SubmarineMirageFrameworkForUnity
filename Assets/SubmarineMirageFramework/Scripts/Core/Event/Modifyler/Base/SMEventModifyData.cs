//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event.Modifyler {
	using System.Collections.Generic;
	using Base;
	using Debug;


	public abstract class SMEventModifyData<T> : SMLightBase {
		public BaseSMEvent<T> _owner	{ get; set; }
		[SMShowLine] public T _function	{ get; set; }


		public override void Dispose() {}


		public abstract void Run();


		protected void NoEventError( string key )
			=> throw new KeyNotFoundException( $"イベント関数が未登録 : {key}" );
	}
}