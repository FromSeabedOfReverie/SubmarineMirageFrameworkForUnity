//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using Extension;


	// TODO : コメント追加、整頓


	public class RemoveEventModifyData<T> : EventModifyData<T> {
		string _removeKey;


		public RemoveEventModifyData( string removeKey ) => _removeKey = removeKey;


		public override void Run() {
			_owner._events.RemoveAll( pair => {
				var isRemove = pair.Key == _removeKey;
				if ( isRemove )	{ _owner.OnRemove( pair.Value ); }
				return isRemove;
			} );
		}


		public override string ToString() => $"{this.GetAboutName()}( {_removeKey} )";
	}
}