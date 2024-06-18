#include "CLR.cpp"

void fnNativeHostStaticLibCLR()
{
	NativeHost::CLR clr = NativeHost::CLR();
	clr.execute_assembly();
}
