//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestModifyler {
	using Cysharp.Threading.Tasks;
	using Modifyler;
	using Utility;
	using Debug;



	public class SMTestDisposeModifyData : SMTestModifyData {
		public SMTestDisposeModifyData( string name, SMModifyType type ) : base( name, type ) {
		}



		public override async UniTask Run() {
			await UTask.Delay( _modifyler._asyncCanceler, 500 );
			SMLog.Warning( $"{nameof( Run )} : 解放\n{this}" );
			_modifyler.Dispose();
		}
	}
}