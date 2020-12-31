//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class CreateSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public CreateSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
		}
	}
}