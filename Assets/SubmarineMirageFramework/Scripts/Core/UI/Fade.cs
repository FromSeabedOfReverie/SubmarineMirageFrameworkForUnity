//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using Singleton;
	///====================================================================================================
	/// <summary>
	/// ■ フェードのクラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class Fade : Singleton<Fade> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>フェード状態</summary>
		public enum State {
			/// <summary>フェードイン</summary>
			In,
			/// <summary>フェードアウト</summary>
			Out
		}

		/// <summary>現在の、フェード状態</summary>
		State _state;
		/// <summary>フェード画像</summary>
		Image _image;
		/// <summary>フェード率</summary>
		float _rate;
		/// <summary>フェード速度</summary>
		[SerializeField] float _velocity = 0.7f;
		/// <summary>フェードインの色</summary>
		public Color _inColor = Color.clear;
		/// <summary>フェードアウトの色</summary>
		public Color _outColor = Color.black;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public Fade() {
			// ● 初期化
			_initializeEvent += async () => {
				var go = Object.Instantiate( Resources.Load<GameObject>( "UIFade" ) );
				Object.DontDestroyOnLoad( go );
				_image = go.GetComponentInChildren<Image>();

				_rate = 1;
				ChangeState( State.Out );

				await UniTask.Delay( 0 );
			};


			// ● 更新
			_updateEvent.Subscribe( _ => {
				// フェード率を更新
				_rate += _velocity * Time.deltaTime;
				_rate = Mathf.Clamp01( _rate );

				// 画像の不透明度にフェード率を設定
				_image.color = Color.Lerp( _inColor, _outColor, _rate );

				// フェード下のUIが押せるかを更新
				_image.raycastTarget = _rate > 0;
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 状態を変更
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void ChangeState( State state ) {
			if ( _state != state ) {
				_state = state;
				_velocity = ( state == State.In ? -1 : 1 ) * Mathf.Abs( _velocity );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 速度を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void SetVelocity( float velocity ) {
			if ( _velocity != velocity ) {
				_velocity = velocity;
				_state = velocity < 0 ? State.In : State.Out;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● フェードが完了したか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsFinish( State state ) {
			return (
				_state == state &&
				(	( state == State.In && _rate == 0 ) ||
					( state == State.Out && _rate == 1 ) )
			);
		}
	}
}