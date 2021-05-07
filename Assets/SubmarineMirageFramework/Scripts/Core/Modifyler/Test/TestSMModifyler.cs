//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler.Test {
	using Cysharp.Threading.Tasks;
	using Base;
	using Utility;



	public class TestTarget : SMStandardBase, ISMModifyTarget {
	}


	public abstract class BaseTestData : SMModifyData {
	}

	public class TestData : BaseTestData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Runner;
		public override UniTask Run() => UTask.DontWait();
	}
}