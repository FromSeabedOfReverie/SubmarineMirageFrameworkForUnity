//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using UnityEngine;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMChunkData : BaseSMChunkData {
		Vector3Int _gridPosition	{ get; set; }

		readonly LinkedList<SMSubChunkData> _subChunkData = new LinkedList<SMSubChunkData>();



		public SMChunkData( OpenWorldSMScene owner, Vector3Int gridPosition ) : base( owner ) {
			_gridPosition = gridPosition;
		}

		public override void Setup() {
			var name = $"{_owner._name},{_gridPosition.x},{_gridPosition.y},{_gridPosition.z}";
			_lowName = $"{name},L";
			_highName = $"{name},H";

			base.Setup();

			if ( _isExistLow != _isExistHigh ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"シーンの高低が不足 : ",
					$"{nameof( _isExistLow )} : {_isExistLow}",
					$"{nameof( _isExistHigh )} : {_isExistHigh}"
				) );
			}

			_subChunkData.ForEach( d => d.Setup() );
		}



		protected override void SetLoadType( SMChunkLoadType type ) {
			base.SetLoadType( type );
			_subChunkData.ForEach( d => d._nextLoadType = _loadType );
		}

		protected override UniTask SequentialReload( ChunkSMScene enterScene, ChunkSMScene exitScene )
			=> UniTask.WhenAll(
				new Func<UniTask>( async () => {
					await Enter( enterScene );
					await Exit( exitScene );
				} ).Invoke(),
				ReloadSubChunk()
			);

		protected override UniTask ParallelReload( ChunkSMScene enterScene, ChunkSMScene exitScene )
			=> UniTask.WhenAll(
				Enter( enterScene ),
				Exit( exitScene ),
				ReloadSubChunk()
			);

		UniTask ReloadSubChunk() => UniTask.WhenAll(
			_subChunkData.Select( d => d.Reload() )
		);



		public void AddSubChunk( uint chunkID, Vector3Int startWorldPosition, Vector3Int endWorldPosition )
			=> _subChunkData.AddLast( new SMSubChunkData( this, chunkID, startWorldPosition, endWorldPosition ) );

		public SMSubChunkData GetSubChunk( Vector3Int startPosition, Vector3Int endPosition )
			=> _subChunkData.FirstOrDefault( d =>
				d._startPosition == startPosition &&
				d._endPosition == endPosition
			);

		public SMSubChunkData GetSubChunk( uint chunkID )
			=> _subChunkData.FirstOrDefault( d => d._chunkID == chunkID );
	}
}