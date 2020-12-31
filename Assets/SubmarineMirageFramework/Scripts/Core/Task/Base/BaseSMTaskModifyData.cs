//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTaskModifyler
namespace SubmarineMirage.Task.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMTaskModifyData<TOwner, TModifyler, TTarget, TLowerData, TLowerTarget>
		: SMLightBase, IBaseSMTaskModifyData<TOwner, TModifyler, TTarget>
		where TOwner : IBaseSMTaskModifylerOwner<TModifyler>
		where TModifyler : IBaseSMTaskModifyler
		where TTarget : IBaseSMTaskModifyDataTarget
	{
		protected TOwner _owner	{ get; private set; }
		protected TModifyler _modifyler	{ get; private set; }
		[SMShowLine] public TTarget _target	{ get; protected set; }
		[SMShowLine] public abstract SMTaskModifyType _type	{ get; }
		bool _isCalledDestructor	{ get; set; }

		protected readonly SMTaskRunAllType[] _sequentialRunOrder = new SMTaskRunAllType[] {
			SMTaskRunAllType.Sequential, SMTaskRunAllType.Parallel
		};
		protected readonly SMTaskRunAllType[] _reverseSequentialRunOrder = new SMTaskRunAllType[] {
			SMTaskRunAllType.Parallel, SMTaskRunAllType.ReverseSequential
		};



		public BaseSMTaskModifyData( TTarget target ) {
			_target = target;
#if TestTaskModifyler
			SMLog.Debug( $"{this.GetAboutName()}() : {this}" );
#endif
		}

		~BaseSMTaskModifyData() => _isCalledDestructor = true;

		public override void Dispose() {
			if ( !_isCalledDestructor )	{ Cancel(); }
		}

		protected virtual void Cancel() {}


		public virtual void Set( TOwner owner ) {
			_owner = owner;
			_modifyler = _owner._modifyler;
		}


		public abstract UniTask Run();

		protected async UniTask RunLower( SMTaskRunAllType type, Func<TLowerTarget, TLowerData> createEvent ) {
			var ls = GetLowers( type );
			switch ( type ) {
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:
					foreach ( var l in ls ) {
						await RegisterAndRunLower( l, createEvent( l ) );
					}
					return;
				case SMTaskRunAllType.Parallel:
					await ls.Select( l =>
						RegisterAndRunLower( l, createEvent( l ) )
					);
					return;
			}
		}

		protected abstract UniTask RegisterAndRunLower( TLowerTarget lowerTarget, TLowerData data );


		IEnumerable<TLowerTarget> GetLowers( SMTaskRunAllType type ) {
			var t = ToTaskType( type );
			var ls = GetAllLowers()
				.Where( l => IsTargetLower( l, t ) );
			switch ( type ) {
				case SMTaskRunAllType.ReverseSequential:	return ls.Reverse();
				default:									return ls;
			}
		}

		SMTaskType ToTaskType( SMTaskRunAllType type ) {
			switch ( type ) {
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:	return SMTaskType.FirstWork;
				case SMTaskRunAllType.Parallel:				return SMTaskType.Work;
				default:									return SMTaskType.DontWork;
			}
		}

		protected abstract IEnumerable<TLowerTarget> GetAllLowers();

		protected abstract bool IsTargetLower( TLowerTarget lowerTarget, SMTaskType type );
	}
}