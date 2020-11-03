//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {


	// TODO : コメント追加、整頓


	public enum SMTaskRunState {
		None,
		Create,
		SelfInitializing,
		SelfInitialized,
		Initializing,
		Initialized,
		FixedUpdate,
		Update,
		LateUpdate,
		Finalizing,
		Finalized,
	}
	public enum SMTaskActiveState {
		Disable,
		Enable,
	}
	public enum SMTaskType {
		DontWork,
		Work,
		FirstWork,
	}
	public enum SMTaskLifeSpan {
		InScene,
		Forever,
	}
}