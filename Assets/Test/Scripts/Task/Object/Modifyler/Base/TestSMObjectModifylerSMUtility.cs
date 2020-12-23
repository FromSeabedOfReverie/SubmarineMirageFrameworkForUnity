//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task;
	using Task.Object;
	using Task.Object.Modifyler;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestSMObjectModifylerSMUtility {
	}



	public abstract class TestModifyData : SMObjectModifyData {
		public TestModifyData( SMObject smObject ) : base( smObject ) {}

		protected override void Cancel() => SMLog.Debug( $"{nameof( Cancel )} : {this}" );

		public override async UniTask Run() {
			SMLog.Debug( $"start {nameof( Run )} : {this}" );
			await UTask.Delay( _target._asyncCanceler, 1000 );
			SMLog.Debug( $"end {nameof( Run )} : {this}" );
		}

		public void TestRegisterObject() => RegisterObject();
		public void TestAddObject( SMObject brother, SMObject add ) => AddObject( brother, add );
	}

	public class InterruptData : TestModifyData {
		public InterruptData( SMObject smObject ) : base( smObject )
			=> _type = SMTaskModifyType.FirstLinker;
	}

	public class LinkData : TestModifyData {
		public LinkData( SMObject smObject ) : base( smObject )
			=> _type = SMTaskModifyType.Linker;
	}

	public class RunData : TestModifyData {
		public RunData( SMObject smObject ) : base( smObject )
			=> _type = SMTaskModifyType.Runner;
	}
}