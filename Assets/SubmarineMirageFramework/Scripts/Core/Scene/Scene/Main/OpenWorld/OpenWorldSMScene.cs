//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System.Linq;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class OpenWorldSMScene : MainSMScene {
		protected abstract Vector3Int _maxChunkCounts	{ get; set; }
		protected abstract int _loadChunkRadius	{ get; set; }
		protected abstract Vector3 _chunkMeterLength	{ get; set; }

		readonly SMChunkData[,,] _chunkData;
		Vector3 _range	{ get; set; }
		Vector3 _originPosition	{ get; set; }
		Vector3 _endPosition	{ get; set; }

		OpenWorldCenter _center	{ get; set; }

		Vector3? _lastCenterPosition	{ get; set; }
		Vector3Int? _lastLowStartPosition	{ get; set; }
		Vector3Int? _lastLowEndPosition	{ get; set; }
		Vector3Int? _lastHighStartPosition	{ get; set; }
		Vector3Int? _lastHighEndPosition	{ get; set; }

		Vector3Int _clampMaxChunkSize	{ get; set; }

		bool _isReloading	{ get; set; }
		bool _isRequestReload	{ get; set; }



		public OpenWorldSMScene() {
			_range = _chunkMeterLength.Mult( _maxChunkCounts );
			_originPosition = _range / 2 * -1;
			_endPosition = _originPosition + _range;
			_clampMaxChunkSize = _maxChunkCounts.Sub( 1 );

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

			_updateEvent.AddLast().Subscribe( _ => UpdateChunkScene() );
		}


		public void Setup( OpenWorldCenter center ) {
			_center = center;

			foreach ( var data in _chunkData ) {
				data.Setup();
			}
		}



		void UpdateChunkScene() {
			if ( _center == null )	{ return; }
			var center = _center.transform.position;
			if ( center == _lastCenterPosition )	{ return; }
			_lastCenterPosition = center;


			var centerLocal = center - _originPosition;
			var centerGridScale = centerLocal.Div( _chunkMeterLength );
			var centerGrid = centerGridScale.FloorToInt();

			var isLowInside = false;
			var isReloadLow = UpdateLow( centerGrid, out isLowInside );
			var isReloadHigh = UpdateHigh( centerGrid, centerGridScale, isLowInside );

			if ( !isReloadLow && !isReloadHigh )	{ return; }

			ReloadChunkScenes().Forget();
		}


		bool UpdateLow( Vector3Int center, out bool isInside ) {
			var start = center.Sub( _loadChunkRadius );
			var end = center.Add( _loadChunkRadius );

			isInside = IsInside( start, end );
			if ( isInside ) {
				start.Clamp( Vector3Int.zero, _clampMaxChunkSize );
				end.Clamp( Vector3Int.zero, _clampMaxChunkSize );

				var isReload = start != _lastLowStartPosition || end != _lastLowEndPosition;
				_lastLowStartPosition = start;
				_lastLowEndPosition = end;
				return isReload;

			}

			var isUnload = _lastLowStartPosition != null;
			_lastLowStartPosition = null;
			_lastLowEndPosition = null;
			return isUnload;
		}


		bool UpdateHigh( Vector3Int center, Vector3 centerScale, bool isLowInside ) {
			if ( isLowInside ) {
				var start = center;
				var end = start + centerScale.OutsideDirection();
				var tempStart = Vector3Int.Min( start, end );
				var tempEnd = Vector3Int.Max( start, end );
				start = tempStart;
				end = tempEnd;

				if ( IsInside( start, end ) ) {
					start.Clamp( Vector3Int.zero, _clampMaxChunkSize );
					end.Clamp( Vector3Int.zero, _clampMaxChunkSize );

					var isReload = start != _lastHighStartPosition || end != _lastHighEndPosition;
					_lastHighStartPosition = start;
					_lastHighEndPosition = end;
					return isReload;
				}
			}

			var isUnload = _lastHighStartPosition != null;
			_lastHighStartPosition = null;
			_lastHighEndPosition = null;
			return isUnload;
		}


		async UniTask ReloadChunkScenes() {
			_isRequestReload = true;
			if ( _isReloading )	{ return; }
			_isReloading = true;
			_isRequestReload = false;


			if ( _lastLowStartPosition.HasValue ) {
				SetNextLoadType(
					SMChunkData.LoadType.Low, _lastLowStartPosition.Value, _lastLowEndPosition.Value );
			}
			if ( _lastHighStartPosition.HasValue ) {
				SetNextLoadType(
					SMChunkData.LoadType.High, _lastHighStartPosition.Value, _lastHighEndPosition.Value );
			}

			await _chunkData
				.SelectRaw( d => (SMChunkData)d )
				.Select( d => {
					if ( d._nextLoadType == null ) {
						d._nextLoadType = SMChunkData.LoadType.Unload;
//						SMLog.Debug( d._name );
					}
					return d.Reload();
				} );
			await Resources.UnloadUnusedAssets().ToUniTask( _asyncCancelerOnDispose );


			_isReloading = false;
			if ( _isRequestReload )	{ ReloadChunkScenes().Forget(); }
		}


		void SetNextLoadType( SMChunkData.LoadType type, Vector3Int start, Vector3Int end ) {
			for ( var x = start.x; x <= end.x; x++ ) {
				for ( var y = start.y; y <= end.y; y++ ) {
					for ( var z = start.z; z <= end.z; z++ ) {
						var data = _chunkData[x, y, z];
						data._nextLoadType = type;
//						SMLog.Debug( $"{type} : {x},{y},{z}" );
					}
				}
			}
		}


		public bool IsInside( Vector3Int gridPosition )
			=> gridPosition.IsInside( Vector3Int.zero, _maxChunkCounts );


		public bool IsInside( Vector3Int startGridPosition, Vector3Int endGridPosition ) {
			var aStart = startGridPosition;
			var aEnd = endGridPosition;
			var bStart = Vector3Int.zero;
			var bEnd = _clampMaxChunkSize;

			if (
				aStart.x <= bEnd.x && bStart.x <= aEnd.x &&
				aStart.y <= bEnd.y && bStart.y <= aEnd.y &&
				aStart.z <= bEnd.z && bStart.z <= aEnd.z
			) {
				return true;
			}
			return false;
		}
	}
}