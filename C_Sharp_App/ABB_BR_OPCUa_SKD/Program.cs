/* ------------------------------------------------------------------------------------------------------------------------ // 
// ------------------------------------------------------------------------------------------------------------------------ //
// ------------------------------------------------------------------------------------------------------------------------ //
MIT License

Copyright(c) 2020 Roman Parak

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

// ------------------------------------------------------------------------------------------------------------------------ //
// ------------------------------------------------------------------------------------------------------------------------ //
// ------------------------------------------------------------------------------------------------------------------------ //

Author   : Roman Parak
Email    : Roman.Parak @outlook.com
Github   : https://github.com/rparak
File Name: Program.cs

// ------------------------------------------------------------------------------------------------------------------------ //
// ------------------------------------------------------------------------------------------------------------------------ //
// ------------------------------------------------------------------------------------------------------------------------ */



// ------------------------------------------------------------------------------------------------------------------------//
// ----------------------------------------------------- LIBRARIES --------------------------------------------------------//
// ------------------------------------------------------------------------------------------------------------------------//

// -------------------- System -------------------- //
using System;
using System.Threading;
// -------------------- ABB Robotics -------------------- //
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
// -------------------- OPCUA -------------------- //
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace ABB_BR_OPCUa_SKD
{
    class Program
    {
        // -------------------- ApplicationConfiguration -------------------- //
        static ApplicationConfiguration client_configuration_r = new ApplicationConfiguration();
        static ApplicationConfiguration client_configuration_w = new ApplicationConfiguration();
        // -------------------- EndpointDescription -------------------- //
        static EndpointDescription client_end_point_r, client_end_point_w;
        // -------------------- Session -------------------- //
        static Session client_session_r, client_session_w;
        // -------------------- Thread -------------------- //
        static Thread opcua_client_r_Thread, opcua_client_w_Thread;
        static Thread abb_sdk_rw_Thread;
        // -------------------- Bool -------------------- //
        static bool opcua_c_r_while, opcua_c_w_while;
        static bool sdk_read_data_isOk = false;
        static bool stop_rapid = false;
        static bool abb_sdk_rw_while;
        static bool rapid_is_started = false;
        // -------------------- NodeId -------------------- //
        static NodeId node_read_opcua_act_state;
        static NodeId node_read_opcua_j1, node_read_opcua_j2, node_read_opcua_j3;
        static NodeId node_read_opcua_j4, node_read_opcua_j5, node_read_opcua_j6;
        static NodeId node_read_opcua_j_vel, node_read_opcua_j_zone;
        static NodeId node_read_opcua_c_x, node_read_opcua_c_y, node_read_opcua_c_z;
        static NodeId node_read_opcua_c_rx, node_read_opcua_c_ry, node_read_opcua_c_rz;
        static NodeId node_read_opcua_c_vel, node_read_opcua_c_zone, node_read_opcua_c_trajT;
        // -------------------- Float -------------------- //
        static float read_opcua_act_state;
        static float read_opcua_j1, read_opcua_j2, read_opcua_j3;
        static float read_opcua_j4, read_opcua_j5, read_opcua_j6;
        static float read_opcua_j_vel, read_opcua_j_zone;
        static float read_opcua_c_x, read_opcua_c_y, read_opcua_c_z;
        static float read_opcua_c_rx, read_opcua_c_ry, read_opcua_c_rz;
        static float read_opcua_c_vel, read_opcua_c_zone, read_opcua_c_trajT;
        // -------------------- String -------------------- //
        static string node_write_opcua_in_pos, node_write_opcua_in_com_is_ok;
        static string program_name;
        // -------------------- NetworkScanner -------------------- //
        static NetworkScanner abb_sdk_net_scanner;
        // -------------------- Controller -------------------- //
        static Controller abb_sdk_controller;
        // -------------------- Int -------------------- //
        static int index = 0;
        // -------------------- UserDefined -------------------- //
        static UserDefined sdk_rapid_ud_data_r;

        // -------------------------------------------------------------------------------------------------------------------//
        // -------------------------------------------------------- MAIN -----------------------------------------------------//
        // -------------------------------------------------------------------------------------------------------------------//

        static void Main(string[] args)
        {
            // ------------------------ Initialization { Console app. Write } ------------------------//
            Console.WriteLine("[INFO] RobotStudio SDK (Software development kit)");
            Console.WriteLine("[INFO] OPCUa Communication {B&R Auotmation PLC}");
            Console.WriteLine("[INFO] Author: Roman Parak");

            // ------------------------ Initialization { OPCUa Config.} ------------------------//
            // PLC IP Address
            string ip_adr_plc = "127.0.0.1";
            // PLC Port
            string port_adr_plc = "4840";
            // Write IP, PORT {PLC}
            Console.WriteLine("[INFO] B&R Automation (PLC: IP Address - {0}, Port - {1})", ip_adr_plc, port_adr_plc);

            // ------------------------ Main Block { Control of the PLC (B&R) } ------------------------//
            try
            {
                // Program name {Task}
                program_name = "Server_t";
                // Node {Read - Actual State}
                node_read_opcua_act_state = "ns=6;s=::" + program_name + ":abb_control.state";
                // Node {Read - Joint Pos.}
                node_read_opcua_j1 = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.J1";
                node_read_opcua_j2 = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.J2";
                node_read_opcua_j3 = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.J3";
                node_read_opcua_j4 = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.J4";
                node_read_opcua_j5 = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.J5";
                node_read_opcua_j6 = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.J6";
                // Node {Read - Joint Velocity/Zone}
                node_read_opcua_j_vel = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.Velocity";
                node_read_opcua_j_zone = "ns=6;s=::" + program_name + ":abb_control.Joint.SDK.Zone";
                // Node {Read - Cartesian}
                node_read_opcua_c_x = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.X";
                node_read_opcua_c_y = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.Y";
                node_read_opcua_c_z = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.Z";
                node_read_opcua_c_rx = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.RX";
                node_read_opcua_c_ry = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.RY";
                node_read_opcua_c_rz = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.RZ";
                // Node {Read - Joint Velocity/Zone/Trajectory Type}
                node_read_opcua_c_vel = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.Velocity";
                node_read_opcua_c_zone = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.Zone";
                node_read_opcua_c_trajT = "ns=6;s=::" + program_name + ":abb_control.Cartesian.SDK.Trajectory_type";
                // Node {Write - Communication/In Position}
                node_write_opcua_in_pos = "ns=6;s=::" + program_name + ":abb_control.in_position";
                node_write_opcua_in_com_is_ok = "ns=6;s=::" + program_name + ":abb_control.is_connected";

                while (true)
                {
                    Console.WriteLine("[INFO] Connect to RobotStudio ABB SDK (y/n):");
                    // Connect sdk - var
                    string connect_sdk = Convert.ToString(Console.ReadLine());

                    if (connect_sdk == "y")
                    {
                        // ------------------------ Threading Block { OPCUa Read Data } ------------------------//
                        opcua_c_r_while = true;

                        opcua_client_r_Thread = new Thread(() => OPCUa_r_thread_function(ip_adr_plc, port_adr_plc));
                        opcua_client_r_Thread.IsBackground = true;
                        opcua_client_r_Thread.Start();

                        // ------------------------ Threading Block { OPCUa Write Data } ------------------------//
                        opcua_c_w_while = true;

                        opcua_client_w_Thread = new Thread(() => OPCUa_w_thread_function(ip_adr_plc, port_adr_plc));
                        opcua_client_w_Thread.IsBackground = true;
                        opcua_client_w_Thread.Start();

                        // ------------------------ Threading Block { ABB SDK Read/Write Data } ------------------------//
                        abb_sdk_rw_while = true;

                        abb_sdk_rw_Thread = new Thread(() => ABB_SDK_rw_thread_function());
                        abb_sdk_rw_Thread.IsBackground = true;
                        abb_sdk_rw_Thread.Start();

                        Console.WriteLine("[INFO] Stop RobotStudio SDK ABB (y):");
                        // Stop sdk - var
                        string stop_rs = Convert.ToString(Console.ReadLine());

                        if (stop_rs == "y")
                        {
                            // stop rapid variable -> true {ABB_SDK_rw_data() while)
                            stop_rapid = true;
                            // stop write data to OPCUa
                            sdk_read_data_isOk = false;
                            // SDK Stop
                            abb_sdk_stop_rapid();
                            // application quit
                            break;
                        }
                    }
                    else if (connect_sdk == "n")
                    {
                        // application quit
                        break;
                    }

                    // Application quit
                    Application_Quit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Application quit
                Application_Quit();
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------//
        // -------------------------------------------------------- FUNCTIONS -----------------------------------------------------//
        // ------------------------------------------------------------------------------------------------------------------------//

        // -------------------- Abort Threading Blocks -------------------- //
        static void Application_Quit()
        {
            try
            {
                // OPCUa Read Data
                opcua_c_r_while = false;
                // OPCUa Write Data
                opcua_c_w_while = false;
                // ABB Robotstudio SDK -> Read/Write Data
                abb_sdk_rw_while = false;

                // Abort threading block {OPCUA -> read data}
                if (opcua_client_r_Thread.IsAlive == true)
                {
                    opcua_client_r_Thread.Abort();
                }

                // Abort threading block {OPCUA -> write data}
                if (opcua_client_w_Thread.IsAlive == true)
                {
                    opcua_client_w_Thread.Abort();
                }

                // Abort threading block {SDK -> read/write data}
                if (abb_sdk_rw_Thread.IsAlive == true)
                {
                    abb_sdk_rw_Thread.Abort();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // ------------------------ Threading Block { RobotStudio ABB SKD Read/Write Data } ------------------------//
        static void ABB_SDK_rw_thread_function()
        {
            // Initialization Network scanner
            abb_sdk_net_scanner = new NetworkScanner();
            // Scan controllers
            abb_sdk_net_scanner.Scan();
            // Add -> controllers
            ControllerInfoCollection abb_sdk_controller_info = abb_sdk_net_scanner.Controllers;

            // Find controller
            foreach (ControllerInfo info in abb_sdk_controller_info)
            {
                abb_sdk_controller = ControllerFactory.CreateFrom(info);

                // Login - First Controller
                if (index == 0)
                {
                    // Available
                    if (info.Availability == Availability.Available)
                    {
                        // Login: Default User
                        abb_sdk_controller.Logon(UserInfo.DefaultUser);

                        if (rapid_is_started == false)
                        {
                            // Rapid Start Communication {R/W data}
                            abb_sdk_start_rapid();

                            // is stared
                            rapid_is_started = true;
                        }

                        // Read/Write Data from RS SDK
                        ABB_SDK_rw_data();
                    }
                    break;
                }
                index++;
            }

        }

        // ------------------------ Main Function R/W {RobotStudio ABB SDK} ------------------------//
        static void ABB_SDK_rw_data()
        {
            // Main Cycle {Read/Write Data from RobotStudio ABB SDK}
            while (abb_sdk_rw_while)
            {
                // Stop command
                if (stop_rapid == false)
                {
                    // Writa data OPCUa -> SDK
                    abb_sdk_write_data();
                    // Read Data SKD -> OPCUa
                    abb_sdk_read_data();

                    // Thread sleep 10 ms
                    Thread.Sleep(10);
                }
            }
        }

        // ------------------------ Function SDK {RAPID program Start} ------------------------//
        static void abb_sdk_start_rapid()
        {
            try
            {
                if (abb_sdk_controller.OperatingMode == ControllerOperatingMode.Auto)
                {
                    using (Mastership m = Mastership.Request(abb_sdk_controller.Rapid))
                    {
                        // Start Rapid
                        abb_sdk_controller.Rapid.Start(true);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // ------------------------ Function SDK {RAPID program Stop} ------------------------//
        static void abb_sdk_stop_rapid()
        {
            try
            {
                if (abb_sdk_controller.OperatingMode == ControllerOperatingMode.Auto)
                {
                    using (Mastership m = Mastership.Request(abb_sdk_controller.Rapid))
                    {
                        // Stop Rapid
                        abb_sdk_controller.Rapid.Stop();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // ------------------------ Function SDK {Write data: OPCUa -> SDK} ------------------------//
        static void abb_sdk_write_data()
        {
            // Rapid Data -> program, module , name of the strucutre {r_ua}
            RapidData sdk_rapid_data_r = abb_sdk_controller.Rapid.GetRapidData("T_ROB1", "Module1", "r_ua");
            // Get data type -> own structure
            RapidDataType sdk_rapid_data_type_r = abb_sdk_controller.Rapid.GetRapidDataType("T_ROB1", "Module1", "r_ua");
            // User Defined structure
            UserDefined sdk_rapid_ud_data_r = new UserDefined(sdk_rapid_data_type_r);
            // Read data value
            //sdk_rapid_ud_data_r = (UserDefined)sdk_rapid_data_r.Value;

            // Declaration of the data: OPCUa -> SDK {strcture r_ua}
            // Joint {1 - 6}
            sdk_rapid_ud_data_r.Components[0].FillFromString("[" + read_opcua_j1.ToString() + ", " + read_opcua_j2.ToString() + ", " + read_opcua_j3.ToString() + ", " + read_opcua_j4.ToString() + ", " + read_opcua_j5.ToString() + ", " + read_opcua_j6.ToString() + "]");
            // TCP {X, Y, Z}
            sdk_rapid_ud_data_r.Components[1].FillFromString("[" + read_opcua_c_x.ToString() + ", " + read_opcua_c_y.ToString() + ", " + read_opcua_c_z.ToString() + "]");
            // Euler Angles
            // RX
            sdk_rapid_ud_data_r.Components[2].FillFromString(read_opcua_c_rx.ToString());
            // RY
            sdk_rapid_ud_data_r.Components[3].FillFromString(read_opcua_c_ry.ToString());
            // RZ
            sdk_rapid_ud_data_r.Components[4].FillFromString(read_opcua_c_rz.ToString());
            // Actual State {Automation Studio B&R}
            sdk_rapid_ud_data_r.Components[5].FillFromString(read_opcua_act_state.ToString());
            // Joint parameters {velocity, zone}
            sdk_rapid_ud_data_r.Components[6].FillFromString(read_opcua_j_vel.ToString());
            sdk_rapid_ud_data_r.Components[7].FillFromString(read_opcua_j_zone.ToString());
            // Cartesian Parameters {velocity, zone, trajectory type}
            sdk_rapid_ud_data_r.Components[8].FillFromString(read_opcua_c_vel.ToString());
            sdk_rapid_ud_data_r.Components[9].FillFromString(read_opcua_c_zone.ToString());
            sdk_rapid_ud_data_r.Components[10].FillFromString(read_opcua_c_trajT.ToString());

            // Write data to Master
            using (Mastership master = Mastership.Request(abb_sdk_controller.Rapid))
            {
                // Writing data -> ows structure
                sdk_rapid_data_r.Value = sdk_rapid_ud_data_r;
            }
        }

        // ------------------------ Function SDK {Read data: SDK -> OPCUa} ------------------------//
        static void abb_sdk_read_data()
        {
            // Rapid Data -> Program, Module, name of the structure {w_ua}
            RapidData sdk_rapid_data_r = abb_sdk_controller.Rapid.GetRapidData("T_ROB1", "Module1", "w_ua");
            // Get data type -> own structure
            RapidDataType sdk_rapid_data_type_r = abb_sdk_controller.Rapid.GetRapidDataType("T_ROB1", "Module1", "w_ua");
            // User Defined structure
            sdk_rapid_ud_data_r = new UserDefined(sdk_rapid_data_type_r);
            // Read data value
            sdk_rapid_ud_data_r = (UserDefined)sdk_rapid_data_r.Value;
            // Command -> reading data is okay
            sdk_read_data_isOk = true;
        }

        // ------------------------ Threading Block { OPCUa Read Data } ------------------------//
        static void OPCUa_r_thread_function(string ip_adr, string port_adr)
        {
            // OPCUa client configuration
            client_configuration_r = opcua_client_configuration();
            // Establishing communication
            client_end_point_r = CoreClientUtils.SelectEndpoint("opc.tcp://" + ip_adr + ":" + port_adr, useSecurity: false, operationTimeout: 10000);
            // Create session
            client_session_r = opcua_create_session(client_configuration_r, client_end_point_r);

            // Threading while {read data}
            while (opcua_c_r_while)
            {
                // Read Data - Float {Actual State}
                read_opcua_act_state = float.Parse(client_session_r.ReadValue(node_read_opcua_act_state).ToString());
                // Read Data - Float {Joint Pos.}
                read_opcua_j1 = float.Parse(client_session_r.ReadValue(node_read_opcua_j1).ToString());
                read_opcua_j2 = float.Parse(client_session_r.ReadValue(node_read_opcua_j2).ToString());
                read_opcua_j3 = float.Parse(client_session_r.ReadValue(node_read_opcua_j3).ToString());
                read_opcua_j4 = float.Parse(client_session_r.ReadValue(node_read_opcua_j4).ToString());
                read_opcua_j5 = float.Parse(client_session_r.ReadValue(node_read_opcua_j5).ToString());
                read_opcua_j6 = float.Parse(client_session_r.ReadValue(node_read_opcua_j6).ToString());
                // Read Data - Float {Joint Velocity/Zone}
                read_opcua_j_vel = float.Parse(client_session_r.ReadValue(node_read_opcua_j_vel).ToString());
                read_opcua_j_zone = float.Parse(client_session_r.ReadValue(node_read_opcua_j_zone).ToString());
                // Read Data - Float {Cartesian Velocity/Zone/Trajectory type}
                read_opcua_c_x = float.Parse(client_session_r.ReadValue(node_read_opcua_c_x).ToString());
                read_opcua_c_y = float.Parse(client_session_r.ReadValue(node_read_opcua_c_y).ToString());
                read_opcua_c_z = float.Parse(client_session_r.ReadValue(node_read_opcua_c_z).ToString());
                read_opcua_c_rx = float.Parse(client_session_r.ReadValue(node_read_opcua_c_rx).ToString());
                read_opcua_c_ry = float.Parse(client_session_r.ReadValue(node_read_opcua_c_ry).ToString());
                read_opcua_c_rz = float.Parse(client_session_r.ReadValue(node_read_opcua_c_rz).ToString());
                // Read Data - Float {Cartesian Velocity/Zone}
                read_opcua_c_vel = float.Parse(client_session_r.ReadValue(node_read_opcua_c_vel).ToString());
                read_opcua_c_zone = float.Parse(client_session_r.ReadValue(node_read_opcua_c_zone).ToString());
                read_opcua_c_trajT = float.Parse(client_session_r.ReadValue(node_read_opcua_c_trajT).ToString());
            }
        }

        // ------------------------ Threading Block { OPCUa Write Data } ------------------------//
        static void OPCUa_w_thread_function(string ip_adr, string port_adr)
        {
            // OPCUa client configuration
            client_configuration_w = opcua_client_configuration();
            // Establishing communication
            client_end_point_w = CoreClientUtils.SelectEndpoint("opc.tcp://" + ip_adr + ":" + port_adr, useSecurity: false, operationTimeout: 10000);
            // Create session
            client_session_w = opcua_create_session(client_configuration_w, client_end_point_w);

            // Threading while {write data}
            while (opcua_c_w_while)
            {
                // SDK read Data -> ok
                if (sdk_read_data_isOk == true)
                {
                    // Write Data
                    // Communication - is ok
                    opcua_write_value(client_session_w, node_write_opcua_in_com_is_ok, sdk_rapid_ud_data_r.Components[0].ToString());
                    // Robot -> In position
                    opcua_write_value(client_session_w, node_write_opcua_in_pos, sdk_rapid_ud_data_r.Components[1].ToString());
                }
            }
        }

        // ------------------------ OPCUa Client {Application -> Configuration (STEP 1)} ------------------------//
        static ApplicationConfiguration opcua_client_configuration()
        {
            // Configuration OPCUa Client {W/R -> Data}
            var config = new ApplicationConfiguration()
            {
                // Initialization (Name, Uri, etc.)
                ApplicationName = "OPCUa_AS", // OPCUa AS (Automation Studio B&R)
                ApplicationUri = Utils.Format(@"urn:{0}:OPCUa_AS", System.Net.Dns.GetHostName()),
                // Type -> Client
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    // Security Configuration - Certificate
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "OPCUa_AS", System.Net.Dns.GetHostName()) },
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                    AutoAcceptUntrustedCertificates = true,
                    AddAppCertToTrustedStore = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 10000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 50000 },
                TraceConfiguration = new TraceConfiguration()
            };
            config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
            }

            var application = new ApplicationInstance
            {
                ApplicationName = "OPCUa_AS",
                ApplicationType = ApplicationType.Client,
                ApplicationConfiguration = config
            };
            application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();

            return config;
        }

        // ------------------------ OPCUa Client {Application -> Create Session (STEP 2)} ------------------------//
        static Session opcua_create_session(ApplicationConfiguration client_configuration, EndpointDescription client_end_point)
        {
            return Session.Create(client_configuration, new ConfiguredEndpoint(null, client_end_point, EndpointConfiguration.Create(client_configuration)), false, "", 10000, null, null).GetAwaiter().GetResult();
        }

        // ------------------------ OPCUa Client {Write Value (Define - Node)} ------------------------//
        static bool opcua_write_value(Session client_session, string node_id, string value_write)
        {
            // Initialization
            NodeId init_node = NodeId.Parse(node_id);

            try
            {
                // Find Node (OPCUa Client)
                Node node = client_session.NodeCache.Find(init_node) as Node;
                DataValue init_data_value = client_session.ReadValue(node.NodeId);

                // Preparation data for writing
                WriteValue value = new WriteValue()
                {
                    NodeId = init_node,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(Convert.ChangeType(value_write, init_data_value.Value.GetType()))),
                };

                // Initialization (Write)
                WriteValueCollection init_write = new WriteValueCollection();
                // Append variable
                init_write.Add(value);

                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                // Wriate data
                client_session.Write(null, init_write, out results, out diagnosticInfos);

                // Check Result (Status)
                if (results[0] == StatusCodes.Good)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
    }
}
