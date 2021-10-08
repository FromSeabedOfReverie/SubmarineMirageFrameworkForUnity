# ★ 海底の蜃気楼フレームワークの使い方
![0.png](/Assets/SubmarineMirageFramework/Readme/0.png)  



## ● 基本
### 概要  
当フレームワークは、Unityで作るのが面倒な共通機能をまとめた物です。  
Unityの通常動作を妨害しないので、全く使わなくても問題ありません。  



### フォルダ階層
ゲーム本体のアセットは、Assets\Application\、に追加して下さい。  
フレームワークは、Assets\SubmarineMirageFramework\、にありますが、特に触る必要はないです。  



### 注意事項
非同期処理（UniTask）の都合上、面倒なルールが1つだけあります。  
Unityエディタで開発中、下記のショートカットをご利用下さい。

- F5キー : ゲームの実行と停止（元は、Ctrl+P）  
- F6キー : ゲームの一時停止（元は、Ctrl+Shift+P）  
- F7キー : ゲームのステップ実行（元は、Ctrl+Alt+P）  

標準ボタン使用でも良いですが、まれに非同期処理が残って溜まる、外部ライブラリ（UniTask）のバグがあります。  
その為、F5キーのショートカットに、1フレーム待機してから終了する処理を加えています。  
（万が一非同期が溜まっても、次の実行時に消えるので、それほど問題ありません。）  
ビルド後は、非同期は残らないので問題ありません。  



### デバッグウィンドウ
Unityエディタの上部ツールバーから、デバッグウィンドウを表示できます。  
お手数をお掛けしますが、フレームワークの挙動がおかしい時に、確認お願いします。  

Window → SubmarineMirage → TaskManager、に登録されたタスク一覧が表示されます。  
![1.png](/Assets/SubmarineMirageFramework/Readme/1.png)  

Window → SubmarineMirage → ServiceLocator、に登録されたサービス一覧が表示されます。  
![2.png](/Assets/SubmarineMirageFramework/Readme/2.png)  

Window → UniTask Tracker、にUniTask（外部ライブラリ）で実行中の非同期処理が表示されます。  
エディタがフリーズする等のクリティカルな問題は、非同期処理が詰まって起こる可能性が高いです。  
![3.png](/Assets/SubmarineMirageFramework/Readme/3.png)  



## ● プログラム解説
### プログラム開始地点
ゲーム実行後、Assets\Application\Scripts\ApplicationMain.cs、を最初に実行します。  
外部ライブラリや、データ設定を登録し、フレームワーククラスに渡しています。  
追加の初期化処理は、ここに追加下さい。  
```csharp
public static class ApplicationMain {
  static async UniTask InitializePlugin() {
    // ここで、外部ライブラリの初期化を行って下さい。
    await UTask.DontWait();
  }
  static async UniTask RegisterSettings() {
    // ここで、サービスロケーター登録や、管理クラスの初期化を行って下さい。
    await UTask.DontWait();
  }
}
```


### プログラム終了方法
```csharp
using SubmarineMirage;

class Sample {
  void Example() {
    // 安全にゲームを終了する
    SubmarineMirageFramework.Shutdown();
  }
}
```
Application.Quitを使用しても、これが呼ばれるので、どちらを使用しても構いません。  
Application.Quitは、ビルド後のゲーム終了はできますが、ビルド前のエディタ実行の終了はできません。  
これは、エディタ実行も終了できる利点があります。  



### 設定
Assets\SubmarineMirageFramework\Scripts\Setting\、にフレームワークの設定プログラムが多々あります。  
Assets\SubmarineMirageFramework\Scripts\Setting\SMMainSetting.cs、は特に重要で、フォルダ階層や暗号化を設定しています。  
Assets\Application\Scripts\Setting\、にゲーム固有設定を定義しています。  



### シーン
Assets\Application\Scenes\、にゲームで使うシーンがあります。  
Assets\Application\Scripts\Scene\、にシーンに対応するプログラムがあります。  
「シーン名 + SMScene」がクラス名の命名規則で、ロードするシーン名を自動設定します。  
（例 : Game + SMScene = GameSMScene.cs）  

