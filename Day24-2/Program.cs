using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

// turrible
// heuristic solution; no apparent algorithm
namespace Day24_2
{
    internal enum GateType
    {
        And = 0,
        Or,
        Xor,
    }

    internal enum GateState
    {
        Waiting = 0,
        Ready,
        Executed,
    }

    internal class Gate
    {
        public GateType Type;
        public int Id = -1;
        public string[] WiresIn;
        public string WireOut;
        public int Bit = -1;
        private int _Value = -1;
        public bool Verified = false;

        public Gate(int gate_id, GateType type, string wire_in1, string wire_in2, string wire_out)
        {
            Id = gate_id;
            Type = type;

            WiresIn = new string[2];
            WiresIn[0] = wire_in1;
            WiresIn[1] = wire_in2;
            WireOut = wire_out;
        }

        public GateState GetState()
        {
            if (_Value != -1) return GateState.Executed;

            if (Circuit.GetWireSignal(WiresIn[0]) == -1 || Circuit.GetWireSignal(WiresIn[1]) == -1)
                return GateState.Waiting;

            return GateState.Ready;
        }

        public void Execute()
        {
            if (_Value != -1) return;

            switch (Type)
            {
                case GateType.And:
                    _Value = Circuit.GetWireSignal(WiresIn[0]) & Circuit.GetWireSignal(WiresIn[1]);
                    break;
                case GateType.Or:
                    _Value = Circuit.GetWireSignal(WiresIn[0]) | Circuit.GetWireSignal(WiresIn[1]);
                    break;
                case GateType.Xor:
                    _Value = Circuit.GetWireSignal(WiresIn[0]) ^ Circuit.GetWireSignal(WiresIn[1]);
                    break;
                default:
                    throw new NotImplementedException();
            }

            Circuit.SendWireSignal(WireOut, _Value);
        }         
    }

    internal static class Circuit
    {
        private static Dictionary<string, int> _Wires = new();
        private static Dictionary<int, Gate> _Gates = new();
        public const int MaxInBits = 45;
        public const int MaxOutBits = 46;

        public static void OverrideX(long x_value)
        {
            string binstr = Convert.ToString(x_value, 2);

            char[] digits = binstr.ToCharArray();
            Array.Reverse(digits);

            for (int i = 0; i < MaxInBits; i++)
            {
                if (i < digits.Length)
                    _Wires["x" + i.ToString("00")] = (digits[i] == '1') ? 1 : 0;
                else
                    _Wires["x" + i.ToString("00")] = 0;
            }
        }

        public static void OverrideY(long y_value)
        {
            string binstr = Convert.ToString(y_value, 2);

            char[] digits = binstr.ToCharArray();
            Array.Reverse(digits);

            for (int i = 0; i < MaxInBits; i++)
            {
                if (i < digits.Length)
                    _Wires["y" + i.ToString("00")] = (digits[i] == '1') ? 1 : 0;
                else
                    _Wires["y" + i.ToString("00")] = 0;
            }
        }

        public static void AddWire(string name)
        {
            if (!_Wires.ContainsKey(name))
                _Wires.Add(name, -1);
        }

        public static void AddWire(string name, int value)
        {
            if (!_Wires.ContainsKey(name))
                _Wires.Add(name, value);
            else
                _Wires[name] = value;
        }

        public static void AddGate(int gate_id, GateType gate_type, string wire_in1, string wire_in2, string wire_out)
        {
            _Gates.Add(gate_id, new Gate(gate_id, gate_type, wire_in1, wire_in2, wire_out));
        }

        public static int GetWireSignal(string wire_name)
        {
            return _Wires[wire_name];
        }

        public static void SendWireSignal(string wire_name, int value)
        {
            _Wires[wire_name] = value;
        }

        public static Dictionary<int, Gate> GetGates()
        {
            return _Gates;
        }

        public static List<Gate> GetGatesByInputWire(string input_wire)
        {
            List<Gate> gates = new();

            foreach (int gid in _Gates.Keys)
            {
                if (_Gates[gid].WiresIn[0] == input_wire || _Gates[gid].WiresIn[1] == input_wire)
                    gates.Add(_Gates[gid]);
            }

            return gates;
        }

