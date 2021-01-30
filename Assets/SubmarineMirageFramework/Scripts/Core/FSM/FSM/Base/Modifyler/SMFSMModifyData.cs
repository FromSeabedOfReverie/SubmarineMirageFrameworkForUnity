//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler.Base {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using FSM.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMFSMModifyData : SMLightBase {
		[SMHide] protected SMFSMModifyler _modifyler	{ get; private set; }
		[SMShowLine] public abstract SMFSMModifyType _type	{ get; }


		public override void Dispose() {}


		public virtual void Set( SMFSM owner ) {
			_modifyler = owner._modifyler;
		}


		public abstract UniTask Run();
	}
}