//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {


	// TODO : コメント追加、整頓


	public enum SMTaskRanState {
		None,
		Creating,
		Created,
		Loading,
		Loaded,
		Initializing,
		Initialized,
		FixedUpdate,
		Update,
		LateUpdate,
		Finalizing,
		Finalized,
	}
	public enum SMTaskActiveState {
		Disabled,
		Disabling,
		Enabled,
		Enabling,
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