
TYPE
	abb_cartes_cmd : 	STRUCT 
		Write : abb_cartesian;
		SDK : abb_cartesian;
	END_STRUCT;
	abb_joint_cmd : 	STRUCT 
		Write : abb_joint;
		SDK : abb_joint;
	END_STRUCT;
	abb_m : 	STRUCT 
		Joint : abb_joint_cmd;
		Cartesian : abb_cartes_cmd;
		Command : abb_cmd_m;
		in_position : USINT;
		is_connected : USINT;
		state : USINT;
	END_STRUCT;
	abb_cmd : 	STRUCT 
		start : BOOL;
		stop : BOOL;
		default_parameters : BOOL;
		home : BOOL;
	END_STRUCT;
	abb_cmd_m : 	STRUCT 
		joint : abb_cmd;
		cartesian : abb_cmd;
	END_STRUCT;
	abb_cartesian : 	STRUCT 
		X : LREAL;
		Y : LREAL;
		Z : LREAL;
		RX : LREAL;
		RY : LREAL;
		RZ : LREAL;
		Velocity : USINT;
		Zone : USINT;
		Trajectory_type : USINT;
	END_STRUCT;
	abb_joint : 	STRUCT 
		J1 : LREAL;
		J2 : LREAL;
		J3 : LREAL;
		J4 : LREAL;
		J5 : LREAL;
		J6 : LREAL;
		Velocity : USINT;
		Zone : USINT;
	END_STRUCT;
END_TYPE
