//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task;
	using Task.Modifyler;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestSMObjectModifylerSMUtility {
	}



	public abstract class TestModifyData : SMObjectModifyData {
		public TestModifyData( SMObject smObject ) : base( smObject ) {}

		public override void Cancel() => SMLog.Debug( $"{nameof( Cancel )} : {this}" );

		public override async UniTask Run() {
			SMLog.Debug( $"start {nameof( Run )} : {this}" );
			await UTask.Delay( _object._asyncCanceler, 1000 );
			SMLog.Debug( $"end {nameof( Run )} : {this}" );
		}

		public void TestRegisterObject() => RegisterObject();
		public void TestAddObject( SMObject brother, SMObject add ) => AddObject( brother, add );
	}

	public class InterruptData : TestModifyData {
		public override ModifyType _type => ModifyType.Interrupter;
		public InterruptData( SMObject smObject ) : base( smObject ) {}
	}

	public class LinkData : TestModifyData {
		public override ModifyType _type => ModifyType.Linker;
		public LinkData( SMObject smObject ) : base( smObject ) {}
	}

	public class RunData : TestModifyData {
		public override ModifyType _type => ModifyType.Runner;
		public RunData( SMObject smObject ) : base( smObject ) {}
	}
}