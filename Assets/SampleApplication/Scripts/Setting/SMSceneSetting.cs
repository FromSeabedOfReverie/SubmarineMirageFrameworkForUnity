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



public class SMSceneSetting : SMStandardBase, ISMSceneSetting {
	public SMFSMGenerateList _sceneFSMList	{ get; private set; }


	public SMSceneSetting() {
		_sceneFSMList = new SMFSMGenerateList {
			{
				new Type[] { typeof( ForeverSMScene ), },
				typeof( ForeverSMScene ),
				null,
				false
			}, {
				new Type[] {
					typeof( TitleSMScene ),
					typeof( GameSMScene ),
//					typeof( FieldSMScene ),
					typeof( GameOverSMScene ),
					typeof( GameClearSMScene ),
					typeof( UnknownSMScene ),
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