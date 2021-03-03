//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler.Test {
	using Cysharp.Threading.Tasks;
	using Utility;



	// TODO : コメント追加、整頓



	public class TestTarget : SMModifyTarget<TestTarget, BaseTestData> {
	}


	public abstract class BaseTestData : SMModifyData<TestTarget, BaseTestData> {
	}

	public class TestData : BaseTestData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		public override UniTask Run() => UTask.DontWait();
	}
}