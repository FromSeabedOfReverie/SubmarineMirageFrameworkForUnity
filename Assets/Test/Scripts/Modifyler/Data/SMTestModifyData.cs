//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestModifyler {
	using Cysharp.Threading.Tasks;
	using Modifyler;
	using Extension;
	using Utility;
	using Debug;



	public class SMTestModifyData : SMModifyData {
		[SMShowLine] public override SMModifyType _type => _internalType;
		SMModifyType _internalType;
		[SMShowLine] public string _name { get; private set; }



		public SMTestModifyData( string name, SMModifyType type ) {
			_name = name;
			_internalType = type;
			SMLog.Debug( $"{this.GetAboutName()} : \n{this}" );
		}



		public override async UniTask Run() {
			SMLog.Warning( $"{nameof( Run )} : start\n{this}" );
			await UTask.Delay( _modifyler._asyncCanceler, 1000 );
			SMLog.Warning( $"{nameof( Run )} : end\n{this}" );
		}
	}
}