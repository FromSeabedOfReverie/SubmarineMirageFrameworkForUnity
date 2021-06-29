//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Event.Modifyler {
	using SubmarineMirage.Modifyler;
	using Debug;



	public abstract class SMEventModifyData : SMModifyData {
		public new BaseSMEvent _target	{ get; private set; }
		[SMShowLine] public override SMModifyType _type => SMModifyType.Normal;



		public override void Set( object target, SMModifyler modifyler ) {
			base.Set( target, modifyler );
			_target = base._target as BaseSMEvent;
		}
	}
}