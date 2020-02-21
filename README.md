# Submarine Mirage Framework for Unity
##### Version 0.1
![Logo.png](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/Assets/SubmarineMirageFrameworkForUnity/Textures/Logo.png?raw=true)  



## 概要
Submarine Mirage Framework for Unityとは、Unityでのゲーム開発時に、不便さを補い、安定、迅速、堅牢な実装を行う為の、フレームワークである。  
市販レベルのゲームで、一般的、汎用的、必須な機能を、全体的に実装している。  
現在開発中の為、[未実装機能](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/issues)が沢山あり、不安定である。  


#### 対象
+ 中規模インディーゲーム向け  
数名規模のインディーゲーム開発スタジオで、それなりの中規模ゲーム開発での使用を想定している。  
（ストーリーがある中編アクションRPG、小規模のオープンワールドゲーム等。）  
また、プログラム実装に悩んだ際に、処理の参考となる。  

+ 日本人向け  
Unity用の総合機能フレームワークは、オープンソースで多々あるが、プログラムのコメントが中国語や英語である事が多い。  
マニュアルを日本語化して読む程度ならまだしも、外国語のプログラムのコメントを見ながらの開発は辛いと思われる。  
そのような語学が堪能でない日本人にとって、扱い易いフレームワークである。  
逆に、語学堪能な日本人、外国人には、
[GameFramework](https://github.com/EllanJiang/GameFramework)、
[QFramework](https://github.com/liangxiegame/QFramework)、
[BDFramework.Core](https://github.com/yimengfan/BDFramework.Core)、
[KSFramework](https://github.com/mr-kelly/KSFramework)等のフレームワークの方が出来が良い為、そちらをお勧めする。  

+ 導入事例  
[夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)（インディーゲーム開発スタジオ）の開発環境に採用中である。  


#### 特徴
+ Unityのいい加減な機能を、補強  
Unityは、いい加減なゲーム開発をサポートし、市販ゲーム程度のきちんと動作するゲーム開発を、ほぼ全くサポートしない。  
Unityの問題ばかりの機能の内、ゲーム開発で頻繁に使われるであろう機能を、堅牢に、使い易く実装している。  

+ フレームワークとして、全面実装  
フレームワークの為、特定の機能実装だけでなく、多種多様な機能を全体的に実装している。  

+ 外部ライブラリ導入済の為、環境構築が簡単  
業界水準を鑑み、ゲーム開発企業で頻繁に使用される、多種多様な外部ライブラリを同封している。  
（ライセンス上、同封可能なライブラリのみ。）  
このフレームワークを導入するだけで、即、開発環境を構築可能である。  

+ 見易いプログラム  
可読性を意識し、（日本語であるが）見易いコメント、分かり易い設計を重視している。  
プログラムが見易いので、使い易いフレームワークである。  


#### 対応
+ Unity動作環境  
  + 2018.4.16f1～（以降）  
  動作確認は、2018.4.16f1を使用している。

+ ビルド対応  
  + Windows（IL2CPP含む）  
  + Android（IL2CPP含む）  



## 使用方法
現在、執筆中。  

<!--
+ 導入  
Assets/以下のファイルを全てコピー
UnityAds、UnityIAPを導入


+ デモ  
SubmarineMirageFrameworkForUnity\Sample\Scenes\Sample.unity

+ 使い方  
BaseProcessを使い、
MonoBehaiviorProcessを使い、
Singletonを使い、管理クラス作成
MainProcessに、登録


+ 狙い  
UniRx使用のリアクティブスパゲッティと、Processプログラムの比較を、画像等で表示
要は、UniRxだとゴチャゴチャなのは、コンポーネント設計が、全て等価だから
マネージャー系は、最初にまとめて順番通り処理、その後シーンコンポーネントを順不同で処理し、処理完了まで待ってから、フェードイン
Constractorが、生成直後に呼ばれ、
Load→Initializeを非同期実行し、
FixedUpdate→Update→LateUpdateを初期化完了後に毎回、それぞれのタイミングで呼び出し、
Finalizeを非同期実行を待機後、
オブジェクトが破棄されて、シーン遷移

Constractorで、マネージャーの参照を取得（この時すでにマネージャーは初期化済）
Loadで、自身のオブジェクトに必要なリソースのサーバー、ローカル読込
Initializeで、他オブジェクトの参照を入手（既に読込済なのでnullにならない）
全ての初期化が完了後、各種Update処理
Finalizeで、破棄に時間が掛かる処理を想定し、非同期破棄
全て破棄してから、シーン切り替え
-->


## 実装処理
現在、執筆中。  

<!--
Assets/SubmarineMirageFrameworkForUnityフォルダ内
Scriptsに、プログラム

・Main
Main/MainProcess.csから、プログラム開始

・Test
Test/にて、各種プログラムのテストコードが見れます。
Sample\Scenes\Sample.unityのScriptsオブジェクトに、貼られているのが、このプログラム

・System
System/にて、実プログラム

ネットワーク管理、時間管理のクラス



・Audio
プログラムのみで、音を鳴らせる
音楽、背景音、ジングル音、効果音、ループ効果音、声音を再生できる

・Build
ビルド時の、自動処理


・Data
データ読込関連処理

File/
ファイル読み書き
暗号化、CSV、画像、音、シリアライズ、文章、生情報を、サーバー、外部、リソース内から読み書き
キャッシュにも対応

Master/
マスターデータ
広告
課金
エラー
システム
アイテム情報を読込

AIの命令情報
構文解析情報も読み込む

Raw/
生データ
UnityのAudioSource、Texture、Sprite等を保存する為、生データに変換する

Save/
セーブデータ
遊戯、設定、サーバーキャッシュデータを、暗号化して読み書き

Server/
サーバーデータ
アプリ、製品版宣伝、他アプリ宣伝データを、指定URLからダウンロードし、CSVを読み込む


・Debug
デバッグ関連
Debug.Logに、ジャンル別色文字を付けた、拡張クラス
デバッグ文章を、アプリ画面に描画するクラス
FPS計測


・Extension
uGUI、nGUIの装飾文字を簡単に扱える拡張クラス
ゲームオブジェクト、部品の便利クラス
キャッシュ機能付き、MonoBehaviour拡張クラス
数学の便利クラス
入力の管理クラス
Unityのレイヤー、タグを使い易くしたクラス
音、真偽値、色、スプラッシュスクリーン待機、スプライト、テクスチャ、文字、タイプ、通信などの拡張クラス
ジェネリック、オブジェクト等の拡張、シリアライズで、ToDeepString、ToDeepCopy


・FSM
有限状態機械
コルーチンで駆動する、使い易いFSM

・Process
MonoBehaviourを介さず、Initialize、Update、Finalizeする処理クラス

BaseProcess.cs
ゲームオブジェクトにする意味が無いが、初期化、更新処理が欲しいクラスに、継承して使用

MonoBehaviourProcess.cs
全ゲームオブジェクトは、MonoBehaviourを使わず、このクラスを継承する
シーン初期化のManagerクラスの処理が終了後、まとめてLoad、Initialize（非同期の為、サーバー受信やロードに使用できる）が順番に呼ばれ、
初期化が完了してから、FixedUpdate、Update、LateUpdateを繰り返し呼び出し、
シーンが破棄される前に、Finalizeが、非同期で呼ばれる

CoroutineProcess.cs
UniRxのコルーチンを簡単に使える用にした
遅延再生、一時停止、自動解放を行える


・Singleton
Process処理を用いた、シングルトンのクラス
ゲームオブジェクトに張り付ける用と、貼り付けずにプログラムのみで使用できる用がある


・Utility
便利処理
ビルボード、CoroutineProcessの簡易呼出、DEVELOPのみ実行されるゲームオブジェクト、ゲームオブジェクト便利、階層便利、シリアライズ便利、MonoBehaviourProcessのテンプレート
-->





## 外部ライブラリ
+ 外部ライブラリの配置フォルダ  
[Assets/Plugins/ExternalAssets/](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/tree/master/Assets/Plugins/ExternalAssets)に、導入した外部ライブラリを配置している。  
（Plugins/以下に配置する理由は、.csprojを別アセンブリ化し、ゲーム本体プログラム変更の度に、外部プログラム全体を再ビルドしない為である。）  
主に、前職のゲーム開発企業で使用した、ライブラリを導入している。  


+ [DOTween](http://dotween.demigiant.com/index.php)  
これは、音、画像、移動、数値等、様々な処理に、トゥイーン動作を付けるライブラリである。  
様々なゲーム開発企業で使用される有名ライブラリの為、これに対応する事で、業界水準と鑑みている。  
当フレームワークでは、音のフェード処理や、UIの処理に使用している。  


+ [Json & MessagePack Serialization](https://github.com/deniszykov/msgpack-unity3d)  
これは、プログラムのクラスや構造体のデータを、文章化し、JSONやMessagePack形式に変換するライブラリである。  
Unity固有のクラスや構造体も文章化でき、IL2CPPビルド時に事前コード生成が要らず、特殊な属性を付ける必要も無く（除外属性は付ける事が可能）、扱い易い。  
ただし、クラスのポリモーフィズムに未対応の為、配列等で基底クラス読込の場合、継承先クラスのメンバが復元されない。  
当フレームワークでは、書類の読み書き（Save & Load）、クラス中身のデバッグ表示（ToDeepString）、クラスのコピー（DeepCopy）等に使用している。  


+ [KoganeUnityLib](https://github.com/baba-s/kogane-unity-lib)  
これは、C#やUnityの簡易記述を可能にする、拡張、便利クラスのライブラリである。  
当初、同じような便利プログラムの実装を検討していたが、車輪の再開発をする必要は無い為、ライブラリに依存した。  
当フレームワークでは、至る所で、便利記述に使用している。  


+ [Lunar Unity Mobile Console](https://github.com/SpaceMadness/lunar-unity-console)  
これは、スマートフォン端末で、画面にデバッグログを表示する為のライブラリである。  
uGUI装飾による色文字等は反映せず、余計な画像によりアプリサイズが大きくなる問題があり、他ライブラリと差し替えを検討している。  
当フレームワークでは、Androidのデバッグの為に使用している。  


+ [ReferenceViewer](https://github.com/anchan828/ReferenceViewer)  
これは、Unity内のアセットが何処で使われているか、全参照を表示するライブラリである。  
当フレームワークでは、アセットの整理整頓に使用している。  


+ [ScriptingDefineSymbolsEditor](https://github.com/kankikuchi/ScriptingDefineSymbolsEditor)  
これは、#シンボルをPlayerSettingsでプラットフォーム別に設定する手間を省き、簡単に定義できるライブラリである。  
当フレームワークでは、#DEVELOP等のシンボル定義に使用している。  


+ [UniRx](https://github.com/neuecc/UniRx)  
これは、ラムダ式等を用い、関数定義を簡略化し、複雑な処理の簡略化を行うライブラリである。  
様々なゲーム開発企業で使用される有名ライブラリの為、これに対応する事で、業界水準と鑑みている。  
当フレームワークでは、至る所で全面的に積極的に使用している。  


+ [UniTask](https://github.com/Cysharp/UniTask)  
これは、C#のasync/awaitの非同期処理を、Unityに対応させ、簡易記述ができるライブラリである。  
コルーチン、UniRxの非同期を経て、次世代の非同期処理として期待されているが、キャンセル処理に難がある為、使用が難しい。  
当フレームワークでは、可能な限り、async/await処理を使用している。  



## 不具合、実装予定
不具合の解決、新機能の実装予定等は、逐次[Issues](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/issues)に記載する。  
[夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)開発中の為、スタジオがゲーム公開の度に、次回作開発前に、毎回追加分をマージする予定である。  



## 由来
+ Submarine Mirage Framework for Unity  
Unity用の、海底の蜃気楼フレームワーク。  

+ Submarine  
  + 海底  
  [夢想海の水底より](https://seabed-of-reverie.wixsite.com/front)ゲーム開発に使用する為、海底とした。  

  + 潜水艦  
  スタジオの組員皆で乗って行く船として、潜水艦の意味も含む。  
  ロゴ画像から分かる通り、ボロボロの沈没船である。  
  社会の競争に負けた沈没船とも言える。  

+ Mirage  
  + 蜃気楼  
  完成可能性が低く、理想を追えども中々実現に至らない為、蜃気楼と名付けた。  
  （ファイナルファンタジー根性でもある。）  
  ゲームが未完成となる場合を鑑み、膨大な作業を水の泡と帰さない為、せめて開発環境を先に公開する。  
  似た悩みを持つインディーゲーム開発者の為、我が屍が道と成らんことを。



## 開発者
+ [夢想海の水底より（From Seabed of Reverie）](https://seabed-of-reverie.wixsite.com/front)  

+ [神呪歌人魚](https://twitter.com/SirenSingsCurse)  

+ 共同開発者を募集中  
万人が便利に使う為のフレームワークは、作家性等とは無関係に、作業分担可能と思われる。  
[Issues](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/issues)、
[掲示板](https://seabed-of-reverie.wixsite.com/front/notice-board)、
[お問い合わせ](https://seabed-of-reverie.wixsite.com/front/contact)等から連絡頂けると幸いである。  



## ライセンス
+ [MIT License](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE)  

+ 外部ライブラリ  
他人が開発した外部ライブラリを多数使用している為、それぞれ別々のライセンスが存在する。  
詳しくは、[LICENSE](https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE)書類を確認頂きたい。  