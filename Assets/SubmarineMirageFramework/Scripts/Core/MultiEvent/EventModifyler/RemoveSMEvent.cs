//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using Debug;


	// TODO : コメント追加、整頓


	public class RemoveSMEvent<T> : SMEventModifyData<T> {
		[SMShowLine] string _removeKey	{ get; set; }


		public RemoveSMEvent( string removeKey ) => _removeKey = removeKey;


		public override void Run() {
			_owner._events.RemoveAll( pair => {
				var isRemove = pair.Key == _removeKey;
				if ( isRemove )	{ _owner.OnRemove( pair.Value ); }
				return isRemove;
			} );
		}
	}
}