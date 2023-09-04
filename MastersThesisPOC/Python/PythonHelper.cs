namespace MastersThesisPOC.Python
{
    public class PythonHelper : IPythonHelper
    {
        private readonly Microsoft.Scripting.Hosting.ScriptEngine _engine;
        private readonly Microsoft.Scripting.Hosting.ScriptScope _scope;
        public PythonHelper(Microsoft.Scripting.Hosting.ScriptEngine engine)
        {
            _engine = engine;
            _scope = _engine.CreateScope();
        }

        public string GetStringPatternOfInteger(float input)
        {
            _engine.Execute("import struct", _scope);
            var source = $@"
import struct

M = {input}
fpNumberBytes = struct.pack('f', 1.0/M)
mantissaInt = struct.unpack('!L', fpNumberBytes)[0] & int('00000000011111111111111111111111', base = 2)
mantissaIntBinary = bin(mantissaInt)[2:].zfill(23)";
            _engine.Execute(source, _scope);

            return _scope.GetVariable<string>("mantissaIntBinary");
        }

        public string GetStringPatternOfFloat(float input)
        {
            _engine.Execute("import struct", _scope);
            var source = $@"
import struct

M = {input}
div_result = 1.0 / M
fpNumberBytes = struct.pack('f', div_result)
mantissaFloat = struct.unpack('!L', fpNumberBytes)[0] & float('00000000011111111111111111111111', base = 2)
mantissaFloatBinary = bin(mantissaFloat)[2:].zfill(23)";
            _engine.Execute(source, _scope);

            return _scope.GetVariable<string>("mantissaFloatBinary");
        }
    }
}
