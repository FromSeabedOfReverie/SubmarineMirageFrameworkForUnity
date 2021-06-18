//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Debug;


	public class UnregisterSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public UnregisterSMTask( SMTask task ) : base( task ) {}


		public override UniTask Run() => RunWhenNotUpdate( () => {
			_task.Unlink();
		} );
	}
}