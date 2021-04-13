/* ********************************************************************************************************************** //
// ---------------------------------------------------------------------------------------------------------------------- //
// ********************************************************************************************************************** //

MIT License

Copyright(c) 2020 Roman Parak

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files(the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions :

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

// ********************************************************************************************************************** //
// ---------------------------------------------------------------------------------------------------------------------- //
// ********************************************************************************************************************** //

Author    : Roman Parak
Email     : Roman.Parak @outlook.com
Github    : https://github.com/rparak
File Name : Main.c

// ********************************************************************************************************************** //
// ---------------------------------------------------------------------------------------------------------------------- //
// ********************************************************************************************************************** */


// ************************************************************************************************************************************ //
// -------------------------------------------------------------  LIBRARIES ----------------------------------------------------------- //
// ************************************************************************************************************************************ //
#include <bur/plctypes.h>

#ifdef _DEFAULT_INCLUDES
	#include <AsDefault.h>
#endif

// ************************************************************************************************************************************ //
// -------------------------------------------------------------  DECLARATIONS -------------------------------------------------------- //
// ************************************************************************************************************************************ //
// ABB Control // STRUCT // 
_LOCAL struct abb_m abb_control;

// ************************************************************************************************************************************ //
// -------------------------------------------------------------  INITIALIZATION ------------------------------------------------------ //
// ************************************************************************************************************************************ //
void _INIT ProgramInit(void)
{
	// -------------------- ABB CONTROL {Main} -------------------- //
	abb_control.in_position = 0;
	abb_control.is_connected = 0;
	abb_control.state = 0;
	// -------------------- ABB CONTROL {Command - Joint} -------------------- //
	abb_control.Command.joint.start = 0;
	abb_control.Command.joint.stop = 0;
	abb_control.Command.joint.default_parameters = 0;
	abb_control.Command.joint.home = 0;
	// -------------------- ABB CONTROL {Joint - Position (WRITE)} -------------------- //
	// Write MappView
	abb_control.Joint.Write.J1 = 0;
	abb_control.Joint.Write.J2 = 0;
	abb_control.Joint.Write.J3 = 0;
	abb_control.Joint.Write.J4 = 0;
	abb_control.Joint.Write.J5 = 0;
	abb_control.Joint.Write.J6 = 0;
	abb_control.Joint.Write.Velocity = 0;
	abb_control.Joint.Write.Zone     = 0;
	// Write SDK {ABB}
	abb_control.Joint.SDK.J1 = 0;
	abb_control.Joint.SDK.J2 = 0;
	abb_control.Joint.SDK.J3 = 0;
	abb_control.Joint.SDK.J4 = 0;
	abb_control.Joint.SDK.J5 = 0;
	abb_control.Joint.SDK.J6 = 0;
	abb_control.Joint.SDK.Velocity = 0;
	abb_control.Joint.SDK.Zone     = 0;
	// -------------------- ABB CONTROL {Command - Cartesian} -------------------- //
	abb_control.Command.cartesian.start = 0;
	abb_control.Command.cartesian.stop = 0;
	abb_control.Command.cartesian.default_parameters = 0;
	abb_control.Command.cartesian.home = 0;
	// -------------------- ABB CONTROL {Cartesian - Position (WRITE)} -------------------- //
	// Write MappView
	abb_control.Cartesian.Write.X = 0;
	abb_control.Cartesian.Write.Y = 0;
	abb_control.Cartesian.Write.Z = 0;
	abb_control.Cartesian.Write.RX = 0;
	abb_control.Cartesian.Write.RY = 0;
	abb_control.Cartesian.Write.RZ = 0;
	abb_control.Cartesian.Write.Velocity = 0;
	abb_control.Cartesian.Write.Zone = 0;
	abb_control.Cartesian.Write.Trajectory_type = 0;
	// Write SDK {ABB}
	abb_control.Cartesian.SDK.X = 0;
	abb_control.Cartesian.SDK.Y = 0;
	abb_control.Cartesian.SDK.Z = 0;
	abb_control.Cartesian.SDK.RX = 0;
	abb_control.Cartesian.SDK.RY = 0;
	abb_control.Cartesian.SDK.RZ = 0;
	abb_control.Cartesian.SDK.Velocity = 0;
	abb_control.Cartesian.SDK.Zone = 0;
	abb_control.Cartesian.SDK.Trajectory_type = 0;
}

