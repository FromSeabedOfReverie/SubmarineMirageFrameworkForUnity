//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System;
using SubmarineMirage.Base;
using SubmarineMirage.FSM;
using SubmarineMirage.Scene;



// TODO : コメント追加、整頓



public class SMSceneSetting : SMStandardBase, ISMSceneSetting {
	public SMFSMGenerateList<SMScene> _sceneFSMList	{ get; private set; }


	public SMSceneSetting() {
		_sceneFSMList = new SMFSMGenerateList<SMScene> {
			{
				new Type[] { typeof( ForeverSMScene ), },
				typeof( ForeverSMScene ),
				null,
				false
			}, {
				new Type[] {
					typeof( TitleSMScene ),
					typeof( FieldSMScene ),
					typeof( GameOverSMScene ),
					typeof( GameClearSMScene ),
					typeof( UnknownMainSMScene ),
				},
				typeof( MainSMScene ),
				null,
				false
			}, {
				new Type[] { typeof( UISMScene ), },
				typeof( UISMScene ),
				null,
				false
			}, {
				new Type[] { typeof( DebugSMScene ), },
				typeof( DebugSMScene ),
				null,
				false
			},
		};


		_disposables.AddLast( () => {
			_sceneFSMList.Dispose();
		} );
	}
}