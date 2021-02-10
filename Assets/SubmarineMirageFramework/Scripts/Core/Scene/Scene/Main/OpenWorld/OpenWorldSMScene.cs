//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Linq;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Extension;



	// TODO : コメント追加、整頓



	public abstract class OpenWorldSMScene : MainSMScene {
		protected abstract Vector3Int _maxChunkCounts	{ get; set; }
		protected abstract int _loadChunkRadius	{ get; set; }
		protected abstract Vector3 _chunkMeterLength	{ get; set; }

		readonly SMChunkData[,,] _chunkData;
		Vector3 _range	{ get; set; }
		Vector3 _origin	{ get; set; }
		Vector3 _end	{ get; set; }

		OpenWorldCenter _center	{ get; set; }


		public OpenWorldSMScene() {
			_range = _chunkMeterLength.Mult( _maxChunkCounts );
			_origin = _range / 2 * -1;
			_end = _origin + _range;

			_chunkData = new SMChunkData[
				_maxChunkCounts.x,
				_maxChunkCounts.y,
				_maxChunkCounts.z
			];

			_maxChunkCounts.x.Times( x => {
				_maxChunkCounts.y.Times( y => {
					_maxChunkCounts.z.Times( z => {
						var data = new SMChunkData( this, new Vector3Int( x, y, z ) );
						_chunkData[x, y, z] = data;
					} );
				} );
			} );
		}


		public void Setup( OpenWorldCenter center ) {
			_center = center;
		}


		async UniTask InitialLoad() {
			var local = _center.transform.position - _origin;

			var grid = local.Div( _chunkMeterLength );
			var center = grid.FloorToInt();


			var lowStart = center.Sub( _loadChunkRadius );
			var lowEnd = center.Add( _loadChunkRadius );

			var highStart = center;
			var highEnd = highStart + new Vector3Int(
				grid.x % 1 < 0.5 ? -1 : 1,
				grid.y % 1 < 0.5 ? -1 : 1,
				grid.z % 1 < 0.5 ? -1 : 1
			);


			var isLowOutside = (
				lowStart.IsOutside( Vector3Int.zero, _maxChunkCounts ) &&
				lowEnd.IsOutside( Vector3Int.zero, _maxChunkCounts )
			);
			if ( isLowOutside )	{ return; }

			var max = _maxChunkCounts.Sub( 1 );
			lowStart.Clamp( Vector3Int.zero, max );
			lowEnd.Clamp( Vector3Int.zero, max );

			var isLoadHigh = (
				highStart.IsInside( Vector3Int.zero, _maxChunkCounts ) ||
				highEnd.IsInside( Vector3Int.zero, _maxChunkCounts )
			);
			if ( isLoadHigh ) {
				highStart.Clamp( Vector3Int.zero, max );
				highEnd.Clamp( Vector3Int.zero, max );
				var tempStart = Vector3Int.Min( highStart, highEnd );
				var tempEnd = Vector3Int.Max( highStart, highEnd );
				highStart = tempStart;
				highEnd = tempEnd;
			}


			var loadTypes = new SMChunkData.LoadType[_maxChunkCounts.x, _maxChunkCounts.y, _maxChunkCounts.z];



			_maxChunkCounts.x.Times( x => {
				_maxChunkCounts.y.Times( y => {
					_maxChunkCounts.z.Times( z => {
						loadTypes[x, y, z] = SMChunkData.LoadType.Unload;
					} );
				} );
			} );
			for ( var x = lowStart.x; x <= lowEnd.x; x++ ) {
				for ( var y = lowStart.y; y <= lowEnd.y; y++ ) {
					for ( var z = lowStart.z; z <= lowEnd.z; z++ ) {
						loadTypes[x, y, z] = SMChunkData.LoadType.Low;
					}
				}
			}
			for ( var x = highStart.x; x <= highEnd.x; x++ ) {
				for ( var y = highStart.y; y <= highEnd.y; y++ ) {
					for ( var z = highStart.z; z <= highEnd.z; z++ ) {
						loadTypes[x, y, z] = SMChunkData.LoadType.High;
					}
				}
			}

			for ( var x = lowStart.x; x <= lowEnd.x; x++ ) {
				for ( var y = lowStart.y; y <= lowEnd.y; y++ ) {
					for ( var z = lowStart.z; z <= lowEnd.z; z++ ) {
						var data = _chunkData[x, y, z];
						var type = loadTypes[x, y, z];
						await data.Load( type );
					}
				}
			}
		}
	}
}