Assets\Application\Scripts\Setting\SMSceneSetting.cs、にシーン登録が必要です。  
```csharp
public class SMSceneSetting : BaseSMSceneSetting {
  public override void Setup() {
    _datas = new SMFSMGenerateList {
      {
        // アクティブ状態とする、メインシーン
        new Type[] {
          typeof( GameSMScene ),  // ←のように、Type型でシーンを登録下さい。
        },
        typeof( MainSMScene )
      }
    };
  }
}
```

シーン遷移するには、これを呼びます。  
```csharp
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.Scene;

class Sample {
  void Example() {
    // ゲームシーンに遷移します
    var mainFSM = SMServiceLocator.Resolve<SMSceneManager>().GetFSM<MainSMScene>();
    mainFSM.ChangeState<GameSMScene>().Forget();
  }
}
```

未登録シーンでゲーム実行した場合、自動的にUnknownSMSceneに設定されます。  
個人的なテストシーンでは、何も設定しなくて大丈夫です。（シーン対応クラスも未作成でOKです。）  
- 登録シーン → 未登録シーン : 遷移不可  
- 未登録シーン → 登録シーン : 遷移可能  

TitleSMScene.cs、GameSMScene.cs、GameOverSMScene.cs、GameClearSMScene.cs、に単純なシーン遷移、非同期イベントのデモを用意しました。  
不要な場合、コンストラクタの中身を消して下さい。  
```csharp
public class GameSMScene : MainSMScene {
  public GameSMScene() {
    // 中身を消して下さい
  }
}
```



### デバッグ
Assets\SubmarineMirageFramework\Scripts\Core\Debug\Log\SMLog.cs、はUnityEngine.Debugの拡張ラッパーです。  
```csharp
using SubmarineMirage.Debug;
using SubmarineMirage.Setting;

class Sample {
  void Example() {
    // SMLogTagは省略可能ですが、指定すると先頭に色文字が付きます。
    SMLog.Debug( "デバッグ文章", SMLogTag.AI );
    SMLog.Warning( "警告文章",   SMLogTag.Data );
    SMLog.Error( "エラー文章",   SMLogTag.File );
  }
}
```

Assets\SubmarineMirageFramework\Scripts\Core\Debug\Log\SMDisplayLog.cs、はゲーム画面にデバッグ用の文字列を表示します。  
```csharp
using UnityEngine;
using SubmarineMirage.Service;
using SubmarineMirage.Debug;

class Sample {
  void Example() {
    // SMDisplayLogは、LateUpdateでリセットされる為、毎フレーム登録して下さい。
    void Update() {
      var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();
      displayLog.Add( Color.blue );
      displayLog.Add( "文章" );
    }
  }
}
```
標準デバッグキーの1キーを押すか、マウス左右ボタンの同時押し（スマホの二本指タッチ）で下から上にスワイプすると、非表示になります。  

これらは、Developビルドのみの機能で、Releaseビルドでは処理されません。  



### Input
Assets\SubmarineMirageFramework\Scripts\Setting\Input\Enum\、にキー対応のEnumが定義されています。  
Assets\SubmarineMirageFramework\Scripts\Setting\Input\SMInputManager.cs、はInputの拡張ラッパーです。  
キーごとの押された処理は、SMInputManagerに記述されているので、追加にベタ書きが必要です。  
（時間無くて、処理を分離できませんでした...。余裕があればリファクタします...。）  

Unityエディタの上部ツールバーの、Edit → ProjectSettings → InputManager、でキー設定が必要です。  
（なお、新InputSystemには非対応です。）  
![4.png](/Assets/SubmarineMirageFramework/Readme/4.png)  

UnityのuGUIを使う為にEventSystemが必要ですが、Dummy名を登録下さい。  
（EventSystem自動生成時の標準ボタン（Horizontal、Vertical、Submit、Cancel）は、削除している為、エラーになります。）  
Dummy系は、何もしない入力の為、意図せずuGUIが反応する事故を防げます。  
![5.png](/Assets/SubmarineMirageFramework/Readme/5.png)  