        public static List<int> GetGateIdsForXYRegisters(int xy_reg)
        {
            return GetGateIdsByInputWires("x" + xy_reg.ToString("00"), "y" + xy_reg.ToString("00"));
        }

        public static List<int> GetGateIdsByInputWires(string input1, string input2)
        {
            List<int> gate_ids = new();

            foreach (int gid in _Gates.Keys)
            {
                if ((_Gates[gid].WiresIn[0] == input1 && _Gates[gid].WiresIn[1] == input2) ||
                    (_Gates[gid].WiresIn[0] == input2 && _Gates[gid].WiresIn[1] == input1))

                    gate_ids.Add(gid);
            }

            return gate_ids;
        }

        public static int GetGateIdForZRegister(int z_reg)
        {
            var gate = GetGateByOutputWire("z" + z_reg.ToString("00"));

            if (gate == null)
                return -1;

            return gate.Id;
        }

        public static Gate? GetGateByOutputWire(string output_wire)
        {
            foreach (int gid in _Gates.Keys)
            {
                if (_Gates[gid].WireOut == output_wire)
                    return _Gates[gid];
            }

            return null;
        }

        
        public static Gate GetGate(int gate_id)
        {
            return _Gates[gate_id];
        }

        public static void SwitchOutputWires(string wire1, string wire2)
        {
            List<Gate> gates_with_wire1 = GetGatesByInputWire(wire1);
            List<Gate> gates_with_wire2 = GetGatesByInputWire(wire2);

            foreach (Gate g in gates_with_wire1)
            {
                if (g.WiresIn[0] == wire1)
                    g.WiresIn[0] = wire2;
                else if (g.WiresIn[1] == wire1)
                    g.WiresIn[1] = wire2;
            }

            foreach (Gate g in gates_with_wire2)
            {
                if (g.WiresIn[0] == wire2)
                    g.WiresIn[0] = wire1;
                else if (g.WiresIn[1] == wire2)
                    g.WiresIn[1] = wire1;
            }
        }

        public static void Exec()
        {
            while (true)
            {
                int ready_gates = 0;

                foreach (int gid in _Gates.Keys)
                {
                    if (_Gates[gid].GetState() == GateState.Ready)
                    {
                        ready_gates++;
                        _Gates[gid].Execute();
                    }
                }

                if (ready_gates == 0)
                    break;
            }
        }

        private static string GetZBits()
        {
            StringBuilder sbz = new();
            int bit_count = 0;

            while (true)
            {
                try
                {
                    int wire_value = _Wires["z" + bit_count.ToString("00")];
                    sbz.Append(wire_value.ToString("0"));
                    bit_count++;
                }
                catch
                {
                    break;
                }                
            }

            char[] z_array = sbz.ToString().ToCharArray();
            Array.Reverse(z_array);
            return new string(z_array);
        }

        public static long GetZReg()
        {
            string z_reg = Circuit.GetZBits();
            return Convert.ToInt64(z_reg, 2);
        }

