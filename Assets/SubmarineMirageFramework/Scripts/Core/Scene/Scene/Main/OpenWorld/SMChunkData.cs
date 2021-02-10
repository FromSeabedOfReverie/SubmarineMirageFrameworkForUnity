//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using Cysharp.Threading.Tasks;
	using UnityEngine;
	using SubmarineMirage.Base;
	using Utility;



	// TODO : コメント追加、整頓



	public class SMChunkData : SMStandardBase {
		public enum LoadType {
			High,
			Low,
			Unload,
		}

		OpenWorldSMScene _ownerScene	{ get; set; }
		Vector3Int _position	{ get; set; }
		ChunkSMScene _highScene	{ get; set; }
		ChunkSMScene _lowScene	{ get; set; }
		ChunkSMScene _scene	{ get; set; }



		public SMChunkData( OpenWorldSMScene ownerScene, Vector3Int position ) {
			_ownerScene = ownerScene;
			_position = position;

			var name = $"{_ownerScene._name},{_position.x},{_position.y},{_position.z}";
			_highScene = new ChunkSMScene( $"{name},H" );
			_lowScene = new ChunkSMScene( $"{name},L" );

			_highScene._body.Setup( ownerScene._owner, ownerScene._fsm._body );
			_lowScene._body.Setup( ownerScene._owner, ownerScene._fsm._body );
		}


		public async UniTask Load( LoadType type ) {
			await UTask.DontWait();
		}
	}
}