```csharp
using UniRx;
using SubmarineMirage.Service;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;

class Sample {
  void Example() {
    var inputManager = SMServiceLocator.Resolve<SMInputManager>();

    // 決定ボタン（Enterキー、マウス左クリック）状態を取得
    var key = inputManager.GetKey( SMInputKey.Decide );
    key._enabledEvent.AddLast().Subscribe( _ =>         SMLog.Debug( "キーが押された直後" ) );
    key._enablingEvent.AddLast().Subscribe( _ =>        SMLog.Debug( "キーが押され続けている" ) );
    key._thinOutEnablingEvent.AddLast().Subscribe( _ => SMLog.Debug( "キーが定期的に押され続けている" ) );
    key._disabledEvent.AddLast().Subscribe( _ =>        SMLog.Debug( "キーが離された直後" ) );
    if ( key._isEnabled )         { SMLog.Debug( "キーが押された直後" ); }
    if ( key._isEnabling )        { SMLog.Debug( "キーが押され続けている" ); }
    if ( key._isThinOutEnabling ) { SMLog.Debug( "キーが定期的に押され続けている" ); }
    if ( key._isDisabled )        { SMLog.Debug( "キーが離された直後" ); }

    // WASD、矢印キーの押下状態を、2次元ベクトルで取得
    var vector = inputManager.GetAxis( SMInputAxis.Move );

    // 下方向へのスワイプ状態を取得
    var swipe = inputManager.GetSwipe( SMInputSwipe.Down );
    swipe._enabledEvent.AddLast().Subscribe( _ => SMLog.Debug( "スワイプ直後" ) );
    if ( swipe._isEnabled ) { SMLog.Debug( "スワイプ直後" ); }
  }
}
```
.Subscribe( _ => \{} )系は、UniRxのSubjectを使用しています。



### セーブデータ
Assets\Application\Scripts\Data\Save\PlayData.cs、にゲームプレイのセーブデータを定義し、  
Data\PlayData0.data、に保存されます。  

Assets\Application\Scripts\Data\Save\SettingData.cs、にゲーム設定のセーブデータを定義し、  
Data\SettingData.data、に保存されます。  

（他に、サーバー通信キャッシュのセーブデータがあります。）

PlayData.cs、SettingData.cs、に必要な変数を追加するとセーブされます。  
Assets\SubmarineMirageFramework\Scripts\Setting\SMMainSetting.cs、に保存フォルダを指定しています。  
スマホ、PCなどのビルド種類で、保存フォルダを自動変更します。  
セーブデータは、暗号化が施されています。  
アプリバージョン変更に対応し、セーブデータも更新されます。  

```csharp
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.Data;

class Sample {
  void Example() {
    // プログラム実行直後に、全セーブデータ読込を、すでに完了しています。
    // ゲームプレイのセーブデータから、勝利回数を取得します。
    var playDatas = SMServiceLocator.Resolve<SMAllDataManager>().Get<PlayDataManager>();
    var data = playDatas._currentData;  // 複数のゲームプレイデータの内、現在ロード中のデータを取得
    var count = data._winCount;

    // ゲームプレイのセーブデータの、勝利回数を更新し、ファイルに保存します。
    var playDatas = SMServiceLocator.Resolve<SMAllDataManager>().Get<PlayDataManager>();
    var data = playDatas._currentData;  // 複数のゲームプレイデータの内、現在ロード中のデータを取得
    data._winCount += 1;
    playDatas.SaveCurrentData().Forget();  // 現在ロード中のデータをファイルに保存
  }
}
```



### マスターデータ
CSVファイルから、ゲームデータを簡単に設定できます。  
Assets\Application\Resources\Data\、にCSVファイルを配置下さい。  
Assets\Application\Scripts\Data\Master\SampleItemData.cs、を参考にデータクラスを作ります。  

作成したデータクラスの、_registerKey変数に登録キー（DictionaryのKey）を設定します。  

Assets\Application\Scripts\Setting\SMDataSetting.cs、にデータクラスを登録します。
```csharp
public class SMDataSetting : BaseSMDataSetting {
  public override void Setup() {
    base.Setup();
    _datas = new Dictionary< SMDataSettingType, List<IBaseSMDataManager> > {
      {
        // マスターデータを登録
        SMDataSettingType.Master,
        new List<IBaseSMDataManager> {
          // ここに、データクラスを登録して下さい。
          new SMCSVDataManager<登録キー型, データクラス>( "サブフォルダ", "CSVファイル名", SMFileLocation.Resource, 1 ),
          // サンプルのアイテムデータを登録
          new SMCSVDataManager<string, SampleItemData>( "", "SampleItem", SMFileLocation.Resource, 1 ),
        }
      },
    };
  }
}

```