        public static bool Validate(int gate_id)
        {
            switch (_Gates[gate_id].Type)
            {
                case GateType.And:
                    if ((_Gates[gate_id].WiresIn[0] == "x00" && _Gates[gate_id].WiresIn[1] == "y00") ||
                        (_Gates[gate_id].WiresIn[0] == "y00" && _Gates[gate_id].WiresIn[1] == "x00"))
                    {
                        return true;
                    }
                    else
                    { 
                        List<Gate> and_gate_output_gates = GetGatesByInputWire(_Gates[gate_id].WireOut);

                        if (and_gate_output_gates.Count != 1)
                            return false;

                        foreach (Gate g in and_gate_output_gates)
                        {
                            if (g.Type != GateType.Or) 
                                return false;
                        }
                    }
                    return true;
                case GateType.Or:
                    if (_Gates[gate_id].WireOut == "z45")
                        return true;
                    else if (_Gates[gate_id].WireOut[0] == 'z')
                        return false;

                    Gate in_gate1 = GetGateByOutputWire(_Gates[gate_id].WiresIn[0]);
                    Gate in_gate2 = GetGateByOutputWire(_Gates[gate_id].WiresIn[1]);

                    List<Gate> or_gate_output_gates = GetGatesByInputWire(_Gates[gate_id].WireOut);

                    if ((or_gate_output_gates[0].Type == GateType.Xor &&
                        or_gate_output_gates[1].Type == GateType.And) ||
                        (or_gate_output_gates[0].Type == GateType.And &&
                        or_gate_output_gates[1].Type == GateType.Xor))
                        return true;

                    return false;
                case GateType.Xor:
                    if ((_Gates[gate_id].WiresIn[0] == "x00" && _Gates[gate_id].WiresIn[1] == "y00") ||
                        (_Gates[gate_id].WiresIn[0] == "y00" && _Gates[gate_id].WiresIn[1] == "x00"))
                    {
                        if (_Gates[gate_id].WireOut != "z00") return false;

                        return true;
                    }
                    else if ((_Gates[gate_id].WiresIn[0][0] == 'x' && _Gates[gate_id].WiresIn[1][0] == 'y') ||
                        (_Gates[gate_id].WiresIn[0][0] == 'y' && _Gates[gate_id].WiresIn[1][0] == 'x'))
                    {
                        List<Gate> xor_gate_output_gates = GetGatesByInputWire(_Gates[gate_id].WireOut);

                        if (xor_gate_output_gates.Count != 2)
                            return false;

                        bool found_xor = false;

                        foreach (Gate g in xor_gate_output_gates)
                        {
                            if (g.Type == GateType.Xor) found_xor = true;
                        }

                        return found_xor;
                    }
                    else if (_Gates[gate_id].WireOut[0] == 'z' && _Gates[gate_id].WireOut != "z45")
                    {
                        return true;
                    }
                    
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string[] lines = sr.ReadToEnd().Split("\r\n");
                    int line = 0;
                    
                    while (true)
                    {
                        if (lines[line].Length == 0)
                            break;

                        string[] wires = lines[line].Split(':');
                        Circuit.AddWire(wires[0], int.Parse(wires[1]));
                        line++;
                    }

                    int gate_id = 0;

                    while (true)
                    {
                        line++;

                        if (line >= lines.Length)
                            break;

                        string[] gates = lines[line].Split(' ');

                        GateType new_gate_type = gates[1].Trim() switch
                        {
                            "AND" => GateType.And,
                            "OR" => GateType.Or,
                            "XOR" => GateType.Xor,
                            _ => throw new NotImplementedException(),
                        };

                        Circuit.AddGate(gate_id, new_gate_type, gates[0].Trim(), gates[2].Trim(), gates[4].Trim());

                        int bit = -1;

                        if (gates[0][0] == 'x' || gates[0][0] == 'y')
                            bit = int.Parse(gates[0].Substring(1));

                        if (bit == -1 && (gates[2][0] == 'x' || gates[2][0] == 'y'))
                            bit = int.Parse(gates[2].Substring(1));

                        if (bit == -1 && (gates[4][0] == 'z'))
                            bit = int.Parse(gates[4].Substring(1));

                        if (bit != -1)
                            Circuit.GetGate(gate_id).Bit = bit;

                        Circuit.AddWire(gates[4].Trim());
                        gate_id++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
                finally
                {
                    sr.Close();
                }
            }

            List<string> bad_outputs = new();

            foreach (int gate_id in Circuit.GetGates().Keys)
            {
                if (!Circuit.Validate(gate_id))
                {
                    Console.WriteLine($"Gate Id {gate_id} does not validate properly ({Circuit.GetGate(gate_id).Type.ToString()})");
                    bad_outputs.Add(Circuit.GetGate(gate_id).WireOut);
                }
            }

            bad_outputs.Sort();
            Console.WriteLine($"There were {bad_outputs.Count} gates that were incorrectly wired with the following output wires: {string.Join(',', bad_outputs)}");
            //Circuit.Exec();
            
            //Console.WriteLine($"The number represented by the zXX wire registers is {Circuit.GetZReg()}");
        }
    }
}
