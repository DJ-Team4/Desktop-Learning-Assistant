[TOC]  



# 概述

## 1. 命名空间

在使用任务管理的服务前，需要引入以下两个命名空间：

1. DesktopLearningAssistant.TomatoClock.Model
   + TaskInfo 类
   + Tomato 类
2. DesktopLearningAssistant.TomatoClock.SQLite
   + ITaskTomatoService 类
   + TaskTomatoService 类
   + TaskTomatoContext 类
   + TaskTomatoList 类

您需要关注的只有 Model 中的两个类和 ITaskTomatoService、TaskTomatoService 类。所有操作都通过 TaskToamtoService 类完成。  



## 2. 类

### 2.1 基本逻辑

> 我们将在数据库中建立两张表，一张用于存储所有设置任务的详情信息，另一张用于存储每个任务中的所有番茄钟的具体信息。两张表属于一对多关系，两表连接即可查询一个任务下的所有信息并能获得完成任务的时间起止信息。

### 2.2 类的说明

#### 2.2.1 TaskInfo

> 结构，用于封装任务信息相关变量组。

#### 2.2.2 Tomato

> 结构，用于封装番茄钟信息相关变量组。

#### 2.2.3 TaskTomatoList

> 实体类，EF CodeFirst 通过运行该类来创建数据库中所需的表及表间关系。

#### 2.2.4 TaskTomatoContext

> 建立数据库上下文。

#### 2.2.5 TaskTomatoService

> 数据服务类，单例类。您不需要关心此类，因为这个类用于对数据库进行操作，提供数据管理服务。

#### 2.2.6 ITaskTomatoService

> 服务类的接口，抽象了所有服务类提供的服务。**在使用前建议仔细阅读此类**。  



## 3. 图

### 3.1 UML 图

![TaskTomatoClassDiagram](/Users/sylvia/Desktop/DJ_Team/Desktop-Learning-Assistant/doc/pic/TaskTomatoClassDiagram.png)

### 3.2 实体类 E-R 图



# 实体类

## 1. TaskList

​	表示任务的实体类。

- ### 属性
  - #### TaskID

    任务的编号，主键，自动生成。

  - #### Name

    任务的名称。

  - #### StartTime

    任务的开始时间。

  - #### Deadline

    任务的截止时间。

  - #### Notes

    任务备注信息。

  - #### TomatoNum

    任务设定的番茄钟数量。

  - #### TomatoCount

    任务已经完成的番茄钟数量。

  - #### State

    任务完成状态。

  - #### TaskTomatoLists

    任务与番茄钟的一对多关系。

## 2. TaskTomatoList

​	表示番茄钟的实体类。

- ### 属性

  - #### TomatoID

    番茄钟的编号，主键，自动生成。

  - #### BeginTime

    番茄钟的开始时间。

  - #### EndTime

    番茄钟的结束时间。

  - #### TaskID

    番茄钟所属的任务的任务编号，外键。

  - #### TaskLists

    番茄钟与任务的多对一关系。



# 服务类

## 1. TaskTomatoService

### 1.1 对 TaskList 进行操作

#### 1.1.1 AddTask(TaskInfo taskInfo)

将用户键入的信息以 TaskInfo 结构存入 TaskList 表中。

#### 1.1.2 DeleteTask(int TaskID)

根据 TaskID 进行搜索，将该 ID 的任务删除。

#### 1.1.3 ModifyTask(TaskInfo taskInfo)

更改 ID 为 taskInfo.TaskID 的任务的信息。

#### 1.1.4 ReadTask(int TaskID)

根据 TaskID 来读取任务详情信息。

### 1.2 对 TaskTomatoLIst 进行操作

#### 1.2.1 AddTomatoStartTime(int iTaskID)

为正在进行的任务添加新的番茄钟的起始时间。

#### 1.2.3 AddTomatoEndTime(int iTaskID, int iTomatoID)

为正在进行的任务的番茄钟添加结束时间，并调用私有函数 AddTomatoNum() 判断是否要为对应任务更新番茄钟数量，调用私有函数GiveFilePath() 获取当前番茄钟内所有打开文件的信息。

#### 1.2.4 ReadTomato(int iTaskID)

提供 ID 为TaskID 的所有番茄钟信息（起止时间），返回 List\<Tomato\>。

#### 1.2.5 RecentTenApp(DateTime iTime)

提供时间节点 iTime 之前最新完成的十个任务的任务名，返回任务名列表 List\<string>

### 1.3 私有方法

#### 1.3.1 AddTomatoNum(DateTime iBeginTime, DateTime iEndTime, int iTomatoCount)

私有方法，您不需要关心此方法。在为当前番茄钟添加结束时间后，通过时间差判断是否完成此次番茄钟，若完成则为该任务番茄钟数加一，若未完成则保持番茄数不变。

#### 1.3.2 GetFilePath(DateTime iBeginTime, DateTime iEndTime, int iTaskID)

私有方法，您不需要关心此方法。在为当前番茄钟添加结束时间后，通过开始和结束两个时间节点在系统 Recent 文件夹中获取所给时间段段内的所有使用过的文件信息，将快捷方式路径解析为实际路径，并返回文件实际路径列表。



