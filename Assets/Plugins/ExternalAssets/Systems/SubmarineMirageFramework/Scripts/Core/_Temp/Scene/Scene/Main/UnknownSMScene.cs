//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;
	using KoganeUnityLib;



	public class UnknownSMScene : MainSMScene {
		public UnknownSMScene() {
			// テストの場合、高確率で不明シーンはテストシーンな為、下手に読み込み解除しないようにする
			if ( SMDebugManager.s_isPlayTest ) {
				_exitEvent.Remove( _registerEventName );
			}
		}


		public void Setup( List<Scene> unknownScenes ) {
			var count = unknownScenes.Count();
			if ( count == 0 ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"設定不可 : 不明場面が無",
					$"{this.GetName()}.{nameof( Setup )}",
					$"{unknownScenes.ToShowString( 0, true )}"
				) );
			}
			if ( count > 1 ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"設定不可 : 不明場面が複数存在",
					$"{this.GetName()}.{nameof( Setup )}",
					$"{unknownScenes.ToShowString( 0, true )}"
				) );
			}

			_rawScene = unknownScenes.FirstOrDefault();
			_name = _rawScene.name;
		}


		protected override void SetSceneName() {
			_name = "";
		}

		protected override void ReloadRawScene() {
			if ( _name.IsNullOrEmpty() )	{ return; }

			base.ReloadRawScene();
		}
	}
}