// ************************************************************************************************************************************ //
// ----------------------------------------------------------------  MAIN ------------------------------------------------------------- //
// ************************************************************************************************************************************ //
void _CYCLIC ProgramCyclic(void)
{
	
	// *********************************************************//
	// ---------------- ABB MAIN STATE  ----------------------- //
	// *********************************************************//
	
	switch(abb_control.state){
		case 0:
			{
				/*************************************** INITIALIZATION STATE ******************************************/
				abb_control.state = 0;
			
				// Check communication
				if(abb_control.is_connected == 1){
					abb_control.state = 10;
				}
			}
			break;
		case 10:
			{
				/*************************************** WAIT STATE ******************************************/
				abb_control.state = 10;
				// -------------------- START CONTROL {JOINT} -------------------- //
				if(abb_control.Command.joint.start == 1){
					abb_control.state = 31;
				}
				// -------------------- START CONTROL {CARTESIAN} -------------------- //
				if(abb_control.Command.cartesian.start == 1){
					abb_control.state = 41;
				}
				// -------------------- HOME POSITION -------------------- //
				if(abb_control.Command.cartesian.home == 1 || abb_control.Command.joint.home == 1){
					abb_control.state = 20;
				}
			}
			break;
		case 20:
			{
				/*************************************** HOME STATE ******************************************/
				abb_control.state = 20;
			
				// Check in position state
				if(abb_control.in_position == 1){
					abb_control.state = 10;
				}
			}
			break;
		case 30:
			{
				/*************************************** MOVE JOINT STATE (1) ******************************************/
				abb_control.state = 30;
			
				// Start Move {Joint}
				if(abb_control.Command.joint.start == 1){
					abb_control.state = 31;
				}else if(abb_control.Command.joint.stop == 1){
					abb_control.state = 10;
				}
			}
			break;
		case 31:
			{
				/*************************************** MOVE JOINT STATE (2) ******************************************/
				abb_control.state = 31;
			
				// Send data {MappView -> SDK}
				abb_control.Joint.SDK.J1 = abb_control.Joint.Write.J1;
				abb_control.Joint.SDK.J2 = abb_control.Joint.Write.J2;
				abb_control.Joint.SDK.J3 = abb_control.Joint.Write.J3;
				abb_control.Joint.SDK.J4 = abb_control.Joint.Write.J4;
				abb_control.Joint.SDK.J5 = abb_control.Joint.Write.J5;
				abb_control.Joint.SDK.J6 = abb_control.Joint.Write.J6;
				abb_control.Joint.SDK.Velocity = abb_control.Joint.Write.Velocity;
				abb_control.Joint.SDK.Zone     = abb_control.Joint.Write.Zone;
			
				// Check in position state
				if(abb_control.in_position == 1){
					abb_control.state = 30;
				}
			}
			break;
		case 40:
			{
				/*************************************** MOVE CARTESIAN STATE (1) ******************************************/
				abb_control.state = 40;
			
				// Start Move {Cartesian}
				if(abb_control.Command.cartesian.start == 1){
					abb_control.state = 41;
				}else if(abb_control.Command.cartesian.stop == 1){
					abb_control.state = 10;
				}
			}
			break;
		case 41:
			{
				/*************************************** MOVE CARTESIAN STATE (2) ******************************************/
				abb_control.state = 41;
			
				// Send data {MappView -> SDK}
				abb_control.Cartesian.SDK.X = abb_control.Cartesian.Write.X;
				abb_control.Cartesian.SDK.Y = abb_control.Cartesian.Write.Y;
				abb_control.Cartesian.SDK.Z = abb_control.Cartesian.Write.Z;
				abb_control.Cartesian.SDK.RX = abb_control.Cartesian.Write.RX;
				abb_control.Cartesian.SDK.RY = abb_control.Cartesian.Write.RY;
				abb_control.Cartesian.SDK.RZ = abb_control.Cartesian.Write.RZ;
				abb_control.Cartesian.SDK.Velocity = abb_control.Cartesian.Write.Velocity;
				abb_control.Cartesian.SDK.Zone = abb_control.Cartesian.Write.Zone;
				abb_control.Cartesian.SDK.Trajectory_type = abb_control.Cartesian.Write.Trajectory_type;
			
				// Check in position state
				if(abb_control.in_position == 1){
					abb_control.state = 40;
				}
			}
			break;
	}
	
	// -------------------- DEFAULT PARAM. {JOINT} -------------------- //
	if(abb_control.Command.joint.default_parameters == 1){
		abb_control.Joint.Write.J1 = 0;
		abb_control.Joint.Write.J2 = 0;
		abb_control.Joint.Write.J3 = 0;
		abb_control.Joint.Write.J4 = 0;
		abb_control.Joint.Write.J5 = 90;
		abb_control.Joint.Write.J6 = 0;
	}
	// -------------------- DEFAULT PARAM. {CARTESIAN	} -------------------- //
	if(abb_control.Command.cartesian.default_parameters == 1){
		abb_control.Cartesian.Write.X = 302;
		abb_control.Cartesian.Write.Y = 0;
		abb_control.Cartesian.Write.Z = 558;
		abb_control.Cartesian.Write.RX = 180;
		abb_control.Cartesian.Write.RY = 0;
		abb_control.Cartesian.Write.RZ = 180;
	}
}

