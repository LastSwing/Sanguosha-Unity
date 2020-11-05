# Sanguosha-Unity

这是一个卡牌游戏三国杀的服务器端。（客户端用UNITY3D开发，暂不公布源码）  
截至2020年1月1日，共完成身份模式武将269名，国战模式武将102名，官渡模式武将26名
  
特性：  
1、游戏逻辑基于太阳神三国杀（QSanguosha），并进行了优化。并且所有游戏逻辑全部在服务器端计算，客户端仅处理操作。  
2、采用SQL server作为游戏数据和用户信息管理，可以更加方便的管理数据和进行开发，比如游戏成就、战功这样的功能。  
   同时也意味着如果想单机玩，就必须在自己的电脑上安全SQL server程序。  
3、支持多种游戏模式，目前已支持国战和身份模式，以及小场景官渡之战。  
4、网络传输使用SuperSocket和MsgPack，Json使用Newtonsoft  
  
Client DEMO video：https://www.bilibili.com/video/av58008252/

Client download link: https://pan.baidu.com/s/1uxK5_sIDTQVOg-BXPCX9_g 提取码:kc3b
