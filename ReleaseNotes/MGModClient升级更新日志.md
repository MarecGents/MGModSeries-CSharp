# 发布链接
见MGMod和MGGTMod


# MGModClient v0.2.1.040102
## 新增
0. x
## 修复
1. 【功能】[配置编辑]：补齐 MGConfig 配置模型（对齐服务端 ConfigSettingType 全部字段），修复 F12 菜单触发写回时清空 config.json 的问题。✅
2. 【功能】[配置编辑]：JsonUtils.WriteAtomic 在目标文件不存在时改用 File.Move，修复首次写回失败。✅
3. 【功能】[配置编辑]：修复 config.json 损坏/缺失时写回默认空模型覆盖原文件的问题——读取失败时跳过写回。✅
## 变更
1. 【版本】版本号更新至 v0.2.1.040102。✅
## 优化
1. 【功能】[配置编辑]：读取到损坏 config.json 时输出警告日志（不再静默回退默认）。✅
## 移出
0. x


# MGModClient v0.2.0.040102
## 新增
1. 【功能】[客户端资源]-[通用加载器]：新增 ClientResourceLoader——检索 MGMod 与 MGGTMod 的 `bundles/resources/` 下全部 .bundle（含子文件夹递归），按资源类型分发加载。✅
2. 【功能】[客户端资源]-[弹挂布局]：RigLayoutInjector 改为类型处理器（TryInjectPrefab），支持 FG_Alpha/FG_RBAV 与 MG_Velocity_Systems 自定义弹挂布局注入。✅
3. 【功能】[客户端资源]-[目录约定]：客户端注入资源统一收归 `bundles/resources/`（对应 Unity Resources 系统，与 SPT bundle 链路隔离），按类型分子目录（rig 布局、Slots/语音预留）。✅
## 修复
0. x
## 变更
1. 【版本】版本号更新至 v0.2.0.040102。✅
## 优化
0. x
## 移出
0. x

