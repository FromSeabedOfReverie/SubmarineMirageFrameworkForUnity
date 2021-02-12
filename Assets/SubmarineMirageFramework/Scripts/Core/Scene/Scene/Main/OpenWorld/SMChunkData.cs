//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using Cysharp.Threading.Tasks;
	using UnityEngine;
	using SubmarineMirage.Base;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMChunkData : SMStandardBase {
		public enum LoadType {
			Unload,
			Low,
			High,
		}

		OpenWorldSMScene _owner	{ get; set; }
		Vector3Int _gridPosition	{ get; set; }

		ChunkSMScene _lowScene	{ get; set; }
		ChunkSMScene _highScene	{ get; set; }
		ChunkSMScene _scene	{ get; set; }

		LoadType _loadType	{ get; set; }
		public LoadType? _nextLoadType	{ get; set; }

		public string _name	{ get; private set; }



		public SMChunkData( OpenWorldSMScene owner, Vector3Int gridPosition ) {
			_owner = owner;
			_gridPosition = gridPosition;
		}

		public void Setup() {
			_name = $"{_owner._name},{_gridPosition.x},{_gridPosition.y},{_gridPosition.z}";
			var lowName = $"{_name},L";
			var highName = $"{_name},H";

			var isExistLow = _owner._owner._body.IsExistSceneInBuild( lowName, true );
			var isExistHigh = _owner._owner._body.IsExistSceneInBuild( highName, true );
			if ( isExistLow != isExistHigh ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"シーンの高低が不足 : ",
					$"{nameof( isExistLow )} : {isExistLow}",
					$"{nameof( isExistHigh )} : {isExistHigh}"
				) );
			}
			if ( isExistLow )	{ _lowScene = new ChunkSMScene( lowName ); }
			if ( isExistHigh )	{ _highScene = new ChunkSMScene( highName ); }

			_lowScene?._body.Setup( _owner._owner, _owner._fsm._body );
			_highScene?._body.Setup( _owner._owner, _owner._fsm._body );

/*
			if ( !isExistLow ) {
				SMLog.Debug(
					$"{lowName}.isExistLow : {isExistLow}\n" + $"{highName}.isExistHigh : {isExistHigh}" );
			}
*/
		}


		public async UniTask Reload() {
			if ( !_nextLoadType.HasValue )	{ return; }
			if ( _loadType == _nextLoadType ) {
				_nextLoadType = null;
				return;
			}
			var lastLoadType = _loadType;
			_loadType = _nextLoadType.Value;
			_nextLoadType = null;

//			SMLog.Debug( $"{_loadType} : {_name}" );


			var lastScene = _scene;

			switch ( _loadType ) {
				case LoadType.Unload:	_scene = null;			break;
				case LoadType.Low:		_scene = _lowScene;		break;
				case LoadType.High:		_scene = _highScene;	break;
			}

			if ( lastLoadType == LoadType.Low && _loadType == LoadType.High ) {
				if ( _scene != null )		{ await _scene._body.Enter(); }
				if ( lastScene != null )	{ await lastScene._body.Exit(); }

			} else if ( lastLoadType == LoadType.High && _loadType == LoadType.Low ) {
				if ( _scene != null )		{ await _scene._body.Enter(); }
				if ( lastScene != null )	{ await lastScene._body.Exit(); }

			} else {
				await UniTask.WhenAll(
					lastScene?._body.Exit() ?? UTask.DontWait(),
					_scene?._body.Enter() ?? UTask.DontWait()
				);
			}

//			_scene._body.UpdateAsync();
		}
	}
}