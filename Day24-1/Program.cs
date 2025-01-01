using System.IO;
using System.Linq;
using System.Text;

namespace Day24_1
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
        private GateType _Type;
        private string _WireIn1;
        private string _WireIn2;
        private string _WireOut;
        private int _Value = -1;

        public Gate(GateType type, string wire_in1, string wire_in2, string wire_out)
        {
            _Type = type;
            _WireIn1 = wire_in1;
            _WireIn2 = wire_in2;
            _WireOut = wire_out;
        }

        public GateState GetState()
        {
            if (_Value != -1) return GateState.Executed;

            if (Circuit.GetWireSignal(_WireIn1) == -1 || Circuit.GetWireSignal(_WireIn2) == -1)
                return GateState.Waiting;

            return GateState.Ready;
        }

        public void Execute()
        {
            if (_Value != -1) return;

            switch (_Type)
            {
                case GateType.And:
                    _Value = Circuit.GetWireSignal(_WireIn1) & Circuit.GetWireSignal(_WireIn2);                    
                    break;
                case GateType.Or:
                    _Value = Circuit.GetWireSignal(_WireIn1) | Circuit.GetWireSignal(_WireIn2);
                    break;
                case GateType.Xor:
                    _Value = Circuit.GetWireSignal(_WireIn1) ^ Circuit.GetWireSignal(_WireIn2);
                    break;
                default:
                    throw new NotImplementedException();
            }

            Circuit.SendWireSignal(_WireOut, _Value);
        }
    }

    internal static class Circuit
    {
        private static Dictionary<string, int> _Wires = new();
        private static List<Gate> _Gates = new();

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

        public static void AddGate(GateType gate_type, string wire_in1, string wire_in2, string wire_out)
        {
            _Gates.Add(new Gate(gate_type, wire_in1, wire_in2, wire_out));
        }

        public static int GetWireSignal(string wire_name)
        {
            return _Wires[wire_name];
        }

        public static void SendWireSignal(string wire_name, int value)
        {
            _Wires[wire_name] = value;
        }

        public static List<Gate> GetGates()
        {
            return _Gates;
        }

        public static string GetZBits()
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

                        Circuit.AddGate(new_gate_type, gates[0].Trim(), gates[2].Trim(), gates[4].Trim());
                        Circuit.AddWire(gates[4].Trim());
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

            while (true)
            {
                List<Gate> gates = Circuit.GetGates();

                int ready_gates = 0;

                foreach (Gate g in gates)
                {
                    if (g.GetState() == GateState.Ready)
                    {
                        ready_gates++;
                        g.Execute();
                    }
                }

                if (ready_gates == 0)
                    break;
            }

            string z_reg = Circuit.GetZBits();
            long z_long = Convert.ToInt64(z_reg, 2);

            Console.WriteLine($"The number represented by the zXX wire registers is {z_long}");
        }
    }
}
