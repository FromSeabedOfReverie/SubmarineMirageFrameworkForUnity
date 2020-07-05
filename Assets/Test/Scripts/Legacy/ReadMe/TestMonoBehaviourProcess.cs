//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.ReadMe {

    using UniRx;
    using Cysharp.Threading.Tasks;
    using SubmarineMirage.Process;
    // MonoBehaviourProcessは、処理順序に規則を持たせたMonoBehaviour
    // UniRxで無理矢理組むような、リアクティブスパゲッティを防止できる
    public class TestMonoBehaviourProcess : MonoBehaviourProcess {
        // 疑似的コンストラクタ
        // 生成時に呼ばれるが、実際のコンストラクタは実行時以外にも呼ばれる為、これを使用
        protected override void Constructor() {
            // 読込処理を設定
            // 各種管理クラスの初期化後、最初に管理クラスから呼ばれる
            _loadEvent += async () => {
                // AssetBundle、ServerData等、自身で完結する非同期処理を記述
                await UniTask.Delay( 0 );
            };
            // 初期化処理を設定
            // _loadEvent実行後、管理クラスから呼ばれる
            _initializeEvent += async () => {
                // 他オブジェクトの要素、管理クラスの要素等、読込済の他者の取得処理を記述
                await UniTask.Delay( 0 );
            };
            // 更新処理を設定
            // _initializeEvent実行後、管理クラスから毎フレーム呼ばれる
            _updateEvent.Subscribe( _ => {
                // 毎フレーム実行する処理を記述
            } );
            // 終了処理を設定
            // 破棄直前に管理クラスから呼ばれる
            _finalizeEvent += async () => {
                // 破棄が必要な、非同期処理を記述
                await UniTask.Delay( 0 );
            };
        }
    }

}