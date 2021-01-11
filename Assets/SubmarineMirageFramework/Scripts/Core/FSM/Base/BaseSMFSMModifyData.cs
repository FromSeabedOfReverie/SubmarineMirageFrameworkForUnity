//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using Base;
	using Task;
	using Task.Group.Manager.Modifyler;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSMModifyData<TOwner, TModifyler>
		: SMLightBase, IBaseSMFSMModifyData<TOwner, TModifyler>
		where TOwner : IBaseSMFSMModifylerOwner<TModifyler>
		where TModifyler : IBaseSMFSMModifyler
	{
		protected TOwner _owner	{ get; private set; }
		protected TModifyler _modifyler	{ get; private set; }
		[SMShowLine] public abstract SMFSMModifyType _type	{ get; }
		bool _isCalledDestructor	{ get; set; }



		public BaseSMFSMModifyData() {}

		~BaseSMFSMModifyData() => _isCalledDestructor = true;

		public override void Dispose() {
			if ( !_isCalledDestructor )	{ Cancel(); }
		}

		protected virtual void Cancel() {}


		public virtual void Set( TOwner owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}


		public abstract UniTask Run();
	}
}