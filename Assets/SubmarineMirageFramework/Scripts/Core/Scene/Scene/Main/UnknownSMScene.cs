//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using System;
	using System.Linq;
	using KoganeUnityLib;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class UnknownSMScene : MainSMScene {
		public UnknownSMScene() {
		}


		public void Setup() {
			// 中心シーンが設定済の場合、未処理
			if ( _owner._mainFSM._body._startStateType != null )	{ return; }

			var scenes = _owner._body.GetUnknownRawScenes();
			var count = scenes.Count();
			if ( count == 0 )	{ return; }

			if ( count > 1 ) {
				throw new InvalidOperationException(
					$"不明なシーンが複数ある為、{nameof( UnknownSMScene )}に設定不可 : \n"
					+ scenes.ToShowString()
				);
			}

			_rawScene = scenes.FirstOrDefault();
			_name = _rawScene.name;
			_fsm._body._startStateType = GetType();

			// テストの場合、高確率で不明シーンはテストシーンな為、下手に読み込み解除しないようにする
			if ( SubmarineMirageFramework.s_isPlayTest ) {
				_exitEvent.Remove( _registerEventName );
			}

//			SMLog.Debug( _owner._mainFSM._body._startStateType?.GetAboutName() );
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