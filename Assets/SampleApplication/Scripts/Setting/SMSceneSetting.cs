//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System;
using SubmarineMirage.FSM;
using SubmarineMirage.Scene;



public class SMSceneSetting : BaseSMSceneSetting {
	public override void Setup() {
		_datas = new SMFSMGenerateList {
			{
				new Type[] { typeof( ForeverSMScene ), },
				typeof( ForeverSMScene )
			}, {
				new Type[] {
					typeof( UnknownSMScene ),
					typeof( TitleSMScene ),
					typeof( GameSMScene ),
					typeof( GameOverSMScene ),
					typeof( GameClearSMScene ),
				},
				typeof( MainSMScene )
			}, {
				new Type[] { typeof( UISMScene ), },
				typeof( UISMScene )
			}, {
				new Type[] { typeof( DebugSMScene ), },
				typeof( DebugSMScene )
			},
		};
	}
}