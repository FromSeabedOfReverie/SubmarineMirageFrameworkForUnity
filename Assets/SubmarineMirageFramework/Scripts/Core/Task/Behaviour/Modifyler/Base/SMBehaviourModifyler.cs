//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Task.Modifyler;



	// TODO : コメント追加、整頓



	public class SMBehaviourModifyler
		: BaseSMTaskModifyler<SMBehaviourBody, SMBehaviourModifyler, SMBehaviourModifyData>
	{
		protected override SMTaskCanceler _asyncCanceler => _owner._asyncCancelerOnDispose;


		public SMBehaviourModifyler( SMBehaviourBody owner ) : base( owner ) {}


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _owner._owner.ToLineString() );
		}
	}
}