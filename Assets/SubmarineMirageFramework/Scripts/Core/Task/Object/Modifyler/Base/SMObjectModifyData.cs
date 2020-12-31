//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using Task.Modifyler;
	using Behaviour;
	using Behaviour.Modifyler;



	// TODO : コメント追加、整頓



	public abstract class SMObjectModifyData
		: BaseSMTaskModifyData<SMObject, SMObjectModifyler, SMObject, SMBehaviourModifyData, SMBehaviourBody>
	{
		protected SMTaskRunAllType _runType	{ get; private set; }


		public SMObjectModifyData( SMTaskRunAllType runType ) : base( null )
			=> _runType = runType;

		public override void Set( SMObject owner ) {
			base.Set( owner );
			_target = _owner;
			if ( _owner == null || _owner._isDispose ) {
				throw new ObjectDisposedException( $"{nameof( _owner )}", $"既に解放、削除済\n{_owner}" );
			}
		}


		protected override UniTask RegisterAndRunLower( SMBehaviourBody lowerTarget, SMBehaviourModifyData data )
			=> lowerTarget._modifyler.RegisterAndRun( data );

		protected override IEnumerable<SMBehaviourBody> GetAllLowers()
			=> _owner.GetBehaviours().Select( b => b._body );

		protected override bool IsTargetLower( SMBehaviourBody lowerTarget, SMTaskType type )
			=> lowerTarget._owner._type == type;
	}
}