//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.ReadMe {

    using System.Linq;
    using Cysharp.Threading.Tasks;
	using SubmarineMirage.UTask;
    using SubmarineMirage.Process;
    // 実行時に、一番最初に処理されるクラス
    // Assets/SubmarineMirageFrameworkForUnity/Scripts/Main/MainProcess.csを、簡易的に記述している
    // 実際は、そこを編集する
    public class MainProcess {
        // 外部プラグインを初期化する
        static async UniTask InitializePlugin() {
            // ここで、様々な外部プラグインの初期化を記述する
            // ...省略...
            await UTask.DontWait();
        }
        // 処理を登録する
        static async UniTask RegisterProcesses() {
            // ここで、シングルトン等のProcess系クラスを初期化し、呼び出し順を確定させる
            // ...省略...
            await TestSingleton.WaitForCreation();    // シングルトン試験の生成と登録を行い、完了まで待機
			var c = new System.Threading.CancellationTokenSource();
            await UTask.NextFrame( c.Token );
			c.Dispose();
        }
    }
    // 登録済シングルトンを呼ぶ為の、コンポーネントクラス
    public class UseSingletonByRegister : MonoBehaviourProcess {
        // このコンポーネントが使用する試験データ
        string _data;
        // コンストラクタで、初期化時に管理クラスからデータ取得、を設定
        protected override void Constructor() {
            // ここでシングルトン未使用でも、生成済で内部登録済の為、読込処理が行われる
    //      var i = TestSingleton.s_instance;
            _initializeEvent += async () => {
                _data = TestSingleton.s_instance._data.FirstOrDefault();
                await UTask.DontWait();
            };
        }
    }

}