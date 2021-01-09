//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestTask {
	using System.Linq;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using KoganeUnityLib;
	using Task;
	using Extension;
	using Debug;
	using TestBase;



	// TODO : コメント追加、整頓



	public class TestSMTaskRunner : SMStandardTest {
		SMTaskRunner _taskRunner	{ get; set; }
		Text _text	{ get; set; }


		protected override void Create() {
			Application.targetFrameRate = 30;
			_taskRunner = SMTaskRunner.s_instance;

			Resources.Load<GameObject>( "TestCamera" ).Instantiate();
			var go = Resources.Load<GameObject>( "TestCanvas" ).Instantiate();
			go.DontDestroyOnLoad();
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _taskRunner == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text = string.Join( "\n",
					$"{_taskRunner.GetAboutName()}(",
					$"    {nameof( _taskRunner._isDispose )} : {_taskRunner._isDispose}",
					")"
				);
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_disposables.AddLast( _taskRunner );
		}
	}
}