//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using SubmarineMirage.Scene;



// TODO : コメント追加、整頓



public class SMSceneSetting : BaseSMSceneSetting {
	public SMSceneSetting() {
		_scenes[SMSceneType.Main] = new SMSceneInternalFSM(
			new SMScene[] {
				new TitleSMScene(),
				new FieldSMScene(),
				new GameOverSMScene(),
				new GameClearSMScene(),
			},
			typeof( MainSMScene )
		);


		var chunkScenes = new SMScene[] {
			new FieldChunk1SMScene(),
			new FieldChunk2SMScene(),
			new FieldChunk3SMScene(),
			new FieldChunk4SMScene(),
		};
		_scenes[SMSceneType.FieldChunk1] = new SMSceneInternalFSM(
			chunkScenes,
			typeof( FieldChunkSMScene ),
			false
		);
		_scenes[SMSceneType.FieldChunk2] = new SMSceneInternalFSM(
			chunkScenes,
			typeof( FieldChunkSMScene ),
			false
		);
		_scenes[SMSceneType.FieldChunk3] = new SMSceneInternalFSM(
			chunkScenes,
			typeof( FieldChunkSMScene ),
			false
		);
		_scenes[SMSceneType.FieldChunk4] = new SMSceneInternalFSM(
			chunkScenes,
			typeof( FieldChunkSMScene ),
			false
		);
	}
}