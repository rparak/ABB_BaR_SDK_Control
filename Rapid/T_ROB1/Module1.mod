MODULE Module1
    ! ## =========================================================================== ## 
    ! MIT License
    ! Copyright (c) 2021 Roman Parak
    ! Permission is hereby granted, free of charge, to any person obtaining a copy
    ! of this software and associated documentation files (the "Software"), to deal
    ! in the Software without restriction, including without limitation the rights
    ! to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ! copies of the Software, and to permit persons to whom the Software is
    ! furnished to do so, subject to the following conditions:
    ! The above copyright notice and this permission notice shall be included in all
    ! copies or substantial portions of the Software.
    ! THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ! IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ! FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ! AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ! LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ! OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ! SOFTWARE.
    ! ## =========================================================================== ## 
    ! Author   : Roman Parak
    ! Email    : Roman.Parak@outlook.com
    ! Github   : https://github.com/rparak
    ! File Name: T_ROB1/Module1.mod
    ! ## =========================================================================== ## 
    
    ! Structure - Joint positon
    RECORD joints_pos
        num joint_1;
        num joint_2;
        num joint_3;
        num joint_4;
        num joint_5;
        num joint_6;
    ENDRECORD
    ! Structure Read OPC UA - B&R Automation
    RECORD read_UA_BR
        joints_pos joint_pos;
        pos tcp_pos;
        num rx_ea;
        num ry_ea;
        num rz_ea;
        num actual_state;
        num joint_velocity;
        num joint_zone;
        num cartesian_velocity;
        num cartesian_zone;
        num cartesian_trajectoryT; 
    ENDRECORD
    ! Structure Write OPC UA - B&R Automation
    RECORD write_UA_BR
        num isConnect;
        num inPosition;
    ENDRECORD
    ! Initialization structures
    PERS read_UA_BR r_ua;
    PERS write_UA_BR w_ua;
    ! Home position
    CONST jointtarget Target_home:= [[ 0, 0, 0, 0, 90, 0],[ 0, 9E9, 9E9, 9E9, 9E9, 9E9]];
    ! Profinet SDK
    PERS robtarget Target_sdk;
    ! Position Home
    PERS bool home_pos;
    ! Joints parameters {J1 - J6}
    PERS num q1;
    PERS num q2;
    PERS num q3;
    PERS num q4;
    PERS num q5;
    PERS num q6; 
    ! Linear parameters {X - Z}
    PERS num pos_x;
    PERS num pos_y;
    PERS num pos_z;
    ! Rotary parameters {quaternion}
    PERS num rot_qa;
    PERS num rot_qb;
    PERS num rot_qc;
    PERS num rot_qd;
    ! Rotary parameters {euler angles}
    PERS num angle_rx;
    PERS num angle_ry;
    PERS num angle_rz;
    ! Zone & Speed Data {Joint}
    VAR speeddata vel_param_joint;
    VAR zonedata zone_param_joint;
    ! Zone & Speed Data {Cartesian}
    VAR speeddata vel_param_cartes;
    VAR zonedata zone_param_cartes;
    
    ! ################################## SDK -> Main {Cycle} ################################## !
    PROC main()
        ! Initialization ConfJ -> OFF and ConfL -> OFF
        !ConfJ \Off;
        !ConfL \Off;
        
        ! ############### Main State {Control ABB Robot} ############### !
        TEST r_ua.actual_state
            CASE 0:
                ! ******************** INITIALIZATION STATE ******************** !
                ! Move Home
                MoveAbsJ Target_home \NoEOffs, v200, fine, tool0\WObj:=wobj0;
                ! Set signal {Connected}
                w_ua.isConnect := 1;
            CASE 10:
                ! ********************  WAIT STATE ******************** !
                ! Reset signal
                w_ua.inPosition := 0;
                ! Set Signal - Home Position
                home_pos := FALSE;
            CASE 20:
                ! ******************** HOME STATE ******************** !
                ! Move Home
                MoveAbsJ Target_home \NoEOffs, v200, fine, tool0\WObj:=wobj0;
                ! Set signal {Position}
                w_ua.inPosition := 1;
            CASE 30:
                ! ******************** MOVE JOINT STATE - INITIALIZATION ******************** !
                ! Reset signal {Position}
                w_ua.inPosition := 0;
            CASE 31:
                ! ******************** MOVE JOINT STATE {EXECUTE} ******************** !
                ! Call function {Calculation SDK Joint}
                sdk_jointConfiguration;
                ! Call function {Calculation SDK Velocity/Zone}
                sdk_VZ_Param r_ua.joint_velocity, r_ua.joint_zone, vel_param_joint, zone_param_joint;
                ! Move
                MoveAbsJ [[q1, q2, q3, q4, q5, q6],[9E9,9E9,9E9,9E9,9E9,9E9]] \NoEOffs,vel_param_joint,zone_param_joint,tool0\WObj:=wobj0;
                ! Set signal {Position}
                w_ua.inPosition := 1;
            CASE 40:
                ! ******************** MOVE CARTESIAN STATE - INITIALIZATION ******************** !
                ! Reset signal {Position}
                w_ua.inPosition := 0;
            CASE 41:
                ! ******************** MOVE CARTESIAN STATE {EXECUTE} ******************** !
                ! Call function {Calculation SDK TCP & Euler Angles}
                sdk_linearConfiguration;
                sdk_rotaryEulConfiguration;
                ! Call function {Calculation SDK Velocity/Zone}
                sdk_VZ_Param r_ua.cartesian_velocity, r_ua.cartesian_zone, vel_param_cartes, zone_param_cartes;
                ! ***** Set Target ***** !
                ! TCP
                Target_sdk.trans  := [pos_x, pos_y, pos_z];
                ! Euler Angles
                Target_sdk.rot    := OrientZYX(angle_rx, angle_ry, angle_rz);
                
                ! ############### Move ############### !
                IF r_ua.cartesian_trajectoryT = 0 THEN
                    ! Joint
                    MoveJ Target_sdk,vel_param_cartes,zone_param_cartes,tool0\WObj:=wobj0;
                ELSEIF r_ua.cartesian_trajectoryT = 1 THEN
                    ! Linear
                    MoveL Target_sdk,vel_param_cartes,zone_param_cartes,tool0\WObj:=wobj0;
                ENDIF
                ! Set signal {Position}
                w_ua.inPosition := 1;
        ENDTEST
    ENDPROC
    
    ! ################################## SDK -> Configuration Joints {J1-J6} ################################## !
    PROC sdk_jointConfiguration()
        q1 := r_ua.joint_pos.joint_1;
        q2 := r_ua.joint_pos.joint_2;
        q3 := r_ua.joint_pos.joint_3;
        q4 := r_ua.joint_pos.joint_4;
        q5 := r_ua.joint_pos.joint_5;
        q6 := r_ua.joint_pos.joint_6;
    ENDPROC
    ! ################################## SDK -> Configuration Linear {X,Y,Z} ################################## !
    PROC sdk_linearConfiguration()
        pos_x := r_ua.tcp_pos.x;
        pos_y := r_ua.tcp_pos.y;
        pos_z := r_ua.tcp_pos.z;
    ENDPROC

    ! ################################## SDK -> Configuration Euler Angles {RX, RY, RZ} ################################## !
    PROC sdk_rotaryEulConfiguration()
        angle_rx := r_ua.rx_ea;
        angle_ry := r_ua.ry_ea;
        angle_rz := r_ua.rz_ea;
    ENDPROC

    ! ################################## SDK -> Configuration Joint/Cartesian Parameters ################################## !
    PROC sdk_VZ_Param(num prof_vel, num prof_zon, VAR speeddata velocity, VAR zonedata zone)
        ! ############### Velocity ############### !
        TEST prof_vel
            CASE 0:
                velocity := v5;
            CASE 1:
                velocity := v10;
            CASE 2:
                velocity := v20;
            CASE 3:
                velocity := v30;
            CASE 4:
                velocity := v40;
            CASE 5:
                velocity := v50;
            CASE 6:
                velocity := v60;
            CASE 7:
                velocity := v80;
            CASE 8:
                velocity := v100;
            CASE 9:
                velocity := v150;
            CASE 10:
                velocity := v200;
            CASE 11:
                velocity := v300;
            CASE 12:
                velocity := v400;
            CASE 13:
                velocity := v500;
            CASE 14:
                velocity := v600;
            CASE 15:
                velocity := v800;
            CASE 16:
                velocity := v1000;
            CASE 17:
                velocity := v1500;
            CASE 18:
                velocity := v2000;
            CASE 19:
                velocity := v2500;
            CASE 20:
                velocity := v3000;
            CASE 21:
                velocity := v4000;
            CASE 22:
                velocity := v5000;
            CASE 23:
                velocity := v6000;         
        ENDTEST
        
        ! ############### Zone ############### !
        TEST prof_zon
            CASE 0:
                zone := fine;
            CASE 1:
                zone := z0;
            CASE 2:
                zone := z1;
            CASE 3:
                zone := z5;
            CASE 4:
                zone := z10;
            CASE 5:
                zone := z15;
            CASE 6:
                zone := z20;
            CASE 7:
                zone := z30;
            CASE 8:
                zone := z40;
            CASE 9:
                zone := z50;
            CASE 10:
                zone := z60;
            CASE 11:
                zone := z80;
            CASE 12:
                zone := z100;
            CASE 13:
                zone := z150;
            CASE 14:
                zone := z200;       
        ENDTEST
        
    ENDPROC
ENDMODULE