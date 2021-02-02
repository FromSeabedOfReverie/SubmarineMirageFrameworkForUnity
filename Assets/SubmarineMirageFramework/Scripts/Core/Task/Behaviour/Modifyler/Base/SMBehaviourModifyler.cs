//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler.Base {
	using Task.Base;
	using Utility;



	// TODO : コメント追加、整頓



	public class SMBehaviourModifyler
		: BaseSMTaskModifyler<SMBehaviourBody, SMBehaviourModifyler, SMBehaviourModifyData>
	{
		protected override SMAsyncCanceler _asyncCanceler => _owner._asyncCancelerOnDispose;


		public SMBehaviourModifyler( SMBehaviourBody owner ) : base( owner ) {}


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _toStringer.DefaultValue( _owner, i, true ) );
		}
	}
}