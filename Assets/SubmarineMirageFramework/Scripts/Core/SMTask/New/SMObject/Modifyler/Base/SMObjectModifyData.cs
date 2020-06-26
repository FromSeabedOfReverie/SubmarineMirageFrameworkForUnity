//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask.Modifyler {
	using UniRx.Async;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class SMObjectModifyData {
		public SMObject _object;


		public SMObjectModifyData( SMObject smObject ) {
			_object = smObject;
//			Debug.Log.Debug( $"{this.GetAboutName()}( {_object} )" );
		}

		public abstract UniTask Run();


		public override string ToString()
			=> $"{this.GetAboutName()}( {_object._behavior.GetAboutName()} )";
	}
}