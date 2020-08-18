//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestSMTask.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using UTask;
	using SMTask;
	using SMTask.Modifyler;
	using Extension;
	using Debug;
	using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;



	// TODO : コメント追加、整頓



	public static class TestSMObjectModifylerUtility {
	}



	public abstract class TestModifyData : SMObjectModifyData {
		public TestModifyData( SMObject smObject ) : base( smObject ) {}

		public override void Cancel() => Log.Debug( $"{nameof( Cancel )} : {this}" );

		public override async UniTask Run() {
			Log.Debug( $"start {nameof( Run )} : {this}" );
			await UTask.Delay( _object._asyncCanceler, 1000 );
			Log.Debug( $"end {nameof( Run )} : {this}" );
		}

		public void TestRegisterObject() => RegisterObject();
		public void TestAddObject( SMObject brother, SMObject add ) => AddObject( brother, add );
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