# LZ4ForUnity
LZ4压缩在Unity上的应用

## 100次解压成绩比较

类型 | 解压时间 | 大小|GC内存分配
---|---|---|---
LZ4HC | 805.32ms|435KB|500.0 MB
LZ4 | 959.12ms |487KB|500.0 MB
PKZIP|4581.51ms|253KB|432.1 MB

## LZ4 Editor 工具介绍
- 在Project项目下选择文件，点击右键即可弹出LZ4相关转换操作
- 一键压缩一个或多个文件为LZ4
- 一键解压LZ4文件
- 一键转换PKZIP压缩到LZ4压缩