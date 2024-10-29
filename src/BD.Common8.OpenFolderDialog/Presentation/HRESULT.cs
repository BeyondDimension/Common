#pragma warning disable CS8603 // 可能返回 null 引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8600 // 将 null 文本或可能的 null 值转换为不可为 null 类型。

namespace MS.Win32;

[StructLayout(LayoutKind.Explicit)]
internal struct HRESULT
{
    [FieldOffset(0)]
    private readonly uint _value;

    public static readonly HRESULT S_OK = new HRESULT(0u);

    public static readonly HRESULT S_FALSE = new HRESULT(1u);

    public static readonly HRESULT E_NOTIMPL = new HRESULT(2147500033u);

    public static readonly HRESULT E_NOINTERFACE = new HRESULT(2147500034u);

    public static readonly HRESULT E_POINTER = new HRESULT(2147500035u);

    public static readonly HRESULT E_ABORT = new HRESULT(2147500036u);

    public static readonly HRESULT E_FAIL = new HRESULT(2147500037u);

    public static readonly HRESULT E_UNEXPECTED = new HRESULT(2147549183u);

    public static readonly HRESULT DISP_E_MEMBERNOTFOUND = new HRESULT(2147614723u);

    public static readonly HRESULT DISP_E_TYPEMISMATCH = new HRESULT(2147614725u);

    public static readonly HRESULT DISP_E_UNKNOWNNAME = new HRESULT(2147614726u);

    public static readonly HRESULT DISP_E_EXCEPTION = new HRESULT(2147614729u);

    public static readonly HRESULT DISP_E_OVERFLOW = new HRESULT(2147614730u);

    public static readonly HRESULT DISP_E_BADINDEX = new HRESULT(2147614731u);

    public static readonly HRESULT DISP_E_BADPARAMCOUNT = new HRESULT(2147614734u);

    public static readonly HRESULT DISP_E_PARAMNOTOPTIONAL = new HRESULT(2147614735u);

    public static readonly HRESULT SCRIPT_E_REPORTED = new HRESULT(2147614977u);

    public static readonly HRESULT STG_E_INVALIDFUNCTION = new HRESULT(2147680257u);

    public static readonly HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER = new HRESULT(2147749635u);

    public static readonly HRESULT E_ACCESSDENIED = new HRESULT(2147942405u);

    public static readonly HRESULT E_OUTOFMEMORY = new HRESULT(2147942414u);

    public static readonly HRESULT E_INVALIDARG = new HRESULT(2147942487u);

    public static readonly HRESULT COR_E_OBJECTDISPOSED = new HRESULT(2148734498u);

    public static readonly HRESULT WC_E_GREATERTHAN = new HRESULT(3222072867u);

    public static readonly HRESULT WC_E_SYNTAX = new HRESULT(3222072877u);

    public Facility Facility => GetFacility((int)_value);

    public int Code => GetCode((int)_value);

    public bool Succeeded => (int)_value >= 0;

    public bool Failed => (int)_value < 0;

    public HRESULT(uint i)
    {
        _value = i;
    }

    public static HRESULT Make(bool severe, Facility facility, int code)
    {
        return new HRESULT((severe ? 2147483648u : 0u) | (uint)((int)facility << 16) | (uint)code);
    }

    public static Facility GetFacility(int errorCode)
    {
        return (Facility)((errorCode >> 16) & 0x1FFF);
    }

    public static int GetCode(int error)
    {
        return error & 0xFFFF;
    }

    public override string ToString()
    {
        FieldInfo[] fields = typeof(HRESULT).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (FieldInfo fieldInfo in fields)
        {
            if (fieldInfo.FieldType == typeof(HRESULT))
            {
                HRESULT hRESULT = (HRESULT)fieldInfo.GetValue(null);
                if (hRESULT == this)
                {
                    return fieldInfo.Name;
                }
            }
        }

        if (Facility == Facility.Win32)
        {
            FieldInfo[] fields2 = typeof(Win32Error).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fieldInfo2 in fields2)
            {
                if (fieldInfo2.FieldType == typeof(Win32Error))
                {
                    Win32Error win32Error = (Win32Error)fieldInfo2.GetValue(null);
                    if ((HRESULT)win32Error == this)
                    {
                        return "HRESULT_FROM_WIN32(" + fieldInfo2.Name + ")";
                    }
                }
            }
        }

        return string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", new object[1] { _value });
    }

    public override bool Equals(object obj)
    {
        try
        {
            return ((HRESULT)obj)._value == _value;
        }
        catch (InvalidCastException)
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
    {
        return hrLeft._value == hrRight._value;
    }

    public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
    {
        return !(hrLeft == hrRight);
    }

    public void ThrowIfFailed()
    {
        ThrowIfFailed(null);
    }

    [SecurityCritical]
    public void ThrowIfFailed(string message)
    {
        Exception exception = GetException(message);
        if (exception != null)
        {
            throw exception;
        }
    }

    public Exception GetException()
    {
        return GetException(null);
    }

    [SecurityCritical]
    public Exception GetException(string message)
    {
        if (!Failed)
        {
            return null;
        }

        Exception ex = Marshal.GetExceptionForHR((int)_value, new IntPtr(-1));
        if (ex.GetType() == typeof(COMException))
        {
            Facility facility = Facility;
            ex = (facility != Facility.Win32) ? ((ExternalException)new COMException(message ?? ex.Message, (int)_value)) : ((ExternalException)((!string.IsNullOrEmpty(message)) ? new Win32Exception(Code, message) : new Win32Exception(Code)));
        }
        else if (!string.IsNullOrEmpty(message))
        {
            ConstructorInfo constructor = ex.GetType().GetConstructor(new Type[1] { typeof(string) });
            if (constructor != null)
            {
                ex = constructor.Invoke([message]) as Exception;
            }
        }

        return ex;
    }
}