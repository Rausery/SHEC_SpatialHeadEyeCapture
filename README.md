# SHEC_SpatialHeadEyeCapture
Unity project for collecting spatial head pose and eye tracking data using a mixed reality headset.
By running this program on the MR headset, you can obtain the subject's relative **head pose** and **eye-tracking data** with respect to the **spatial anchor**  
这是一个用MR头显采集大空间头部位姿和眼动数据的Unity项目，通过在MR头显上运行该程序，你可以获得佩戴者相对于**空间锚点**的**头部位姿**和**眼动数据**。

**Note:** Currently, this project is developed exclusively for **Pico 4**, with plans to port it to HoloLens 2.  
**注意：**目前，该项目仅针对**Pico 4**进行开发，有计划移植到HoloLens 2。

## Quick Start 快速上手👍

You can quickly experience the project by downloading and installing the [SHEC.apk](https://github.com/Rausery/SHEC_SpatialHeadEyeCapture/blob/main/SHEC.apk) package.  
你可以下载安装包[SHEC.apk](https://github.com/Rausery/SHEC_SpatialHeadEyeCapture/blob/main/SHEC.apk)快速体验

### Operating Instructions 操作方法

**1. Spatial Calibration 空间标定**  
Before starting the program, you need to calibrate using Pico's spatial calibration tool. The room size doesn't matter, but furniture calibration is essential. The first **corner** of the first **table** you calibrate will be considered the **spatial anchor point**, and all collected data will be relative to this anchor point.  
启动程序之前，你需要利用Pico系统的空间标定工具进行标定，房间范围无所谓，关键是家具标定，你标定的第一个**桌子**的第一个**角点**会被视为**空间锚点**，采集的所有数据都是相对于这个锚点的数据

**2. Spatial Anchor 空间锚点**⚓  
<div>
<img src="https://github.com/Rausery/SHEC_SpatialHeadEyeCapture/assets/116069411/2465ec5e-398e-4937-b6d4-1207188bd6ea" alt="image" style="width:15%;" />
</div>

The spatial anchor will be loaded within 2 seconds after the program starts, with red representing the X-axis, blue representing the Y-axis, and green representing the Z-axis. It is recommended to choose easily recognizable features on the environmental plane as the spatial anchor.  
空间锚点会在程序启动后的2s内加载完毕，红色为X轴，蓝色为Y轴，绿色为Z轴  
建议选择环境平面上易识别的特征点作为空间锚点

**3. Formal Data Collection 正式采集**  
Right-hand controller button A: Start collection  
Right-hand controller button B: End collection  
右侧手柄A键：开始采集  
右侧手柄B键：结束采集  

**4. Data Structure 数据结构**  
Data path: "PICO 4 Pro\Internal Shared Storage\Android\data\com.TsinghuaUniv.SHEC\files"  
Data is stored in JSON format. For data structure details, see [Assets/Scripts/Data/DataStructure.cs](https://github.com/Rausery/SHEC_SpatialHeadEyeCapture/blob/main/Assets/Scripts/Data/DataStructure.cs)  
数据路径"PICO 4 Pro\内部共享存储空间\Android\data\com.TsinghuaUniv.SHEC\files"  
数据以json格式存储，数据结构详见[Assets/Scripts/Data/DataStructure.cs](https://github.com/Rausery/SHEC_SpatialHeadEyeCapture/blob/main/Assets/Scripts/Data/DataStructure.cs)

**5. Data Types 数据类型**  
| 数据类型              | 数据名                          | 详解                                       | 支持设备                                 |
|----------------------|--------------------------------|-------------------------------------------|----------------------------------------|
| 三维坐标              | Position                       | 头部坐标                                   | Pico 4/ Pico 4 pro/ Pico 4 enterprise   |
| 单位向量              | Direction                      | 头部朝向                                   | Pico 4/ Pico 4 pro/ Pico 4 enterprise   |
| 三维坐标              | ET_Position                    | 双眼概括模型坐标                           | Pico 4 pro/ Pico 4 enterprise           |
| 单位向量              | ET_Orientation                 | 双眼概括模型目视方向                     | Pico 4 pro/ Pico 4 enterprise           |
| 三维坐标              | ET_ScreenPosition              | (x,y) 表示注视点在录屏画面上的坐标，左下角为原点, z为眼动小球与摄像机的距离，无意义，请忽略     | Pico 4 pro/ Pico 4 enterprise           |
| 浮点值                | LeftEyePupilDiameter/RightEyePupilDiameter | 左右眼瞳孔直径              | Pico 4 enterprise                       |
| 布尔值或浮点值        | LeftEyeOpenness/RightEyeOpenness | 左右眼开合情况（布尔: pro）左右眼眼睑开合度（浮点：enterprise） | Pico 4 pro/ Pico 4 enterprise   |

## Custom Development 二次开发

The project is developed using Unity, and this code repository includes all necessary files to create a Unity project. You can build upon this project for further development.  
项目基于Unity开发，本代码库已包含创建一个Unity项目所需的所有必要文件，你可以在本项目基础上进行二次开发。
