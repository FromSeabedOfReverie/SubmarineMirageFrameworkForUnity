//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene.Modifyler {
	using SubmarineMirage.Modifyler;
	using Task.Modifyler;



	public abstract class SMSceneManagerModifyData : SMTaskModifyData {
		public new SMSceneManagerBody _target { get; private set; }


		public override void Set( ISMModifyTarget target, SMModifyler modifyler ) {
			base.Set( target, modifyler );

			_target = ( SMSceneManagerBody )base._target;
		}
	}
}