//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using UnityEngine;
using SubmarineMirage.Scene;



// TODO : コメント追加、整頓



public class FieldSMScene : OpenWorldSMScene {
	protected override Vector3Int _maxChunkCounts	{ get; set; } = new Vector3Int( 6, 4, 6 );
	protected override int _loadChunkRadius	{ get; set; } = 2;
	protected override Vector3 _chunkMeterLength	{ get; set; } = new Vector3( 10, 10, 10 );
}