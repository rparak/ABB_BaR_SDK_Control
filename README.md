# Control of the ABB robot through B&R Automation PLC - ABB's PC Software Development Kit (PC SDK)

## Requirements:

**Software:**
```bash
ABB RobotStudio, B&R Automation Studio
```
ABB RS: https://new.abb.com/products/robotics/robotstudio/downloads

ABB Developer Centre: https://developercenter.robotstudio.com

B&R Automation: https://www.br-automation.com/en/downloads/#categories=Software-1344987434933

## Project Description:

The project is focused on a simple demonstration of client / server communication via OPC UA, which is implemented in C# Console App. (Server - B&R Automation PLC, Client - C# Console App). The console application communicates with the ABB robot via the PC Software Development Kit (PC SDK) from ABB. The application uses performance optimization using multi-threaded programming.

**Notes:**

ABB's PC Software Development Kit (PC SDK) is a software tool, which enables programmers to develop customized operator interfaces for the robot controller.

Example of a simple data processing application (OPC UA):

[OPC UA B&R Automation - Data Processing](https://github.com/rparak/OPCUA_Simple)

Example of a simple data processing application (Robot Web Services):

[ABB Robot - Data Processing](https://github.com/rparak/ABB_Robot_data_processing/)

**MappView (HMI):**
```bash
Simulation Address
PLC_ADDRESS = localhost or 127.0.0.1

http://PLC_ADDRESS:81/index.html?visuId=abb_move
```

The project was realized at Institute of Automation and Computer Science, Brno University of Technology, Faculty of Mechanical Engineering (NETME Centre - Cybernetics and Robotics Division).

<p align="center">
  <img src="https://github.com/rparak/ABB_BaR_SDK_Control/blob/main/images/sdk_diagram.png" width="800" height="450">
</p>


## Project Hierarchy:

**Repositary [/ABB_BaR_SDK_Control/]:**

```bash
[] /.../
```

## Application:

## ABB RobotStudio:

<p align="center">
  <img src="https://github.com/rparak/ABB_BaR_SDK_Control/blob/main/images/abb_app.PNG" width="800" height="450">
</p>

## HMI (Human-Machine Interface) - MappView:

## Result:

Youtube: ...

## Contact Info:
Roman.Parak@outlook.com

## License
[MIT](https://choosealicense.com/licenses/mit/)
