//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System.Collections.Generic;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class EventModifyData<T> {
		public BaseMultiEvent<T> _owner;
		public T _function;

		public abstract void Run();

		protected void NoEventError( string key ) {
			throw new KeyNotFoundException( $"イベント関数が未登録 : {key}" );
		}

		public override string ToString() => $"{this.GetAboutName()}()";
	}
}