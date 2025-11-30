import sys
from pathlib import Path

import pythonnet
pythonnet.load()                # 先加载 .NET 运行时
import clr                      # 再导入 clr

# 1) 指向包含 DLL 的目录
DLL_DIR = Path(r"E:\github\PythonNetSample\src\Python\pyAcsharp\csharp\csharpLib\csharpLib\bin\Debug\net8.0")
sys.path.append(str(DLL_DIR))

# 2) 用绝对路径加载程序集（最稳）
asm = clr.AddReference(str(DLL_DIR / "csharpLib.dll"))

# 3) （可选）打印出 DLL 里导出的类型，用来确认命名空间和类是否可见
exported = {t.FullName for t in asm.GetExportedTypes()}
print("Exported types:", *sorted(exported), sep="\n  ")

# 找一下 Calc 的全名（如 'csharpLib.Calc'），拿到命名空间
calc_fullname = next((name for name in exported if name.endswith(".Calc") or name == "Calc"), None)
assert calc_fullname, "DLL 里没找到 public Calc 类型，请检查是否为 public 或命名空间是否正确。"

ns = calc_fullname.rsplit(".", 1)[0] if "." in calc_fullname else calc_fullname  # 命名空间
print("Namespace detected:", ns)
# 4) 按命名空间导入 Calc
mod = __import__(ns, fromlist=['Calc'])
Calc = getattr(mod, 'Calc')
# 直接调用
calc = Calc()
print("Add(2,3) =", calc.Add(2, 3))

