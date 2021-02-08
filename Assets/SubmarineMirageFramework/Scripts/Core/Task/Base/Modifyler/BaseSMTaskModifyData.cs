//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTaskModifyler
namespace SubmarineMirage.Task.Modifyler.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMTaskModifyData<TOwner, TModifyler, TLowerData, TLowerTarget>
		: SMLightBase, IBaseSMTaskModifyData<TOwner, TModifyler>
		where TOwner : IBaseSMTaskModifylerOwner<TModifyler>
		where TModifyler : IBaseSMTaskModifyler
	{
		protected TOwner _owner	{ get; private set; }
		protected TModifyler _modifyler	{ get; private set; }
		[SMShowLine] public abstract SMTaskModifyType _type	{ get; }
		[SMShow] bool _isCalledDestructor	{ get; set; }



		public BaseSMTaskModifyData() {
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

		protected async UniTask RunLower( SMTaskRunAllType type, Func<TLowerData> createEvent ) {
			var ls = GetLowers( type );
			switch ( type ) {
				case SMTaskRunAllType.Sequential:
				case SMTaskRunAllType.ReverseSequential:
					foreach ( var l in ls ) {
						await RegisterAndRunLower( l, createEvent() );
					}
					return;
				case SMTaskRunAllType.Parallel:
					await ls.Select( l =>
						RegisterAndRunLower( l, createEvent() )
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