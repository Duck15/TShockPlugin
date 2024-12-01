# DwTP 定位传送插件

- 作者: 羽学
- 出处: 无
- 这是一个Tshock服务器插件，主要用于：使用指令定位传送微光湖、地牢、神庙、世纪之花花苞、BOSS宝藏袋

## 更新日志

```
v1.0.0
使用/dw指令传送到定位地标
定位花苞：只在世界图格上存在花苞时才会传送
定位地牢：只在世界存在地牢老人或邪教徒弓箭手时才会传送
定位神庙：神庙门没打开时传送到门前，打开后传送到丛林蜥蜴祭坛
定位宝藏袋：只在有BOSS死亡后才会传送到其死亡地点
定位微光湖：传送到微光液体，第一次会判断液体上方没有方块时放置灰砖
```

## 指令

| 语法                             | 别名  |       权限       |                   说明                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /dw  | /定位 |   dw.use    |    指令菜单    |
| /dw hb | /定位 花苞 |   dw.use    |    传送到世纪之花花苞    |
| /dw dl | /定位 地牢 |   dw.use    |    传送到地牢老人或邪教徒弓箭手    |
| /dw sm | /定位 神庙 |   dw.use    |    传送到神庙门前或丛林蜥蜴祭坛    |
| /dw bag | /定位 宝藏袋 |   dw.use    |    传送到BOSS死亡地点(宝藏袋)    |
| /dw wgh | /定位 微光湖 |   dw.use    |    传送到微光湖(并在液体上方为空时放置灰砖)    |


## 配置
```json
暂无
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love