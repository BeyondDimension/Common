#include <metahost.h>
#include <windows.h>
#include <string>
#include <shellapi.h>

#pragma comment(lib, "mscoree.lib")

#import <mscorlib.tlb> raw_interfaces_only			\
    	high_property_prefixes("_get","_put","_putref")		\
    	rename("ReportEvent", "InteropServices_ReportEvent")	\
	rename("or", "InteropServices_or")
using namespace mscorlib;
#pragma endregion

namespace NativeHost
{
	class CLR
	{
	public:
		int execute_assembly() {
			std::wstring wNetVersion = L"v4.0.30319";

			hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pCLRMetaHost));
			if (FAILED(hr))
			{
				cleanup(safeArrayArgs, pCorRuntimeHost, pCLRRuntimeInfo, pCLRMetaHost);
				return hr;
			}


			hr = pCLRMetaHost->GetRuntime(wNetVersion.c_str(), IID_PPV_ARGS(&pCLRRuntimeInfo));
			if (FAILED(hr))
			{
				cleanup(safeArrayArgs, pCorRuntimeHost, pCLRRuntimeInfo, pCLRMetaHost);
				return hr;
			}

			hr = pCLRRuntimeInfo->IsLoadable(&isLoadable);
			if (FAILED(hr))
			{
				cleanup(safeArrayArgs, pCorRuntimeHost, pCLRRuntimeInfo, pCLRMetaHost);
				return hr;
			}

			hr = pCLRRuntimeInfo->GetInterface(CLSID_CorRuntimeHost, IID_PPV_ARGS(&pCorRuntimeHost));
			if (FAILED(hr))
			{
				cleanup(safeArrayArgs, pCorRuntimeHost, pCLRRuntimeInfo, pCLRMetaHost);
				return hr;
			}
		}
	private:
		ICLRRuntimeInfo* pCLRRuntimeInfo = NULL;
		ICorRuntimeHost* pCorRuntimeHost = NULL;
		ICLRMetaHost* pCLRMetaHost = NULL;
		_MethodInfoPtr	 pMethodInfo = NULL;
		_AssemblyPtr	 spAssembly = NULL;
		IUnknownPtr		 spAppDomainThunk = NULL;
		_AppDomainPtr	 spDefaultAppDomain = NULL;
		SAFEARRAY* safeArrayArgs = NULL;
		VARIANT			 retVal, obj, vtPsa;
		BOOL			 isLoadable = FALSE;
		HRESULT			 hr;

		void cleanup(SAFEARRAY* pSafeArray, ICorRuntimeHost* pCorRuntimeHost, ICLRRuntimeInfo* pCLRRuntimeInfo, ICLRMetaHost* pCLRMetaHost) {
			if (pCLRMetaHost)
			{
				pCLRMetaHost->Release();
				pCLRMetaHost = NULL;
			}
			if (pCLRRuntimeInfo)
			{
				pCLRRuntimeInfo->Release();
				pCLRRuntimeInfo = NULL;
			}
			if (pSafeArray)
			{
				SafeArrayDestroy(pSafeArray);
				pSafeArray = NULL;
			}
			if (pCorRuntimeHost) {
				pCorRuntimeHost->Stop();
				pCorRuntimeHost->Release();
			}
			return;
		}
	};
}