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



	public class RegisterSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;


		public RegisterSMTask( SMTask task ) : base( task ) {}

		protected override void Cancel() {
			_task.Dispose();
		}


		public override UniTask Run() => RunWhenNotUpdate( () => {
			var last = _target.GetLast( _task._type, true )._previous;
			last.Link( _task );
		} );
	}
}