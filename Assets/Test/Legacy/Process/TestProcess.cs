//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Test {
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using Process;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 処理の試験クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class TestProcess : BaseProcess {

		static int s_lastID;
		int _id;
		string _color;
//		public override bool _isWaitInitializedCoreProcesses => false;



		public TestProcess() {
			_id = s_lastID;
			s_lastID++;
			_color = new Color( Random.value, Random.value, Random.value ).ToUGUIFormat();
			ShowLog( $"TestProcess()" );
			Application.targetFrameRate = 30;



			_loadEvent += async () => {
				ShowLog( $"_loadEvent : Start" );
				await Wait();
				ShowLog( $"_loadEvent : Finish" );
			};
			_initializeEvent += async () => {
				ShowLog( $"_initializeEvent : Start" );
				await Wait();
				ShowLog( $"_initializeEvent : Finish" );
			};
			_fixedUpdateEvent.Subscribe(
				_ => ShowLog( $"_fixedUpdateEvent" )
			);
			_updateEvent.Subscribe(
				_ => ShowLog( $"_updateEvent" )
			);
			_lateUpdateEvent.Subscribe(
				_ => ShowLog( $"_lateUpdateEvent" )
			);
			_finalizeEvent += async () => {
				ShowLog( $"_finalizeEvent : Start" );
				await Wait();
				ShowLog( $"_finalizeEvent : Finish" );
			};
		}



		public override async UniTask Load() {
			ShowLog( $"Load() : Start" );
			await Wait();
			await base.Load();
			ShowLog( $"Load() : Finish" );
		}
		public override async UniTask Initialize() {
			ShowLog( $"Initialize() : Start" );
			await Wait();
			await base.Initialize();
			ShowLog( $"Initialize() : Finish" );
		}
		public override void FixedUpdate() {
			ShowLog( $"FixedUpdate() : Start" );
			base.FixedUpdate();
			ShowLog( $"FixedUpdate() : Finish" );
		}
		public override void Update() {
			ShowLog( $"Update() : Start" );
			base.Update();
			ShowLog( $"Update() : Finish" );
		}
		public override void LateUpdate() {
			ShowLog( $"LateUpdate() : Start" );
			base.LateUpdate();
			ShowLog( $"LateUpdate() : Finish" );
		}
		public override async UniTask Finalize() {
			ShowLog( $"Finalize() : Start" );
			await Wait();
			await base.Finalize();
			ShowLog( $"Finalize() : Finish" );
		}



		async UniTask Wait() {
			var frame = Random.Range( 5, 10 );
			Log.Debug( $"\tWait : {frame}", Log.Tag.Process );
			await UniTask.DelayFrame( frame );
		}
		void ShowLog( string text ) {
			var s =
				$"{_color}{this.GetAboutName()}</color> {_id} : {text}\n" +
				$"_isInitialized : {_isInitialized} : _executedState : {_executedState}\n";
			Log.Debug( s, Log.Tag.Process );
		}
	}
}