データの取得には、これを呼びます。
```csharp
using SubmarineMirage.Service;
using SubmarineMirage.Data;

class Sample {
  void Example() {
    // 登録したデータクラスを取得します。
    var datas = SMServiceLocator.Resolve<SMAllDataManager>().Get<登録キー型, データクラス>()
    var data = datas.Get( 登録キー );

    // サンプルのアイテムデータを取得します。
    var itemDatas = SMServiceLocator.Resolve<SMAllDataManager>().Get<string, SampleItemData>()
    var data = itemDatas.Get( "パンフレット" );
  }
}
```



### イベント
フレームワーク内の各クラスでは、独自のイベント関数クラスを使用してます。  
Assets\SubmarineMirageFramework\Scripts\Core\Event、に存在します。  

- SMSubject  
Updateなどの更新処理で、頻繁に使用してます。  
UniRxのSubjectのラッパーで、先頭追加、末尾追加、登録解除を指定できます。
```csharp
using UniRx;
using SubmarineMirage.Event;
using SubmarineMirage.Debug;

class Sample {
  void Example() {
    // UniRxのSubjectのキューを作成
    var updateEvent = new SMSubject();

    // キューの最後に、関数追加
    updateEvent.AddLast().Subscribe( _ => {
      SMLog.Debug( "最後の更新処理。" );
    } );
    // キューの最初に、関数追加
    updateEvent.AddFirst()
      .Select( _ => 100 )  // UniRxのSelectやWhereなど、何でも指定できます
      .Subscribe( i => {
        SMLog.Debug( $"最初の更新処理 : {i}" );
      } );
    // キューの最後に、関数追加し、登録解除用のキーを設定
    updateEvent.AddLast( "削除キー" ).Subscribe( _ => {} );
    // キューから、登録関数を削除
    updateEvent.Remove( "削除キー" );

    // キュー内の関数を、順番に実行
    updateEvent.Run();

    // できれば解放する（しなくても、デストラクタで自動解放される）
    // 全ての内部Subjectは、OnCompletedが呼ばれ、解放される
    updateEvent.Dispose();
  }
}
```

- SMAsyncEvent  
非同期処理で、頻繁に使用してます。  
内部に、UniTaskを使用してます。  
```csharp
using Cysharp.Threading.Tasks;
using SubmarineMirage.Event;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;

class Sample {
  async UniTask Example() {
    // 非同期関数のキューを作成
    var asyncEvent = new SMAsyncEvent();

    // キューの最後に、関数追加
    asyncEvent.AddLast( async canceler => {
      await UTask.Delay( canceler, 1000 );
      SMLog.Debug( "1秒間、待機しました。" );
    } );
    // キューの最初に、関数追加
    asyncEvent.AddFirst( async canceler => {
      var isDone = true;
      await UTask.WaitWhile( canceler, () => !isDone );
      SMLog.Debug( "実行中の間、待機しました。" );
    } );
    // キューの最後に、関数追加し、登録解除用のキーを設定
    asyncEvent.AddLast( "削除キー", async canceler => {
      await UTask.DontWait();  // 何も待機しない
    } );
    // キューから、登録関数を削除
    asyncEvent.Remove( "削除キー" );

    // 非同期実行の停止クラスを作成
    var asyncCanceler = new SMAsyncCanceler();
    // 非同期関数のキューを、順番に実行
    await asyncEvent.Run( asyncCanceler );
    asyncCanceler.Cancel();  // 非同期実行中の場合、中断する

    // できれば解放する（しなくても、デストラクタで自動解放される）
    // 非同期実行中の場合、停止される
    asyncCanceler.Dispose();
    asyncEvent.Dispose();
  }
}
```

- SMDisposable  
デストラクタやDisposeの解放時に、頻繁に使用してます。  
```csharp
using SubmarineMirage.Event;
using SubmarineMirage.Debug;

class Sample {
  void Example() {
    // IDisposableのキューを作成
    var disposable = new SMDisposable();

    // キューの最後に、関数追加
    disposable.AddLast( () => {
      SMLog.Debug( "最後のオブジェクト解放。" );
    } );
    // キューの最初に、関数追加
    disposable.AddFirst( () => {
      SMLog.Debug( "最初のオブジェクト解放。" );
    } );

    // キュー内の関数を、順番に実行し、解放（しなくても、デストラクタで自動解放される）
    disposable.Dispose();
  }
}
```