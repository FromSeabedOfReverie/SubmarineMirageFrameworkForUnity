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
	using Extension;
	using Debug;



	public class SMSubChunkData : BaseSMChunkData {
		SMChunkData _ownerChunk	{ get; set; }

		public uint _chunkID	{ get; private set; }
		public Vector3Int _startPosition	{ get; private set; }
		public Vector3Int _endPosition	{ get; private set; }



		public SMSubChunkData( SMChunkData ownerChunk, uint chunkID, Vector3Int startWorldPosition,
								Vector3Int endWorldPosition
		) : base( ownerChunk._owner )
		{
			_ownerChunk = ownerChunk;

			_chunkID = chunkID;
			_startPosition = startWorldPosition;
			_endPosition = endWorldPosition;
		}

		public override void Setup() {
			_lowName = $"{_ownerChunk._lowName}+{_chunkID},L";
			_highName = $"{_ownerChunk._highName}+{_chunkID},H";

			base.Setup();

			if ( !_isExistLow || !_isExistHigh ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"シーンの高低が不足 : ",
					$"{nameof( _isExistLow )} : {_isExistLow}",
					$"{nameof( _isExistHigh )} : {_isExistHigh}"
				) );
			}
		}



		public void Update() {
			var isInside = IsInside( _owner._center.transform.position );
			_nextLoadType = isInside ? SMChunkLoadType.High : SMChunkLoadType.Low;
			Reload().Forget();
		}



		protected override async UniTask SequentialReload( ChunkSMScene enterScene, ChunkSMScene exitScene ) {
			await Enter( enterScene );
			await Exit( exitScene );
		}

		protected override UniTask ParallelReload( ChunkSMScene enterScene, ChunkSMScene exitScene )
			=> UniTask.WhenAll(
				Enter( enterScene ),
				Exit( exitScene )
			);



		public bool IsInside( Vector3 worldPosition )
			=> worldPosition.IsInside( _startPosition, _endPosition );
	}
}