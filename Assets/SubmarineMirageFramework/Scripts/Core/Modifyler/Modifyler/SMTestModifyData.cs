//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Modifyler.Test {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;



	public class SMTestModifyData : SMModifyData {
		[SMShowLine] public override SMModifyType _type => _internalType;
		SMModifyType _internalType;
		[SMShowLine] public string _name { get; private set; }


		public SMTestModifyData( string name, SMModifyType type ) {
			_name = name;
			_internalType = type;
		}

		protected override void Cancel() {
			SMLog.Debug( $"{nameof( Cancel )} : {this}" );
		}


		public override UniTask Run()
			=> UTask.DontWait();
	}
}