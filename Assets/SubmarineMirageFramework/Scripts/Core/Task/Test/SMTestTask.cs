//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Test {



	public class SMTestTask : SMTask {
		[SMShowLine] public override SMTaskRunType _type => _internalType;
		SMTaskRunType _internalType;
		[SMShowLine] public string _name { get; private set; }



		public SMTestTask( string name, SMTaskRunType type, bool isAdjustRun ) : base( false, isAdjustRun ) {
			_name = name;
			_internalType = type;
			_taskManager.Register( this, isAdjustRun );
		}

		public override void Create() {
		}
	}
}