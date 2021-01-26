//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Scene;



// TODO : コメント追加、整頓



public class SMSceneSetting : SMStandardBase, ISMSceneSetting {
	public Dictionary<SMSceneType, Type[]> _fsmSceneTypes	{ get; private set; }
	public Type[] _chunkSceneTypes	{ get; private set; }


	public SMSceneSetting() {
		_fsmSceneTypes = new Dictionary<SMSceneType, Type[]> {
			{
				SMSceneType.Forever,
				new Type[] { typeof( ForeverSampleSMScene ), }
			}, {
				SMSceneType.UI,
				new Type[] { typeof( UISMScene ), }
			}, {
				SMSceneType.Debug,
				new Type[] { typeof( DebugSMScene ), }
			}, {
				SMSceneType.Main,
				new Type[] {
					typeof( TitleSMScene ),
					typeof( FieldSMScene ),
					typeof( GameOverSMScene ),
					typeof( GameClearSMScene ),
					typeof( TestSMScene ),
				}
			}
		};
		_chunkSceneTypes = new Type[] {
			typeof( FieldChunk1SMScene ),
			typeof( FieldChunk2SMScene ),
			typeof( FieldChunk3SMScene ),
			typeof( FieldChunk4SMScene ),
		};

		_disposables.AddLast( () => {
			_fsmSceneTypes.Clear();
			_chunkSceneTypes.Clear();
		} );
	}
}