//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	using UnityEngine.UI;
	using Cysharp.Threading.Tasks;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ フェードのクラス
	/// </summary>
	///====================================================================================================
	public class SMUIFade : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>フェード状態</summary>
		public enum State {
			/// <summary>フェードイン完了</summary>
			In,
			/// <summary>フェード中</summary>
			Fading,
			/// <summary>フェードアウト完了</summary>
			Out
		}
		const float DEFAULT_VELOCITY = 0.7f;

		public override SMTaskRunType _type => SMTaskRunType.Sequential;

		/// <summary>現在の、フェード状態</summary>
		public State _state { get; private set; } = State.Out;
		public bool _isFading => _state == State.Fading;

		/// <summary>フェード画像</summary>
		Image _image { get; set; }
		/// <summary>フェード率</summary>
		float _rate { get; set; } = 1;
		/// <summary>フェードインの色</summary>
		public Color _inColor { get; set; } = Color.clear;
		/// <summary>フェードアウトの色</summary>
		public Color _outColor { get; set; } = Color.black;

		readonly SMAsyncCanceler _fadeCanceler = new SMAsyncCanceler();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMUIFade() {
			var go = Object.Instantiate( Resources.Load<GameObject>( "Prefabs/UIFade" ) );
			Object.DontDestroyOnLoad( go );
			_image = go.GetComponentInChildren<Image>();

			Out( 10000 ).Forget();

			_disposables.AddFirst( () => {
				_fadeCanceler.Dispose();
				_image.gameObject.Destroy();
			} );
		}
		
		public override void Create () {
		}



		public async UniTask In( float velocity = DEFAULT_VELOCITY ) {
			_fadeCanceler.Cancel();
			_state = State.Fading;
			while ( _rate > 0 ) {
				_rate -= velocity * Time.unscaledDeltaTime;
				_image.color = Color.Lerp( _inColor, _outColor, _rate );
				await UTask.NextFrame( _fadeCanceler );
			}
			_rate = 0;
			_state = State.In;
			_image.raycastTarget = false;
		}

		public async UniTask Out( float velocity = DEFAULT_VELOCITY ) {
			_fadeCanceler.Cancel();
			_image.raycastTarget = true;
			_state = State.Fading;
			while ( _rate < 1 ) {
				_rate += velocity * Time.unscaledDeltaTime;
				_image.color = Color.Lerp( _inColor, _outColor, _rate );
				await UTask.NextFrame( _fadeCanceler );
			}
			_rate = 1;
			_state = State.Out;
		}
	}
}