//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMChunkData : SMStandardBase {
		public OpenWorldSMScene _owner	{ get; private set; }

		ChunkSMScene _lowScene	{ get; set; }
		ChunkSMScene _highScene	{ get; set; }
		ChunkSMScene _scene	{ get; set; }

		public SMChunkLoadType _loadType	{ get; private set; }
		public SMChunkLoadType? _nextLoadType	{ get; set; }

		public string _lowName	{ get; protected set; }
		public string _highName	{ get; protected set; }
		protected bool _isExistLow	{ get; private set; }
		protected bool _isExistHigh	{ get; private set; }

		public bool _isKeepLow	{ get; set; }



		public BaseSMChunkData( OpenWorldSMScene owner ) {
			_owner = owner;
		}

		public virtual void Setup() {
			_isExistLow = _owner._owner._body.IsExistSceneInBuild( _lowName, true );
			_isExistHigh = _owner._owner._body.IsExistSceneInBuild( _highName, true );

			if ( _isExistLow )	{ _lowScene = new ChunkSMScene( _lowName ); }
			if ( _isExistHigh )	{ _highScene = new ChunkSMScene( _highName ); }

			_lowScene?._body.Setup( _owner._owner, _owner._fsm._body );
			_highScene?._body.Setup( _owner._owner, _owner._fsm._body );
		}



		public async UniTask Reload() {
// TODO : 重複呼びを防止するキューを追加
			if ( !_nextLoadType.HasValue )	{ return; }

			if ( _isKeepLow && _nextLoadType == SMChunkLoadType.Unload ) {
				_nextLoadType = SMChunkLoadType.Low;
			}
			if ( _loadType == _nextLoadType ) {
				_nextLoadType = null;
				return;
			}

			var lastLoadType = _loadType;
			SetLoadType( _nextLoadType.Value );
			_nextLoadType = null;

			var lastScene = _scene;
			switch ( _loadType ) {
				case SMChunkLoadType.Unload:	_scene = null;			break;
				case SMChunkLoadType.Low:		_scene = _lowScene;		break;
				case SMChunkLoadType.High:		_scene = _highScene;	break;
			}

			if (
				( lastLoadType == SMChunkLoadType.Low && _loadType == SMChunkLoadType.High ) ||
				( lastLoadType == SMChunkLoadType.High && _loadType == SMChunkLoadType.Low )
			) {
				await SequentialReload( _scene, lastScene );
			} else {
				await ParallelReload( _scene, lastScene );
			}

//			_scene._body.UpdateAsync();
		}

		protected virtual void SetLoadType( SMChunkLoadType type )
			=> _loadType = type;

		protected abstract UniTask SequentialReload( ChunkSMScene enterScene, ChunkSMScene exitScene );

		protected abstract UniTask ParallelReload( ChunkSMScene enterScene, ChunkSMScene exitScene );

		protected UniTask Enter( ChunkSMScene scene )
			=> scene?._body.Enter() ?? UTask.DontWait();

		protected UniTask Exit( ChunkSMScene scene )
			=> scene?._body.Exit() ?? UTask.DontWait();
	}
}