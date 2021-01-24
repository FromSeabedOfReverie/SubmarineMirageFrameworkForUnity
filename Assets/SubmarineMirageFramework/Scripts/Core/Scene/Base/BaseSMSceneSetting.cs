//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Collections.Generic;
	using Base;
	using Service;



	// TODO : コメント追加、整頓



	public abstract class BaseSMSceneSetting : SMStandardBase, ISMService {
		public readonly Dictionary<SMSceneType, SMSceneInternalFSM> _scenes
			= new Dictionary<SMSceneType, SMSceneInternalFSM>();


		public BaseSMSceneSetting() {
			_scenes = new Dictionary<SMSceneType, SMSceneInternalFSM> {
				{
					SMSceneType.Forever,
					new SMSceneInternalFSM(
						new SMScene[] { new ForeverSMScene() },
						typeof( ForeverSMScene )
					)
				}, {
					SMSceneType.UI,
					new SMSceneInternalFSM(
						new SMScene[] { new UISMScene(), },
						typeof( UISMScene )
					)
				}, {
					SMSceneType.Debug,
					new SMSceneInternalFSM(
						new SMScene[] { new DebugSMScene(), },
						typeof( DebugSMScene )
					)
				},
			};

			_disposables.AddLast( () => {
				_scenes.Clear();
			} );
		}
	}
}