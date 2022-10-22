//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Editor {
	using UnityEditor;



	[InitializeOnLoad]
	public class PlayerEditorWindowSMExtension : EditorWindowSMExtension {
		static PlayerEditorSMExtension s_body => PlayerEditorSMExtension.instance;



		static PlayerEditorWindowSMExtension() {
		}

		public override void Dispose() {}


		[MenuItem( "Edit/PlayExtension _F5", priority = 100000 )]
		static void PlayExtension() => s_body.PlayExtension();

		[MenuItem( "Edit/PauseExtension _F6", priority = 100001 )]
		static void PauseExtension() => s_body.PauseExtension();

		[MenuItem( "Edit/StepExtension _F7", priority = 100002 )]
		static void StepExtension() => s_body.StepExtension();
	}
}