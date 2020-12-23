//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object.Modifyler {
	using Cysharp.Threading.Tasks;
	using Object;
	using Debug;



	// TODO : コメント追加、整頓



	public class InitialEnableSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public InitialEnableSMObject( SMObject target ) : base( target ) {}

		protected override void Cancel() {}


		public override async UniTask Run() {
		}
	}
}