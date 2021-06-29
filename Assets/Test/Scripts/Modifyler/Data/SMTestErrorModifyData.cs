//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestModifyler {
	using System;
	using Cysharp.Threading.Tasks;
	using Modifyler;
	using Utility;
	using Debug;



	public class SMTestErrorModifyData : SMTestModifyData {
		public SMTestErrorModifyData( string name, SMModifyType type ) : base( name, type ) {
		}



		public override async UniTask Run() {
			await UTask.Delay( _modifyler._asyncCanceler, 500 );
			throw new Exception( $"試験失敗 : \n{this}" );
		}
	}
}