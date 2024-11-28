## SilkyUI Framework (丝滑的 UI 框架)

### 待办

1. [ ] Flex 布局功能完善
2. [ ] Grid 布局设计与实现
3. [ ] 圆角矩形绘制优化 (圆角矩形池)
4. [ ] RenderTarget2D 实现圆角裁切功能
5. [ ] 圆角矩形 渲染各边边框宽度支持
6. [ ] 圆角矩形 无模糊边框实现 (为了更精细的布局)
7. [ ] css - position: sticky 实现
8. [ ] flex wrap 实现
9. [ ] InlineBlock wrap 实现

#### Grid 布局元素定位方式

##### 类型

1. 默认
2. 有 Span
3. 指定位置

##### 思路

1. 先计算指定位置的, 逐步设置矩阵中的 boolean
2. if NxN can put Element
   1. true => put, list remove element, next grid -> `2`
   2. false => loop next element