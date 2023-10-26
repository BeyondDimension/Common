// Standard headers
#include <iostream>
#include <filesystem>
#include <cassert>

// Header files copied from https://github.com/dotnet/core-setup
#include <coreclr_delegates.h>
#include <hostfxr.h>

#ifdef WINDOWS
#include <Windows.h>

#define STR(s) L ## s
#define CH(c) L ## c
#define DIR_SEPARATOR L'\\'

#define string_compare wcscmp

#else
#include <dlfcn.h>
#include <limits.h>

#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'
#define MAX_PATH PATH_MAX

#define string_compare strcmp

#endif

using string_t = std::basic_string<char_t>;

namespace
{
	// Globals to hold hostfxr exports
	hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr;
	hostfxr_initialize_for_runtime_config_fn init_for_config_fptr;
	hostfxr_get_runtime_delegate_fn get_delegate_fptr;
	hostfxr_run_app_fn run_app_fptr;
	hostfxr_close_fn close_fptr;

	// Forward declarations
	bool load_hostfxr(const char_t* hostfxr_path);
	load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* assembly);
}

#if defined(WINDOWS)
int __cdecl wmain(int argc, wchar_t* argv[])
#else
int main(int argc, char* argv[])
#endif
{
	// Get the current executable's directory
   // This sample assumes the managed assembly to load and its runtime configuration file are next to the host
	char_t host_path[MAX_PATH];
#if WINDOWS
	auto size = ::GetFullPathNameW(argv[0], sizeof(host_path) / sizeof(char_t), host_path, nullptr);
	assert(size != 0);
#else
	auto resolved = realpath(argv[0], host_path);
	assert(resolved != nullptr);
#endif

	string_t root_path = host_path;
	auto pos = root_path.find_last_of(DIR_SEPARATOR);
	assert(pos != string_t::npos);
	root_path = root_path.substr(0, pos + 1);

	//
	// STEP 1: Load HostFxr and get exported hosting functions
	//
	const string_t hostfxr_path = L"C:\\Program Files\\dotnet\\host\\fxr\\8.0.0-rc.2.23479.6\\hostfxr.dll";
	if (!load_hostfxr(hostfxr_path.c_str()))
	{
		assert(false && "Failure: load_hostfxr()");
		return EXIT_FAILURE;
	}

	//
	// STEP 2: Initialize and start the .NET Core runtime
	//
	const string_t config_path = root_path + STR("assemblies\\NativeHost.Sample.runtimeconfig.json");
	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
	load_assembly_and_get_function_pointer = get_dotnet_load_assembly(config_path.c_str());
	assert(load_assembly_and_get_function_pointer != nullptr && "Failure: get_dotnet_load_assembly()");

	//
	// STEP 3: Load managed assembly and get function pointer to a managed method
	//
	const string_t dotnetlib_path = root_path + STR("assemblies\\NativeHost.Sample.dll");
	const char_t* dotnet_type = STR("NativeHost.Sample, Program");
	const char_t* dotnet_type_method = STR("Main");
	// <SnippetLoadAndGet>
	// Function pointer to managed delegate
	component_entry_point_fn custom = nullptr;
	int rc = load_assembly_and_get_function_pointer(
		dotnetlib_path.c_str(),
		dotnet_type,
		dotnet_type_method,
		nullptr /*delegate_type_name*/,
		nullptr,
		(void**)&custom);
	// </SnippetLoadAndGet>
	assert(rc == 0 && custom != nullptr && "Failure: load_assembly_and_get_function_pointer()");

	int exit_code = custom(NULL, 0);
	return exit_code;
}

/********************************************************************************************
 * Function used to load and activate .NET Core
 ********************************************************************************************/
namespace
{
	// Forward declarations
	void* load_library(const char_t*);
	void* get_export(void*, const char*);

#ifdef WINDOWS
	void* load_library(const char_t* path)
	{
		HMODULE h = ::LoadLibraryW(path);
		assert(h != nullptr);
		return (void*)h;
	}
	void* get_export(void* h, const char* name)
	{
		void* f = ::GetProcAddress((HMODULE)h, name);
		assert(f != nullptr);
		return f;
	}
#else
	void* load_library(const char_t* path)
	{
		void* h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
		assert(h != nullptr);
		return h;
	}
	void* get_export(void* h, const char* name)
	{
		void* f = dlsym(h, name);
		assert(f != nullptr);
		return f;
	}
#endif

	// <SnippetLoadHostFxr>
	// Using the nethost library, discover the location of hostfxr and get exports
	bool load_hostfxr(const char_t* hostfxr_path)
	{
		void* lib = load_library(hostfxr_path);
		init_for_cmd_line_fptr = (hostfxr_initialize_for_dotnet_command_line_fn)get_export(lib, "hostfxr_initialize_for_dotnet_command_line");
		init_for_config_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
		get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
		run_app_fptr = (hostfxr_run_app_fn)get_export(lib, "hostfxr_run_app");
		close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

		return (init_for_config_fptr && get_delegate_fptr && close_fptr);
	}
	// </SnippetLoadHostFxr>

	// <SnippetInitialize>
	// Load and initialize .NET Core and get desired function pointer for scenario
	load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* config_path)
	{
		// Load .NET Core
		void* load_assembly_and_get_function_pointer = nullptr;
		hostfxr_handle cxt = nullptr;
		int rc = init_for_config_fptr(config_path, nullptr, &cxt);
		if (rc != 0 || cxt == nullptr)
		{
			std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
			close_fptr(cxt);
			return nullptr;
		}

		// Get the load assembly function pointer
		rc = get_delegate_fptr(
			cxt,
			hdt_load_assembly_and_get_function_pointer,
			&load_assembly_and_get_function_pointer);
		if (rc != 0 || load_assembly_and_get_function_pointer == nullptr)
			std::cerr << "Get delegate failed: " << std::hex << std::showbase << rc << std::endl;

		close_fptr(cxt);
		return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
	}
	// </SnippetInitialize>
}